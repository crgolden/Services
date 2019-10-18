namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using static System.DateTime;
    using static Common.EventId;
    using EventId = Microsoft.Extensions.Logging.EventId;

    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            ISendGridClient? sendGridClient,
            ILogger<SendGridEmailService>? logger)
        {
            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(
            string? source,
            IEnumerable<string>? destinations,
            string? subject,
            string? htmlBody,
            string? textBody = default,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinations == null)
            {
                throw new ArgumentNullException(nameof(destinations));
            }

            var recipients = destinations.ToArray();
            if (recipients.Any())
            {
                throw new ArgumentException("No recipients.", nameof(destinations));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrEmpty(htmlBody))
            {
                throw new ArgumentNullException(nameof(htmlBody));
            }

            var msg = new SendGridMessage
            {
                From = new EmailAddress(source),
                Subject = subject,
                PlainTextContent = textBody,
                HtmlContent = htmlBody
            };
            foreach (var recipient in recipients)
            {
                msg.AddTo(new EmailAddress(recipient));
            }

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            await _sendGridClient.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation(
                eventId: new EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email {@Body} sent at {@Time}",
                args: new object[] { htmlBody, UtcNow });
        }
    }
}
