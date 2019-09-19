namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Server;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using static Common.EventIds;

    public class HangfireJobService : IHostedService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IEnumerable<HangfireJobDetail> _jobDetails;
        private readonly ILogger<HangfireJobService> _logger;

        public HangfireJobService(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IEnumerable<HangfireJobDetail> jobDetails,
            ILogger<HangfireJobService> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _jobDetails = jobDetails;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var jobDetail in _jobDetails)
            {
                _recurringJobManager.AddOrUpdate(
                    recurringJobId: jobDetail.Id,
                    job: jobDetail.Job,
                    cronExpression: jobDetail.CronExpression,
                    options: jobDetail.Options);
            }

            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStarted, $"{HostedServiceStarted}"),
                message: "Hangfire Job Service Started at {@Time}",
                args: new object[] { DateTime.UtcNow });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var jobDetail in _jobDetails)
            {
                _recurringJobManager.RemoveIfExists(jobDetail.Id);
            }

            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStopped, $"{HostedServiceStopped}"),
                message: "Hangfire Job Service Stopped at {@Time}",
                args: new object[] { DateTime.UtcNow });
            return Task.CompletedTask;
        }

        public static DateTime? GetCompareDate(PerformContext context, string methodName)
        {
            return long.TryParse(context.BackgroundJob.Id, out var currentJobId)
                ? JobStorage.Current
                    ?.GetMonitoringApi()
                    ?.SucceededJobs(0, (int)currentJobId)
                    ?.LastOrDefault(x => x.Value?.Job?.Method?.Name == methodName).Value?.SucceededAt
                : null;
        }
    }
}
