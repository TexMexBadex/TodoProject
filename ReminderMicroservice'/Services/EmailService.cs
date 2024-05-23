using MailKit.Net.Smtp;
using MimeKit;
using ReminderMicroservice_.Services.Interfaces;

namespace ReminderMicroservice_.Services
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("Task Reminder", _configuration["EmailSettings:From"]));
      emailMessage.To.Add(new MailboxAddress("", to));
      emailMessage.Subject = subject;
      emailMessage.Body = new TextPart("plain") { Text = body };

      using var client = new SmtpClient();
      await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), bool.Parse(_configuration["EmailSettings:EnableSsl"]));
      await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
      await client.SendAsync(emailMessage);
      await client.DisconnectAsync(true);
    }
  }
}