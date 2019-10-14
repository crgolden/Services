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
    using static Common.EventId;

    public class AmazonEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private readonly ILogger<AmazonEmailService> _logger;

        public AmazonEmailService(
            IAmazonSimpleEmailService amazonSimpleEmailService,
            ILogger<AmazonEmailService> logger)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string source,
            IEnumerable<string> destinations,
            string subject,
            string htmlBody,
            string textBody = default,
            CancellationToken cancellationToken = default)
        {
            var sendRequest = new SendEmailRequest
            {
                Source = source,
                Destination = new Destination
                {
                    ToAddresses = destinations.ToList()
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = System.Text.Encoding.UTF8.HeaderName,
                            Data = htmlBody
                        }
                    }
                }
            };
            if (!string.IsNullOrEmpty(textBody) && !string.IsNullOrWhiteSpace(textBody))
            {
                sendRequest.Message.Body.Text = new Content
                {
                    Charset = System.Text.Encoding.UTF8.HeaderName,
                    Data = textBody
                };
            }

            await _amazonSimpleEmailService.SendEmailAsync(sendRequest, cancellationToken).ConfigureAwait(false);
            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new Microsoft.Extensions.Logging.EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email {@Body} sent at {@Time}",
                args: new object[] { htmlBody, DateTime.UtcNow });
        }
    }
}
