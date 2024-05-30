using System.Net;
using System.Net.Mail;
using ReminderMicroservice.Services.Interfaces;

namespace ReminderMicroservice.Services
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
      _configuration = configuration;
      _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
      var mail = _configuration["EmailSettings:Mail"];
      var pw = _configuration["EmailSettings:Pw"];

      try
      {
        var client = new SmtpClient("smtp-mail.outlook.com", 587)
        {
          EnableSsl = true,
          Credentials = new NetworkCredential(mail, pw)
        };

        await client.SendMailAsync(new MailMessage(from: mail!, to: to, subject, body));

      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong when trying to send email. Exception: {Ex}", ex);
      }


    }
  }
}