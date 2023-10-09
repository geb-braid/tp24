using Microsoft.AspNetCore.Mvc.Testing;

namespace tp24_api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Not strictly necessary in this example, but can be useful if we want to
    // modify the test environment behavior to avoid affecting production.
}