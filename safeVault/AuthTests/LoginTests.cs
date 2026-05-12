public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoginTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Fail_With_Invalid_Credentials()
    {
        var payload = new
        {
            Identifier = "unknownUser",
            Password = "wrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/login", payload);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
