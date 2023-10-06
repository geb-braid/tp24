namespace tp24_api.Models;

public readonly record struct ReceivablesReport(
    Dictionary<string, Dictionary<string, Stats>> Summary,
    IEnumerable<Receivable>? Receivables = null);

public readonly record struct Stats(decimal Total, decimal Max);