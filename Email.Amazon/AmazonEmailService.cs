namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Common.Services;
    using JetBrains.Annotations;
    using static System.String;
    using static System.Text.Encoding;

    /// <inheritdoc />
    [PublicAPI]
    public class AmazonEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;

        /// <summary>Initializes a new instance of the <see cref="AmazonEmailService"/> class.</summary>
        /// <param name="amazonSimpleEmailService">The amazon simple email service.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="amazonSimpleEmailService"/> is <see langword="null"/>
        /// or
        /// <paramref name="name"/> is <see langword="null"/>.</exception>
        public AmazonEmailService(
            IAmazonSimpleEmailService amazonSimpleEmailService,
            string name = nameof(AmazonEmailService))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _amazonSimpleEmailService = amazonSimpleEmailService ?? throw new ArgumentNullException(nameof(amazonSimpleEmailService));
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
            if (IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinations == default)
            {
                throw new ArgumentNullException(nameof(destinations));
            }

            var recipients = destinations.ToList();
            if (recipients.Any())
            {
                throw new ArgumentException("No recipients.", nameof(destinations));
            }

            if (IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (IsNullOrWhiteSpace(htmlBody))
            {
                throw new ArgumentNullException(nameof(htmlBody));
            }

            async Task SendEmail()
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
                if (!IsNullOrWhiteSpace(textBody))
                {
                    sendEmailRequest.Message.Body.Text = new Content
                    {
                        Charset = UTF8.HeaderName,
                        Data = textBody
                    };
                }

                await _amazonSimpleEmailService
                    .SendEmailAsync(sendEmailRequest, cancellationToken)
                    .ConfigureAwait(false);
            }

            return SendEmail();
        }
    }
}
