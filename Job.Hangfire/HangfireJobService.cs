namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Server;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using static Common.EventId;

    public class HangfireJobService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly IEnumerable<HangfireJobDetail> _jobDetails;
        private readonly ILogger<HangfireJobService> _logger;

        public HangfireJobService(
            IServiceProvider services,
            IEnumerable<HangfireJobDetail> jobDetails,
            IOptions<HangfireJobOptions> hangfireJobOptions,
            ILogger<HangfireJobService> logger)
        {
            GlobalJobFilters.Filters.Add(new ApplyStateFilter(hangfireJobOptions));
            _services = services;
            _jobDetails = jobDetails;
            _logger = logger;
        }

        public static DateTime? GetCompareDate(PerformContext context, string methodName)
        {
            return long.TryParse(context.BackgroundJob.Id, out var currentJobId)
                ? JobStorage.Current
                    ?.GetMonitoringApi()
                    ?.SucceededJobs(0, (int)currentJobId)
                    ?.LastOrDefault(x => x.Value?.Job?.Method?.Name == methodName).Value?.SucceededAt
                : default;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                foreach (var jobDetail in _jobDetails)
                {
                    recurringJobManager.AddOrUpdate(
                        recurringJobId: jobDetail.Id,
                        job: jobDetail.Job,
                        cronExpression: jobDetail.CronExpression,
                        options: jobDetail.Options);
                }
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
            using (var scope = _services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                foreach (var jobDetail in _jobDetails)
                {
                    recurringJobManager.RemoveIfExists(jobDetail.Id);
                }
            }

            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStopped, $"{HostedServiceStopped}"),
                message: "Hangfire Job Service Stopped at {@Time}",
                args: new object[] { DateTime.UtcNow });
            return Task.CompletedTask;
        }
    }
}
