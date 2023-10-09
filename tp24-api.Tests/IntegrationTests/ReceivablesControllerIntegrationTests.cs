using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using tp24_api.Models;

namespace tp24_api.IntegrationTests;

public class ReceivablesControllerIntegrationTests : IDisposable
{
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;

    public ReceivablesControllerIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Get_what_was_Posted()
    {
        using var response = await _client.PostAsJsonAsync("api/receivables", Receivables.CLOSED_USD);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdReceivable = await response.Content.ReadFromJsonAsync<Receivable>();
        Assert.NotNull(createdReceivable?.Id);

        var returnedReceivable = await _client.GetFromJsonAsync<Receivable>($"api/receivables/{createdReceivable.Id}");
        Assert.NotNull(returnedReceivable);
        Assert.Equal(Receivables.CLOSED_USD.Reference, returnedReceivable.Reference);
        Assert.Equal(Receivables.CLOSED_USD.ClosedDate, returnedReceivable.ClosedDate);
    }

    public static class Receivables
    {
        public static readonly Receivable CLOSED_USD = CLOSED("USD");
        public static readonly Receivable CLOSED_GBP = CLOSED("GBP");

        public static Receivable CLOSED(string currencyCode) => new()
        {
            Reference = "reference",
            CurrencyCode = currencyCode,
            IssueDate = ParseIso8601("2023-10-06T07:05:20.257Z"),
            OpeningValue = 9,
            PaidValue = 4,
            DueDate = ParseIso8601("2023-10-06T07:05:20.257Z"),
            ClosedDate = ParseIso8601("2023-10-06T07:05:20.257Z"),
            DebtorName = "debtorName",
            DebtorReference = "debtorReference",
            DebtorAddress1 = "debtorAddress1",
            DebtorAddress2 = "debtorAddress2",
            DebtorTown = "debtorTown",
            DebtorState = "debtorState",
            DebtorZip = "debtorZip",
            DebtorCountryCode = "debtorCountryCode",
            DebtorRegistrationNumber = "debtorRegistrationNumber"
        };

        private static DateTime ParseIso8601(string iso8601)
            => DateTime.Parse(iso8601, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}