// Tests/TestInputValidation.cs
using NUnit.Framework;
[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        // Simulated SQL injection payload
        string maliciousInput = "admin' OR '1'='1; DROP TABLE Users; --";

        string sanitized = SecuritySanitizer.Sanitize(maliciousInput);

        // Assert that dangerous SQL characters are removed
        Assert.IsFalse(sanitized.Contains("'"));
        Assert.IsFalse(sanitized.Contains(";"));
        Assert.IsFalse(sanitized.Contains("--"));
        Assert.IsFalse(sanitized.Contains("OR 1=1"));
    }
    [Test]
    public void TestForXSS()
    {
        // Simulated XSS payload
        string maliciousInput = "<script>alert('XSS')</script><img src=x onerror=alert(1)>";

        string sanitized = SecuritySanitizer.Sanitize(maliciousInput);

        // Assert that HTML/script content is removed
        Assert.IsFalse(sanitized.Contains("<"));
        Assert.IsFalse(sanitized.Contains(">"));
        Assert.IsFalse(sanitized.Contains("script"));
        Assert.IsFalse(sanitized.Contains("onerror"));
    }
}