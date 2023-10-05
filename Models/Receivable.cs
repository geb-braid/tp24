using System.ComponentModel.DataAnnotations;

namespace tp24_api.Models;

// Note that some `Required` fields are also marked as optional data-types. This
// is because of how structs, e.g. `decimal` and `DateTime`, are handled when
// missing in JSON. Essentially, they'll get deserialized to their `default`
// value (e.g. `PaidValue = 0m`), and no missing field error will occur. Since
// this is counter-intuitive to clients, we use the workaround suggested in
// <https://learn.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api#data-annotations>.

public class Receivable
{
    [Key]
    public long? Id { get; set; }

    [Required]
    [MaxLength(50)]
    // https://www.tronia.com/webhelp/ar/AR_Reference_Numbers.htm
    public string Reference { get; set; } = string.Empty;

    [Required]
    [MaxLength(3)]
    // https://www.iso.org/iso-4217-currency-codes.html
    public string CurrencyCode { get; set; } = string.Empty;

    [Required]
    public DateTime? IssueDate { get; set; }

    // https://www.planprojections.com/projections/accounts-receivable-opening-balance/
    [Required]
    public decimal? OpeningValue { get; set; }

    [Required]
    public decimal? PaidValue { get; set; }

    [Required]
    public DateTime? DueDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public bool? Cancelled { get; set; }

    [Required]
    [MaxLength(50)]
    public string DebtorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DebtorReference { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DebtorAddress1 { get; set; }

    [MaxLength(50)]
    public string? DebtorAddress2 { get; set; }

    [MaxLength(50)]
    public string? DebtorTown { get; set; }

    [MaxLength(50)]
    public string? DebtorState { get; set; }

    [MaxLength(50)]
    public string? DebtorZip { get; set; }

    [Required]
    [MaxLength(50)]
    public string DebtorCountryCode { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DebtorRegistrationNumber { get; set; } = string.Empty;
}