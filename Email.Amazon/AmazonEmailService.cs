namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Common;
    using Microsoft.Extensions.Logging;
    using static System.DateTime;
    using static System.String;
    using static System.Text.Encoding;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
    public class AmazonEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private readonly ILogger<AmazonEmailService> _logger;

        public AmazonEmailService(
            IAmazonSimpleEmailService? amazonSimpleEmailService,
            ILogger<AmazonEmailService>? logger)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService ?? throw new ArgumentNullException(nameof(amazonSimpleEmailService));
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

            var recipients = destinations.ToList();
            if (recipients.Any())
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
            List<string> recipients,
            string subject,
            string htmlBody,
            string? textBody,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var sendEmailRequest = new SendEmailRequest
            {
                Source = source,
                Destination = new Destination
                {
                    ToAddresses = recipients
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = UTF8.HeaderName,
                            Data = htmlBody
                        }
                    }
                }
            };
            if (!IsNullOrEmpty(textBody))
            {
                sendEmailRequest.Message.Body.Text = new Content
                {
                    Charset = UTF8.HeaderName,
                    Data = textBody
                };
            }

            var sendEmailResponse = await _amazonSimpleEmailService
                .SendEmailAsync(sendEmailRequest, cancellationToken)
                .ConfigureAwait(false);
            _logger.Log(
                logLevel: logLevel,
                eventId: new EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email request {@Request} sent with response {@Response} at {@Time}",
                args: new object[] { sendEmailRequest, sendEmailResponse, UtcNow });
        }
    }
}
