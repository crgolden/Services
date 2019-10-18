namespace Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using static System.DateTime;
    using static System.Text.Encoding;
    using static System.Threading.Tasks.Task;
    using static Common.EventId;
    using static Microsoft.Azure.ServiceBus.TransportType;
    using EventId = Microsoft.Extensions.Logging.EventId;

    public class EmailQueueClientService : QueueClient, IHostedService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailQueueClientService> _logger;

        public EmailQueueClientService(
            IOptions<EmailQueueClientOptions>? emailQueueClientOptions,
            IEmailService? emailService,
            ILogger<EmailQueueClientService>? logger,
            IHostApplicationLifetime? hostApplicationLifetime)
            : base(emailQueueClientOptions?.Value == default
                  ? throw new ArgumentNullException(nameof(emailQueueClientOptions))
                  : new ServiceBusConnectionStringBuilder(
                      endpoint: emailQueueClientOptions.Value.Endpoint,
                      entityPath: emailQueueClientOptions.Value.EmailQueueName,
                      sharedAccessKeyName: emailQueueClientOptions.Value.SharedAccessKeyName,
                      sharedAccessKey: emailQueueClientOptions.Value.PrimaryKey,
                      transportType: Amqp))
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (hostApplicationLifetime == default)
            {
                throw new ArgumentNullException(nameof(hostApplicationLifetime));
            }

            hostApplicationLifetime?.ApplicationStarted.Register(OnStarted);
            hostApplicationLifetime?.ApplicationStopping.Register(OnStopping);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterMessageHandler(
                handler: ProcessMessagesAsync,
                messageHandlerOptions: new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    AutoComplete = false
                });
            return CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return CloseAsync();
        }

        private void OnStarted()
        {
            _logger.LogInformation(
                eventId: new EventId((int)QueueClientStart, $"{QueueClientStart}"),
                message: "Email queue client starting at {Time}",
                args: new object[] { UtcNow });
        }

        private void OnStopping()
        {
            _logger.LogInformation(
                eventId: new EventId((int)QueueClientStop, $"{QueueClientStop}"),
                message: "Email queue client stopping at {Time}",
                args: new object[] { UtcNow });
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken cancellationToken)
        {
            if (!message.UserProperties.ContainsKey("source") || !(message.UserProperties["source"] is string source) ||
                !message.UserProperties.ContainsKey("destinations") || !(message.UserProperties["destinations"] is string[] destinations) ||
                !message.UserProperties.ContainsKey("subject") || !(message.UserProperties["subject"] is string subject))
            {
                throw new ArgumentException("Invalid User Properties", nameof(message));
            }

            _logger.LogInformation(
                eventId: new EventId((int)QueueClientProcessing, $"{QueueClientProcessing}"),
                message: "Email queue client processing at {Time}",
                args: new object[] { UtcNow });
            await _emailService.SendEmailAsync(
                source: source,
                destinations: destinations,
                subject: subject,
                htmlBody: UTF8.GetString(message.Body),
                textBody: default,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            await CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
            _logger.LogInformation(
                eventId: new EventId((int)QueueClientCompleted, $"{QueueClientCompleted}"),
                message: "Email queue client completed at {Time}",
                args: new object[] { UtcNow });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(
                eventId: new EventId((int)QueueClientError, $"{QueueClientError}"),
                exception: exceptionReceivedEventArgs.Exception,
                message: "Email queue client received exception {Context} at {Time}",
                args: new object[] { exceptionReceivedEventArgs.ExceptionReceivedContext, UtcNow });
            return CompletedTask;
        }
    }
}
