namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Quartz.Spi;
    using static Common.EventId;

    public class QuartzJobService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> _triggersAndJobs;
        private readonly ILogger<QuartzJobService> _logger;

        public QuartzJobService(
            IServiceProvider services,
            IEnumerable<IJob> jobDetails,
            ILogger<QuartzJobService> logger)
        {
            _services = services;
            _triggersAndJobs = GetTriggersAndJobs(jobDetails);
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var jobFactory = scope.ServiceProvider.GetRequiredService<IJobFactory>();
                var scheduler = await schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
                scheduler.JobFactory = jobFactory;
                await scheduler.ScheduleJobs(_triggersAndJobs, true, cancellationToken).ConfigureAwait(false);
                await scheduler.Start(cancellationToken).ConfigureAwait(false);
            }

            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStarted, $"{HostedServiceStarted}"),
                message: "Quartz Job Service Started at {@Time}",
                args: new object[] { DateTime.UtcNow });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
                var jobKeys = _triggersAndJobs.Keys.Select(x => x.Key).ToArray();
                await scheduler.DeleteJobs(jobKeys, cancellationToken).ConfigureAwait(false);
                await scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
            }

            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStopped, $"{HostedServiceStopped}"),
                message: "Quartz Job Service Stopped at {@Time}",
                args: new object[] { DateTime.UtcNow });
        }

        private static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetTriggersAndJobs(
            IEnumerable<IJob> jobDetails)
        {
            var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            foreach (var jobDetail in jobDetails.Where(x => x is QuartzJobDetail).Cast<QuartzJobDetail>())
            {
                var type = jobDetail.GetType();
                var job = JobBuilder
                    .Create()
                    .OfType(type)
                    .WithDescription(type.Name)
                    .WithIdentity(type.FullName)
                    .Build();
                var trigger = TriggerBuilder
                    .Create()
                    .WithDescription($"{type.Name}.Trigger")
                    .WithIdentity($"{type.FullName}.Trigger")
                    .WithCronSchedule(jobDetail.CronExpression)
                    .Build();
                triggersAndJobs.Add(job, new[] { trigger });
            }

            return triggersAndJobs;
        }
    }
}
