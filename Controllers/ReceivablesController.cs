using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp24_api.Models;

namespace tp24_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivablesController : ControllerBase
    {
        private readonly ReceivablesContext _context;

        public ReceivablesController(ReceivablesContext context)
        {
            _context = context;
        }

        // GET: api/receivables
        [HttpGet]
        public async Task<ActionResult<ReceivablesReport>> GetReceivables([FromQuery] DateTime startDate, [FromQuery] bool summaryOnly = false)
        {
            if (_context.Receivables == null)
            {
                return NotFound();
            }

            var receivables =
            (
                from r in _context.Receivables.AsQueryable<Receivable>()
                where r.IssueDate > startDate && r.Cancelled != true
                select r
            ).AsAsyncEnumerable();

            // I tried combining the below query with the above, so that it's
            // processed server-side, but translating that LINQ query to the
            // in-memory provider was not supported by the framework.

            // IsOpen -> CurrencyCode -> (Total, Max)
            var statsGroupings =
                from receivable in receivables
                group receivable by new
                {
                    IsOpen = !receivable.ClosedDate.HasValue
                } into openClosedReceivables
                from currencyValues in
                (
                    from receivable in openClosedReceivables
                    group receivable by receivable.CurrencyCode into currencies
                    select new
                    {
                        CurrencyCode = currencies.Key,
                        Total = currencies.SumAsync(r => (r.OpeningValue ?? 0m) - (r.PaidValue ?? 0m)),
                        Max = currencies.MaxAsync(r => (r.OpeningValue ?? 0m) - (r.PaidValue ?? 0m))
                    }
                )
                group currencyValues by openClosedReceivables.Key;

            var statsByStatusAndCurrency = await statsGroupings.ToDictionaryAwaitAsync(
                grouping => ValueTask.FromResult(grouping.Key.IsOpen ? "Open" : "Closed"),
                async grouping => await grouping.ToDictionaryAwaitAsync(
                    line => ValueTask.FromResult(line.CurrencyCode),
                    async line => new Stats (await line.Total, await line.Max)));

            // note that we do not list cancelled receivables
            var reportedReceivables = summaryOnly ? null : await receivables.ToListAsync();
            return new ReceivablesReport(statsByStatusAndCurrency, reportedReceivables);
        }

        // GET: api/receivables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Receivable>> GetReceivable(long id)
        {
            if (_context.Receivables == null)
            {
                return NotFound();
            }

            var receivable = await _context.Receivables.FindAsync(id);
            if (receivable == null)
            {
                return NotFound();
            }

            return receivable;
        }

        // PUT: api/receivables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceivable(long id, Receivable receivable)
        {
            if (receivable.Id == null)
            {
                receivable.Id = id;
            }
            else if (receivable.Id != id)
            {
                return BadRequest($"Cannot modify the {nameof(receivable.Id)} field.");
            }

            _context.Entry(receivable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceivableExists(id))
                {
                    // Item never existed or was deleted while we were trying to update it -> fail
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/receivables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Receivable>> PostReceivable(Receivable receivable)
        {
            if (receivable.Id != null)
            {
                return BadRequest($"Cannot pre-set the {nameof(receivable.Id)} field.");
            }
            if (_context.Receivables == null)
            {
                return Problem($"Entity set '{nameof(ReceivablesContext.Receivables)}' is null.");
            }

            _context.Receivables.Add(receivable);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReceivable), new { id = receivable.Id }, receivable);
        }

        // DELETE: api/receivables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceivable(long id)
        {
            if (_context.Receivables == null)
            {
                return NotFound();
            }

            var receivable = await _context.Receivables.FindAsync(id);
            if (receivable == null)
            {
                // our DELETE implementation is idempotent
                return NoContent(); // silent exit
            }

            _context.Receivables.Remove(receivable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReceivableExists(long id)
        {
            return _context.Receivables?.Any(e => e.Id == id) ?? false;
        }
    }
}
