using MailKit.Security;
using MailKit.Net.Smtp; // Utilisez SmtpClient de MailKit
using MimeKit;

namespace CinephoriaServer.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            // Créer le message MIME
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Patrice Simo", smtpSettings["Username"]));
            message.To.Add(new MailboxAddress("Destinataire", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Utiliser SmtpClient de MailKit
            using (var client = new SmtpClient())
            {
                try
                {
                    // Convertir SecureSocketOptions
                    var secureSocketOptions = smtpSettings["SecureSocketOptions"] switch
                    {
                        "StartTls" => SecureSocketOptions.StartTls,
                        "SslOnConnect" => SecureSocketOptions.SslOnConnect,
                        _ => SecureSocketOptions.Auto
                    };

                    // Connexion au serveur SMTP
                    await client.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), secureSocketOptions);

                    // Authentification
                    await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);

                    // Envoi de l'email
                    await client.SendAsync(message);

                    // Déconnexion
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {ToEmail}", toEmail);
                    throw; // Relancer l'exception pour la gestion des erreurs dans l'appelant
                }
            }
        }
    }
}