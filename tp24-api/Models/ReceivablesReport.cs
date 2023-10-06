using System.Collections.Immutable;

namespace tp24_api.Models;

public readonly record struct ReceivablesReport(
    ImmutableDictionary<string, ImmutableDictionary<string, Stats>> Summary,
    IEnumerable<Receivable>? Receivables = null);

public readonly record struct Stats(decimal Total, decimal Max);