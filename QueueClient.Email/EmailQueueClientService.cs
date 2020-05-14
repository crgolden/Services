namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using JetBrains.Annotations;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using static System.Text.Encoding;
    using static System.Threading.Tasks.Task;

    /// <inheritdoc cref="IHostedService" />
    [PublicAPI]
    public class EmailQueueClientService : IHostedService
    {
        private readonly IEmailService _emailService;
        private readonly IReceiverClient _receiverClient;
        private readonly ILogger<EmailQueueClientService> _logger;

        /// <summary>Initializes a new instance of the <see cref="EmailQueueClientService"/> class.</summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="receiverClient">The queue client.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException"><paramref name="emailService"/> is <see langword="null"/>
        /// or
        /// <paramref name="receiverClient"/> is <see langword="null"/>
        /// or
        /// <paramref name="logger"/> is <see langword="null"/>.</exception>
        public EmailQueueClientService(IEmailService emailService, IReceiverClient receiverClient, ILogger<EmailQueueClientService> logger)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _receiverClient = receiverClient ?? throw new ArgumentNullException(nameof(receiverClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _receiverClient.RegisterMessageHandler(
                handler: async (message, token) =>
                {
                    if (!message.UserProperties.ContainsKey("source") || !(message.UserProperties["source"] is string source) ||
                        !message.UserProperties.ContainsKey("destinations") || !(message.UserProperties["destinations"] is IEnumerable<string> destinations) ||
                        !message.UserProperties.ContainsKey("subject") || !(message.UserProperties["subject"] is string subject))
                    {
                        throw new ArgumentException("Invalid User Properties", nameof(message));
                    }

                    await _emailService.SendEmailAsync(
                        source: source,
                        destinations: destinations,
                        subject: subject,
                        htmlBody: UTF8.GetString(message.Body),
                        textBody: default,
                        cancellationToken: token).ConfigureAwait(false);
                    await _receiverClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                },
                exceptionReceivedHandler: args =>
                {
                    _logger.LogError(
                        eventId: new EventId(1, nameof(Exception)),
                        exception: args.Exception,
                        message: "{@Context}",
                        args: new object[] { args.ExceptionReceivedContext });
                    return CompletedTask;
                });
            return CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => _receiverClient.CloseAsync();
    }
}
