using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Headlight.Services
{
    public class SendGridMailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}