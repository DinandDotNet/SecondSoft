using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SecondSoft.Models;
using SecondSoft.Services;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;

namespace SecondSoft.Helpers
{
    public interface IEmails
    {
        Task ContactEmail(Contact contact, string request = "");
    }

    public class Emails : IEmails
    {
        private readonly IMailService mailService;
        private readonly ILogger<IEmails> logger;

        public Emails(IMailService mailService, ILogger<IEmails> logger)
        {
            this.mailService = mailService;
            this.logger = logger;
        }

        public async Task ContactEmail(Contact contact, string request = "")
        {
            var subject = request?? "Bedankt voor uw aanvraag";
            var body = await ThankyouBody(contact, subject);
            var contactBody = await ContactBody(contact, subject);

            await CreateAndSendEmail(contact.Email, subject, body);
            await CreateAndSendEmail("dinand.dotnet@gmail.com", subject, contactBody);
        }

        /// <summary>
        /// Generic body for callback messages
        /// </summary>
        /// <param name="user">Id user object</param>
        /// <param name="callbackUrl"></param>
        /// <param name="bodySubject"></param>
        /// <returns>Complete messeage as string</returns>
        private async Task<string> ThankyouBody(Contact contact, string bodySubject, string service = "our Service")
        {
            try
            {
                var body = $"Dear {contact.Name},<br/>";
                body += $"This confirmation is send to you, because you did a {bodySubject} request from {service}. If you did not request this, please ignore this email.<br/><br/>";
                body += $"I will send you asap an email with a link to the portfolio <br/> <br/>";
                body += "Thank you for your attention. <br/>Regards,<br/>";
                body += "Dinand van Hartingsveldt";
                return body;
            }
            catch (Exception ex)
            {
                logger.LogError("ContactBody creation error", ex);
            }
            return string.Empty;
        }
        private async Task<string> ContactBody(Contact contact, string bodySubject, string service = "our Service")
        {
            try
            {
                var body = $"On {DateTime.Now.ToUniversalTime()} a request has come in from {contact.Name} of company {contact.CompanyName},<br/>";
                body += $"Email {contact.Email}. ";
                if (!string.IsNullOrEmpty(contact.Phone))
                {
                    body += $"Phone : {contact.Phone}. <br/>";
                }
                if (!string.IsNullOrEmpty(contact.Message))
                {
                    body += $"Message : {contact.Message}. <br/>";
                }
                
                body += "Dinand van Hartingsveldt";
                return body;
            }
            catch (Exception ex)
            {
                logger.LogError("ContactBody creation error", ex);
            }
            return string.Empty;
        }

            /// <summary>
            /// Create and send out the email
            /// </summary>
            /// <param name="user"></param>
            /// <param name="subject"></param>
            /// <param name="body"></param>
            /// <returns></returns>
            private async Task CreateAndSendEmail(string emailaddres, string subject, string body)
        {
            try
            {
                var av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                await mailService.SendEmailAsync(emailaddres, subject, body, av);
                logger.LogInformation($"Send {subject} email to user {emailaddres}");
            }
            catch (Exception ex)
            {
                logger.LogError("Mail error", ex);
            }
        }

    }
}