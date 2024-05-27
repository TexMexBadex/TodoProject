using MailKit.Net.Smtp;
using MimeKit;
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
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("Your App Name", _configuration["EmailSettings:From"]));
      emailMessage.To.Add(new MailboxAddress("", to));
      emailMessage.Subject = subject;
      emailMessage.Body = new TextPart("plain") { Text = body };

      using var client = new SmtpClient();
      try
      {
        await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), bool.Parse(_configuration["EmailSettings:EnableSsl"]));
        await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
        await client.SendAsync(emailMessage);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while sending the email.");
        throw;
      }
      finally
      {
        await client.DisconnectAsync(true);
      }
    }
  }
}