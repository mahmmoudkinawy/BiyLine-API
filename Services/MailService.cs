namespace BiyLineApi.Services;
public sealed class MailService : IMailService
{
    private readonly SmtpSettings _smtpSettings;

    public MailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public string LoadEmailTemplate(string templateName)
    {
        var templateSource = File.ReadAllText($"wwwroot/EmailTemplates/{templateName}");
        return templateSource;
    }

    public string RenderEmailTemplate(string templateName, object data)
    {
        var compiledTemplate = Handlebars.Compile(templateName);
        return compiledTemplate(data);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
        }
        catch (Exception)
        {
            await Console.Out.WriteLineAsync(" --> Failed to send the mail.");
        }
    }
}
