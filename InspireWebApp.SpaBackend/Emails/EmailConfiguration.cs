namespace InspireWebApp.SpaBackend.Emails;

public class EmailConfiguration
{
    public string DefaultFromName { get; set; } = Program.ProjectName;
    public string DefaultFromEmail { get; set; } = "inspire-web-app@example.com";

    public string? SmtpServerHost { get; set; }
    public int? SmtpServerPort { get; set; }

    public string? SmtpLogin { get; set; }
    public string? SmtpPassword { get; set; }
}