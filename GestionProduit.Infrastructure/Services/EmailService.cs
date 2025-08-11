using GestionProduit.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GestionProduit.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // M�thode principale qui envoie un email avec un fromEmail optionnel
        public async Task SendEmailAsync(string toEmail, string subject, string body, string? fromEmail = null)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("L'adresse email destinataire ne peut pas �tre vide.", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Le sujet de l'email ne peut pas �tre vide.", nameof(subject));

            if (body == null)
                body = string.Empty;

            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpHost = emailSettings["SmtpHost"];
            if (string.IsNullOrWhiteSpace(smtpHost))
                throw new InvalidOperationException("Le param�tre 'SmtpHost' est manquant dans la configuration.");

            if (!int.TryParse(emailSettings["SmtpPort"], out int smtpPort))
                smtpPort = 587; // port par d�faut

            var smtpUser = emailSettings["SmtpUser"];
            var smtpPass = emailSettings["SmtpPass"];
            var adminEmail = emailSettings["AdminEmail"];

            if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                throw new InvalidOperationException("Les identifiants SMTP sont manquants dans la configuration.");

            var fromAddress = fromEmail;
            if (string.IsNullOrWhiteSpace(fromAddress))
                fromAddress = adminEmail ?? smtpUser;

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;
                client.Timeout = 10000; // timeout 10 sec

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromAddress);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);
                }
            }
        }

        // Impl�mentation demand�e par IEmailService
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Appelle la m�thode avec 4 param�tres en passant fromEmail = null
            await SendEmailAsync(toEmail, subject, body, null);
        }
    }
}
