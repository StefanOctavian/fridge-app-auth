namespace Auth.Constants;

public class MailTemplates
{
    private static string VerificationTemplate { get; } = ReadTemplate("VerificationMailTemplate.html");

    private static string ReadTemplate(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", name);
        return File.ReadAllText(path);
    }

    public static string VerificationMail(string name, string token) =>
        VerificationTemplate.Replace("{origin}", "example.com")
            .Replace("{name}", name).Replace("{token}", token);
}