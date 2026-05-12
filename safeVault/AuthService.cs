using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

public class AuthService
{
    private readonly string _connectionString;
    private readonly ILogger<AuthService> _logger;

    public AuthService(string connectionString, ILogger<AuthService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<User?> LoginAsync(string identifier, string password)
    {
        // OWASP A05: Input sanitization & normalization
        identifier = SecuritySanitizer.Sanitize(identifier);

        const string query = @"
            SELECT UserID, Username, Email, Password
            FROM Users
            WHERE (Username = @Identifier OR Email = @Identifier)
            LIMIT 1;
        ";

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);

            // OWASP A03: Prevent SQL Injection (parameterized queries)
            command.Parameters.AddWithValue("@Identifier", identifier);

            using var reader = await command.ExecuteReaderAsync();

            if (!reader.Read())
            {
                // OWASP A09: Log failed login attempts (without sensitive data)
                _logger.LogWarning("Failed login attempt for identifier: {Identifier}", identifier);
                return null;
            }

            string storedHash = reader.GetString("Password");

            // OWASP A02: Cryptographic Failures — verify using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, storedHash))
            {
                _logger.LogWarning("Invalid password for user: {Identifier}", identifier);
                return null;
            }

            // OWASP A07: Authentication success
            return new User
            {
                UserID = reader.GetInt32("UserID"),
                Username = reader.GetString("Username"),
                Email = reader.GetString("Email")
            };
        }
        catch (Exception ex)
        {
            // OWASP A09: Logging without leaking sensitive data
            _logger.LogError(ex, "Unexpected error during login process");

            // OWASP A05: Do not reveal internal errors to the client
            return null;
        }
    }
}
