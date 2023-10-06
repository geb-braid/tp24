using Microsoft.EntityFrameworkCore;

namespace tp24_api.Models;

public class ReceivablesRepository : IReceivablesRepository
{
    private readonly ReceivablesContext _context;
    public ReceivablesRepository(ReceivablesContext context)
        => _context = context;

    public async Task Create(Receivable receivable)
    {
        ThrowIfMissingContext();
        _context.Receivables.Add(receivable);
        await _context.SaveChangesAsync();
    }

    public async Task<Receivable?> Read(long id)
    {
        ThrowIfMissingContext();
        return await _context.Receivables.FindAsync(id);
    }

    public async Task<ReceivablesReport> Read(DateTime startDate, bool summaryOnly)
    {
        ThrowIfMissingContext();

        var receivables =
        (
            from r in _context.Receivables.AsQueryable<Receivable>()
            where r.IssueDate > startDate && r.Cancelled != true
            select r
        ).AsAsyncEnumerable();

        // I tried combining the below query with the above, so that it's
        // processed server-side, but the framework did not support translating
        // the combined LINQ query to the in-memory provider.

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
                    Total = currencies.SumAsync(PaymentRemainder),
                    Max = currencies.MaxAsync(PaymentRemainder)
                }
            )
            group currencyValues by openClosedReceivables.Key;

        var statsByStatusAndCurrency = await statsGroupings.ToDictionaryAwaitAsync(
            grouping => ValueTask.FromResult(grouping.Key.IsOpen ? "Open" : "Closed"),
            async grouping => await grouping.ToDictionaryAwaitAsync(
                line => ValueTask.FromResult(line.CurrencyCode),
                async line => new Stats (await line.Total, await line.Max)));

        // note that we excluded cancelled receivables from our report
        var reportedReceivables = summaryOnly ? null : await receivables.ToListAsync();
        return new ReceivablesReport(statsByStatusAndCurrency, reportedReceivables);
    }

    public async Task<bool> Update(Receivable receivable)
    {
        _context.Entry(receivable).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReceivableExists(receivable.Id ?? 0))
            {
                // Item never existed or was deleted while we were trying to update it -> fail
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    public async Task Delete(long id)
    {
        ThrowIfMissingContext();

        var receivable = await _context.Receivables.FindAsync(id);
        if (receivable == null)
        {
            // our DELETE implementation is idempotent
            return; // silent exit
        }

        _context.Receivables.Remove(receivable);
        await _context.SaveChangesAsync();
    }

    private void ThrowIfMissingContext()
    {
        if (_context.Receivables == null)
        {
            throw new NullReceivablesContextException($"Entity set '{nameof(ReceivablesContext.Receivables)}' is null.");
        }
    }

    private bool ReceivableExists(long id)
    {
        return _context.Receivables?.Any(e => e.Id == id) ?? false;
    }

    private decimal PaymentRemainder(Receivable receivable)
        => (receivable.OpeningValue ?? 0m) - (receivable.PaidValue ?? 0m);
}

[System.Serializable]
public class NullReceivablesContextException : System.Exception
{
    public NullReceivablesContextException() { }
    public NullReceivablesContextException(string message) : base(message) { }
    public NullReceivablesContextException(string message, System.Exception inner) : base(message, inner) { }
    protected NullReceivablesContextException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}