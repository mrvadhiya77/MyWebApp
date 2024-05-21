using Microsoft.AspNetCore.Identity.UI.Services;

namespace MyWebApp.CommonHelperRole
{
    // This class created for Inherit IEmailSender for service
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
