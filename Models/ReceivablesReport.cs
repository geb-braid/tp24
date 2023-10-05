namespace tp24_api.Models;

public class ReceivablesReport
{
    public Summary Summary { get; }
    public IEnumerable<Receivable>? Receivables { get; }

    public ReceivablesReport(Summary summary, IEnumerable<Receivable>? receivables)
    {
        Summary = summary;
        Receivables = receivables;
    }
}

public class Summary
{
    public decimal TotalOpenValue { get; }
    public decimal TotalClosedValue { get; }
}