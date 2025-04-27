namespace Auth.Configurations;

/// <summary>
/// This class is used to configure the mail client for sending emails.
/// </summary>
public class MailConfiguration
{
    /// <summary>
    /// MailEnable specifies if the mail client should send the mail or simply ignore it such that no mail is send while testing the backend.
    /// </summary>
    public bool MailEnable { get; set; }
    /// <summary>
    /// The mail server where the client needs to connect.
    /// </summary>
    public string MailHost { get; set; } = null!;
    /// <summary>
    /// The SMTP port to which to connect.
    /// </summary>
    public ushort MailPort { get; set; }
    /// <summary>
    /// The email address that sends the emails.
    /// </summary>
    public string MailAddress { get; set; } = null!;
    /// <summary>
    /// The email user credential.
    /// </summary>
    public string MailUser { get; set; } = null!;
    /// <summary>
    /// The email password credential.
    /// </summary>
    public string MailPassword { get; set; } = null!;
}
