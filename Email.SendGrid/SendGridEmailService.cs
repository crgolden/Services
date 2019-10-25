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
    using static System.String;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public Task SendEmailAsync(
            string? source,
            IEnumerable<string>? destinations,
            string? subject,
            string? htmlBody,
            string? textBody = default,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinations == null)
            {
                throw new ArgumentNullException(nameof(destinations));
            }

            var recipients = destinations.ToArray();
            if (!recipients.Any())
            {
                throw new ArgumentException("No recipients.", nameof(destinations));
            }

            if (IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (IsNullOrEmpty(htmlBody))
            {
                throw new ArgumentNullException(nameof(htmlBody));
            }

            return SendEmail(source, recipients, subject, htmlBody, textBody, logLevel, cancellationToken);
        }

        private async Task SendEmail(
            string source,
            IEnumerable<string> recipients,
            string? subject,
            string? htmlBody,
            string? textBody,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress(source),
                Subject = subject,
                PlainTextContent = textBody,
                HtmlContent = htmlBody
            };
            foreach (var recipient in recipients)
            {
                message.AddTo(new EmailAddress(recipient));
            }

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            message.SetClickTracking(false, false);
            var response = await _sendGridClient
                .SendEmailAsync(message, cancellationToken)
                .ConfigureAwait(false);
            _logger.Log(
                logLevel: logLevel,
                eventId: new EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email message {@Message} sent with response {@Response} at {@Time}",
                args: new object[] { message, response, UtcNow });
        }
    }
}
