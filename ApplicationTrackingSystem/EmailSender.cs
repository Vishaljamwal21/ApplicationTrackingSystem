using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "My Email Display Name")
                };
                mailMessage.To.Add(toEmail);

                if (!string.IsNullOrEmpty(_emailSettings.CcEmail))
                {
                    mailMessage.CC.Add(_emailSettings.CcEmail);
                }

                mailMessage.Subject = "ApplicationTrackingSystem: " + subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error while sending email to {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to {Email}", email);
                throw;
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                await Execute(email, subject, htmlMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendEmailAsync for {Email}", email);
                throw;
            }
        }
    }
}
