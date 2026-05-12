public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        const string query = @"
            SELECT UserID, Username, Email, Password, Role
            FROM Users
            WHERE (Username = @Identifier OR Email = @Identifier)
            LIMIT 1;
        ";

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Identifier", identifier);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.Read())
            return null;

        return new User
        {
            UserID = reader.GetInt32("UserID"),
            Username = reader.GetString("Username"),
            Email = reader.GetString("Email"),
            Password = reader.GetString("Password"),
            Role = reader.GetString("Role")
        };
    }

    public async Task AssignRoleAsync(int userId, string role)
    {
        const string query = @"
            UPDATE Users
            SET Role = @Role
            WHERE UserID = @UserID;
        ";

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Role", role);
        command.Parameters.AddWithValue("@UserID", userId);

        await command.ExecuteNonQueryAsync();
    }
}
