namespace Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _client;

        public SendGridEmailService(IOptions<SendGridEmailOptions> emailOptions)
        {
            _client = new SendGridClient(emailOptions.Value.ApiKey);
        }

        public async Task SendEmailAsync(
            string source,
            IEnumerable<string> destinations,
            string subject,
            string htmlBody,
            string textBody = default,
            CancellationToken cancellationToken = default)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress(source),
                Subject = subject,
                PlainTextContent = textBody,
                HtmlContent = htmlBody
            };
            foreach (var destination in destinations)
            {
                msg.AddTo(new EmailAddress(destination));
            }

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            await _client.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);
        }
    }
}
