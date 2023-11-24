namespace BiyLineApi.Services;
public interface IMailService
{
    Task SendEmailAsync(string to, string subject, string body);
    string LoadEmailTemplate(string templateName);
    string RenderEmailTemplate(string templateName, object data);
}
