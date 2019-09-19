namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using static Common.EventIds;

    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            IOptions<SendGridEmailOptions> sendGridEmailOptions,
            ILogger<SendGridEmailService> logger)
        {
            _client = new SendGridClient(sendGridEmailOptions.Value.ApiKey);
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
            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)EmailSent, $"{EmailSent}"),
                message: "Email {@Body} sent at {@Time}",
                args: new object[] { htmlBody, DateTime.UtcNow });
        }
    }
}
