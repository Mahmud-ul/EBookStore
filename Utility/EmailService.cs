using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EBookStore.Utility
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<bool> SendEmailAsync(List<string> toEmail, string subject, string body)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(_settings.From);
                foreach (var email in toEmail)
                {
                    msg.To.Add(new MailAddress(email));
                }

                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = false;

                using (var smtp = new SmtpClient(_settings.Host, _settings.Port))
                {
                    smtp.EnableSsl = _settings.EnableSsl;
                    smtp.Credentials = new NetworkCredential(_settings.From, _settings.AppPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await smtp.SendMailAsync(msg);
                }

                return true; // ✅ Success
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // ❌ Failure
            }
        }
    }
}
