// Concrete Observer — durum değişikliklerini SMTP üzerinden e-posta olarak gönderir.
// Subject (ObserverRegistry) bu observer'ı Update() ile çağırır; iş arka plan
// thread'inde yapılır ki HTTP cevabı SMTP'yi beklemesin.
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

        // Subject çağırır → fire-and-forget arka plan göndermi başlatır
        public void Update(string message, string status)
        {
            Console.WriteLine($"[EmailObserver] Update triggered: to={_recipientEmail}, tracking={_trackingNo}, status={status}");
            _ = Task.Run(() => SendEmail(message, status));
        }

        private void SendEmail(string message, string status)
        {
            try
            {
                Console.WriteLine($"[EmailObserver] Connecting to SMTP {_settings.SmtpHost}:{_settings.SmtpPort} as {_settings.SenderEmail}");
                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                    EnableSsl = true                        // Gmail için STARTTLS zorunlu
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
                Console.WriteLine($"[EmailObserver] SUCCESS: email sent to {_recipientEmail} for {_trackingNo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailObserver] FAILED: to={_recipientEmail}, error={ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[EmailObserver]   inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
        }
    }
}
