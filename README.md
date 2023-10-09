# TP24 Accounts Receivables Web API

This project implements a simple web API for an accounts receivables log.

Receivables are debts owed to a company for goods or services which have been provided but not yet paid for. Invoices and credit notes can be considered types of receivables.

## Setup

This project uses .NET Core, with ASP.NET and Entity Framework. xUnit is used for testing, with Moq.

To run this locally:

```sh
dotnet build
dotnet run
```

ASP.NET web API quick start documentation:
* <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0>
* <https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0>

## Project Requirements

* Accept a payload containing receivables data, and store it
* Return summary statistics about the stored receivables data; specifically the value of open and closed invoices
* Use the following format:

  ```json
  [
    {
      "reference": "string",
      "currencyCode": "string",
      "issueDate": "string",
      "openingValue": 1234.56,
      "paidValue": 1234.56,
      "dueDate": "string",
      "closedDate": "string", //optional
      "cancelled": true | false, // optional
      "debtorName": "string",
      "debtorReference": "string",
      "debtorAddress1": "string", //optional
      "debtorAddress2": "string", //optional
      "debtorTown": "string", //optional
      "debtorState": "string", //optional
      "debtorZip": "string", //optional
      "debtorCountryCode": "string",
      "debtorRegistrationNumber": "string" //optional
    }
  ]
  ```

## Testing

```sh
dotnet test
```

Notes:
* Integration testing for repository LINQ queries:
  * There's no good way to unit-test LINQ queries in Entity Framework
  * This article recommends testing in production with integration tests instead: <https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy>
* Unit testing ASP.NET controllers: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-7.0>
* Moq: <https://github.com/devlooped/moq/wiki/Quickstart#async-methods>

## TODO

1. ASP.NET integration tests: <https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0>
1. Metrics, alarms & dashboards
