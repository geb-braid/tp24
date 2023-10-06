using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp24_api.Models;

namespace tp24_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivablesController : ControllerBase
    {
        private readonly IReceivablesRepository _repository;
        public ReceivablesController(IReceivablesRepository repository)
            => _repository = repository;

        // GET: api/receivables
        [HttpGet]
        public async Task<ActionResult<ReceivablesReport>> GetReceivables([FromQuery] DateTime startDate, [FromQuery] bool summaryOnly = false)
        {
            return await _repository.Read(startDate, summaryOnly);
        }

        // GET: api/receivables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Receivable>> GetReceivable(long id)
        {
            var receivable = await _repository.Read(id);
            return receivable != null
                ? receivable
                : NotFound();
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

            return await _repository.Update(receivable)
                ? NoContent()
                : NotFound();
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

            await _repository.Create(receivable);
            return CreatedAtAction(nameof(GetReceivable), new { id = receivable.Id }, receivable);
        }

        // DELETE: api/receivables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceivable(long id)
        {
            await _repository.Delete(id);
            return NoContent();
        }
    }
}
