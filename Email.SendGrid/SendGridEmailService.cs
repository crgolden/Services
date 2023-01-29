namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Services;
    using JetBrains.Annotations;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using static System.String;

    /// <inheritdoc />
    [PublicAPI]
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;

        /// <summary>Initializes a new instance of the <see cref="SendGridEmailService"/> class.</summary>
        /// <param name="sendGridClient">The send grid client.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sendGridClient"/> is <see langword="null"/>
        /// or
        /// <paramref name="name"/> is <see langword="null"/>.</exception>
        public SendGridEmailService(
            ISendGridClient sendGridClient,
            string name = nameof(SendGridEmailService))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Task SendEmailAsync(
            string source,
            IEnumerable<string> destinations,
            string subject,
            string htmlBody,
            string textBody = default,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinations == default)
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

            async Task SendEmail()
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
                await _sendGridClient
                    .SendEmailAsync(message, cancellationToken)
                    .ConfigureAwait(false);
            }

            return SendEmail();
        }
    }
}
