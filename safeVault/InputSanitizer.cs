using System.Text.RegularExpressions;
using System.Web;

public static class InputSanitizer
{
    // Removes HTML, trims spaces, normalizes whitespace
    public static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove HTML tags
        string noHtml = Regex.Replace(input, "<.*?>", string.Empty);

        // Decode HTML entities (&amp; → &, &lt; → <)
        string decoded = HttpUtility.HtmlDecode(noHtml);

        // Remove extra whitespace
        string normalized = Regex.Replace(decoded, @"\s+", " ").Trim();

        return normalized;
    }

    // Strict username sanitizer (letters, numbers, underscore, dot)
    public static string CleanUsername(string username)
    {
        username = Clean(username);
        return Regex.Replace(username, @"[^a-zA-Z0-9._]", "");
    }

    // Email sanitizer (keeps only valid email characters)
    public static string CleanEmail(string email)
    {
        email = Clean(email);
        return Regex.Replace(email, @"[^a-zA-Z0-9@._\-+]", "");
    }
}
