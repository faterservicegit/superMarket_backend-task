using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Settings;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        public EmailSender(IWritableOptions<SiteSettings> writableLocations)
        {
            _writableLocations = writableLocations;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var Client = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName = _writableLocations.Value.EmailSettings.Username,
                    Password = _writableLocations.Value.EmailSettings.Password,
                };

                Client.Credentials = Credential;
                Client.Host = _writableLocations.Value.EmailSettings.Host;
                Client.Port = _writableLocations.Value.EmailSettings.Port;
                Client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress(_writableLocations.Value.EmailSettings.Email);
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = message;

                    Client.Send(emailMessage);
                };
                await Task.CompletedTask;
            }
        }
    }
}
