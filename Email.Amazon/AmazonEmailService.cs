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
    using static System.Text.Encoding;
    using static Common.EventId;
    using EventId = Microsoft.Extensions.Logging.EventId;

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

            var recipients = destinations.ToList();
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

            var sendRequest = new SendEmailRequest
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
            if (!string.IsNullOrEmpty(textBody))
            {
                sendRequest.Message.Body.Text = new Content
                {
                    Charset = UTF8.HeaderName,
                    Data = textBody
                };
            }

            await _amazonSimpleEmailService.SendEmailAsync(sendRequest, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation(
                eventId: new EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email {@Body} sent at {@Time}",
                args: new object[] { htmlBody, UtcNow });
        }
    }
}
