namespace Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Common;
    using Microsoft.Extensions.Options;

    public class AmazonEmailService : IEmailService
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;

        public AmazonEmailService(IOptions<AmazonEmailOptions> emailOptions)
        {
            _accessKeyId = emailOptions.Value.AccessKeyId;
            _secretAccessKey = emailOptions.Value.SecretAccessKey;
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
                    ToAddresses = new List<string>(destinations)
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody
                        }
                    }
                }
            };
            if (!string.IsNullOrEmpty(textBody) && !string.IsNullOrWhiteSpace(textBody))
            {
                sendRequest.Message.Body.Text = new Content
                {
                    Charset = "UTF-8",
                    Data = textBody
                };
            }

            using (var client = new AmazonSimpleEmailServiceClient(_accessKeyId, _secretAccessKey))
            {
                await client.SendEmailAsync(sendRequest, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
