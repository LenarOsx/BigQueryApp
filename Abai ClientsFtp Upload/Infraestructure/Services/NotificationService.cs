using Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly AppConfiguration _config;

        private readonly SmtpClient? _smtpClient;

        public NotificationService(ILogger<NotificationService> logger, AppConfiguration config)
        {
            _logger = logger;
            _config = config;
            _smtpClient = CreateClient();
        }

        private SmtpClient? CreateClient()
        {
            try
            {
                if (_smtpClient != null)
                    return _smtpClient;

                var smtp = new SmtpClient(_config.SmtpConfig.Host, _config.SmtpConfig.Port);
                smtp.EnableSsl = _config.SmtpConfig.Ssl;
                smtp.Credentials = new System.Net.NetworkCredential(_config.SmtpConfig.User, _config.SmtpConfig.Password);

                return smtp;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateClient error");
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public bool SendNotification(string message, ReportMailConfiguration campaignConfig,out string errorMailSmtp)
        {
            errorMailSmtp = string.Empty;
            
            //var emailMessage = new MailMessage(_config.Notification.User ?? "no-reply@abaigroup.com", "desarrollo@abaigroup.com");
            var defaultMailAddress = "lenar.oseguera.ext@abaigroup.com";
            var emailMessage = new MailMessage();
            foreach (var mailTo in campaignConfig.MailsTo.Where(m => m.Enabled).ToList())
            {
                emailMessage.To.Add(new MailAddress(mailTo.MailAddress));
            }
            if (!emailMessage.To.Any())
            {
                emailMessage.To.Add(new MailAddress(defaultMailAddress));

            }
            emailMessage.From = new MailAddress(campaignConfig.MailFrom.MailAddress);
            emailMessage.Subject = campaignConfig.Subject;
            emailMessage.Body = message;
            emailMessage.IsBodyHtml = true;
            bool emailSent = false;

            try
            {
                _smtpClient?.Send(emailMessage);
                emailSent = true;
                _logger.LogInformation($"Email notification sent. {campaignConfig.Subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email notification: {ex}");
                errorMailSmtp = $"Error sending email notification: {ex}";
            }
            finally
            {
                _logger.LogInformation(emailSent ?
                    "Email notification has been sent successfully" :
                    "Email notification could not be sent");
                errorMailSmtp = emailSent ? string.Empty:"Email notification could not be sent";
            }
            return emailSent;
        }
    }
}
