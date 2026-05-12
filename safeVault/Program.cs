var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("loginLimiter", limiter =>
    {
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.PermitLimit = 5; // 5 attempts per minute
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddAuthentication("AppCookie")
    .AddCookie("AppCookie", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/forbidden";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
});

app.MapGet("/admin/dashboard", () => "Admin Panel")
   .RequireAuthorization("AdminOnly");

app.MapGet("/profile", () => "User Profile")
   .RequireAuthorization("UserOnly");

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("AppCookie");
    return Results.Ok("Logged out");
});

app.MapPost("/login", async (HttpContext context, AuthService auth, UserRepository repo, LoginRequest req) =>
{
    var user = await repo.GetUserByIdentifierAsync(req.Identifier);

    if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
        return Results.BadRequest("Invalid credentials");

    await auth.SignInUserAsync(context, user);

    return Results.Ok("Logged in");
}).RequireRateLimiting("loginLimiter");;

app.Run();

