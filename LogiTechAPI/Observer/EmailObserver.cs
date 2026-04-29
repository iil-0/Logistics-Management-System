using System.Net;
using System.Net.Mail;
using LogiTechAPI.Settings;

namespace LogiTechAPI.Observer
{
    public class EmailObserver : IShipmentObserver
    {
        private readonly string _recipientEmail;
        private readonly string _trackingNo;
        private readonly EmailSettings _settings;

        public EmailObserver(string recipientEmail, string trackingNo, EmailSettings settings)
        {
            _recipientEmail = recipientEmail;
            _trackingNo = trackingNo;
            _settings = settings;
        }

        public void Update(string message, string status)
        {
            _ = Task.Run(() => SendEmail(message, status));
        }

        private void SendEmail(string message, string status)
        {
            try
            {
                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, "LogiTech"),
                    Subject = $"Shipment Update - {_trackingNo}: {status}",
                    Body = $"Hello,\n\nYour shipment {_trackingNo} status has changed.\n\nStatus: {status}\nDetails: {message}\n\nThanks for choosing LogiTech.",
                    IsBodyHtml = false
                };
                mail.To.Add(_recipientEmail);

                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailObserver] Failed to send email: {ex.Message}");
            }
        }
    }
}
