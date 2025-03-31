using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Services.Interfaces;

namespace RodeFortune.BLL.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
            
            _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            _fromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL");
            _fromName = Environment.GetEnvironmentVariable("FROM_NAME") ?? "RodeFortune";
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Електронний лист надіслано успішно до {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при надсиланні електронного листа до {Email}", to);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
        {
            string subject = "Відновлення пароля - RodeFortune";
            string message = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #6a3cb5; color: white; padding: 10px 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f8f9fa; }}
                        .button {{ display: inline-block; background-color: #6a3cb5; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>RodeFortune</h2>
                        </div>
                        <div class='content'>
                            <h3>Відновлення пароля</h3>
                            <p>Ви отримали цей лист, тому що запросили скидання пароля для вашого облікового запису.</p>
                            <p>Щоб встановити новий пароль, будь ласка, натисніть на посилання нижче:</p>
                            <p><a href='{callbackUrl}' class='button'>Скинути пароль</a></p>
                            <p>Якщо ви не запитували скидання пароля, ігноруйте цей лист.</p>
                            <p>Посилання дійсне протягом 24 годин.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }
    }
}