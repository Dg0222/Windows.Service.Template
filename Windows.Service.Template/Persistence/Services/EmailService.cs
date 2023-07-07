using System.Net.Mail;
using System.Text;
using Windows.Service.Template.Application.Common.Interfaces;

namespace Windows.Service.Template.Persistence.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private SmtpClient GetSmtpClient()
    {

        return new SmtpClient()
        {
            Host = "localhost",
            Port = 25,
        };

    }

    public void SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            using var smtpClient = GetSmtpClient();

            var from = new MailAddress("test@localhost.com");

            var to = new MailAddress(email);

            var message = new MailMessage(from, to);
            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;

            message.Body = body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            _logger.LogError("SMTP error: {ExInnerException}", ex.InnerException);
            throw;
        }
    }

    //private string PopulateBody(string message)
    //{
    //    var webRoot = _webHostEnvironment.WebRootPath;

    //    var body = "";

    //    using (var reader = new StreamReader(webRoot + "/EmailTemplates/TestEmailTemplate.html"))
    //    {
    //        body = reader.ReadToEnd();
    //    }

    //    body = body?.Replace("{MESSAGE}", message);

    //    return body;
    //}
}

