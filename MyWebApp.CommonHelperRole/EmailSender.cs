using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace MyWebApp.CommonHelperRole
{
    // This class created for Inherit IEmailSender for service
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var toEmail = new MimeMessage();
            //From
            toEmail.From.Add(MailboxAddress.Parse("MyEmail"));
            //To
            toEmail.To.Add(MailboxAddress.Parse(email));
            //Subject
            toEmail.Subject = subject;
            //Body
            toEmail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //Send Email
            using(var emailClient = new SmtpClient())
            {
                //smtp.gmail.com-host, 587-port, MailKit.Security.SecureSocketOptions.StartTls - Security
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                // Set UserId and Password
                emailClient.Authenticate("UserId", "Password");
                emailClient.SendAsync(toEmail);
                //Disconnect
                emailClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}
