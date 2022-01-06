using System.Net;
using System.Net.Mail;

namespace SecondSoft.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toMail, string subject, string msg, AlternateView alternateView = null);

        IConfigurationSection MailSettings();
    }

    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MailService> logger;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string toMail, string subject, string msg, AlternateView alternateView = null)
        {
            var mailsettings = MailSettings();

            var mail = new MailMessage();
            mail.From = new MailAddress(mailsettings.GetSection("From").Value);
            mail.Subject = subject ??= mailsettings.GetSection("ServiceName").Value;

            mail.IsBodyHtml = true;
            mail.To.Add(toMail);
            if (alternateView != null)
            {
                mail.AlternateViews.Add(alternateView);
            }
            else
            {
                var body = $" msg : {msg}";
                mail.Body = body;
            }

            await Client(mailsettings).SendMailAsync(mail);
        }

        public IConfigurationSection MailSettings()
        {
            return configuration.GetSection("Mail");
        }

        private SmtpClient Client(IConfiguration mailsettings)
        {
            var apiKey = Secret_Service_ExternalPack.Dataprotection.MailApiAndSecretKeyDec(mailsettings.GetSection("ApiKey").Value);
            var secretKey = Secret_Service_ExternalPack.Dataprotection.MailApiAndSecretKeyDec(mailsettings.GetSection("secretKey").Value);

            var client = new SmtpClient(mailsettings.GetSection("MailClient").Value, int.Parse(mailsettings.GetSection("Port").Value));
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(apiKey, secretKey);

            return client;
        }
    }
}
