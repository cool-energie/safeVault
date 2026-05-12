namespace AuthTests;

public class UnitTest1
{
    [Fact]
    public async Task Access_Admin_Route_Without_Login_Should_Return_401()
    {
        var response = await _client.GetAsync("/admin/dashboard");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task User_Role_Should_Not_Access_Admin_Route()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            });

        var client = factory.CreateClient();

        // Fake user with role "user"
        TestAuthHandler.Role = "user";

        var response = await client.GetAsync("/admin/dashboard");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Role_Should_Access_Admin_Route()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            });

        var client = factory.CreateClient();

        // Fake admin
        TestAuthHandler.Role = "admin";

        var response = await client.GetAsync("/admin/dashboard");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
