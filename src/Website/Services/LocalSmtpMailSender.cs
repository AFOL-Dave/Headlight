using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Headlight.Services
{
    public class LocalSmtpMailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("KLUG Administrator (Test)","test@local"));
            message.To.Add(new MailboxAddress("Test User", email));
            message.Subject = subject;

            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            using(SmtpClient client = new SmtpClient())
            {
                client.Connect("localhost", 25, false);
                client.Send(message);
                client.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}