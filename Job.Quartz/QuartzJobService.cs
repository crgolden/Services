namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Quartz.Spi;
    using static Common.EventIds;

    public class QuartzJobService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> _triggersAndJobs;
        private readonly ILogger<QuartzJobService> _logger;
        private IScheduler _scheduler;

        public QuartzJobService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<QuartzJobDetail> jobDetails,
            ILogger<QuartzJobService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _triggersAndJobs = GetTriggersAndJobs(jobDetails);
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
            _scheduler.JobFactory = _jobFactory;
            await _scheduler.ScheduleJobs(_triggersAndJobs, true, cancellationToken).ConfigureAwait(false);
            await _scheduler.Start(cancellationToken).ConfigureAwait(false);
            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStarted, $"{HostedServiceStarted}"),
                message: "Quartz Job Service Started at {@Time}",
                args: new object[] { DateTime.UtcNow });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var jobKeys = _triggersAndJobs.Keys.Select(x => x.Key).ToArray();
            await _scheduler.DeleteJobs(jobKeys, cancellationToken).ConfigureAwait(false);
            await _scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
            _logger.Log(
                logLevel: LogLevel.Information,
                eventId: new EventId((int)HostedServiceStopped, $"{HostedServiceStopped}"),
                message: "Quartz Job Service Stopped at {@Time}",
                args: new object[] { DateTime.UtcNow });
        }

        private static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetTriggersAndJobs(
            IEnumerable<QuartzJobDetail> jobDetails)
        {
            var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            foreach (var jobDetail in jobDetails)
            {
                var job = JobBuilder
                    .Create()
                    .OfType(jobDetail.Type)
                    .WithDescription(jobDetail.Type.Name)
                    .WithIdentity(jobDetail.Type.FullName)
                    .Build();
                var trigger = TriggerBuilder
                    .Create()
                    .WithDescription($"{jobDetail.Type.Name}.Trigger")
                    .WithIdentity($"{jobDetail.Type.FullName}.Trigger")
                    .WithCronSchedule(jobDetail.CronExpression)
                    .Build();
                triggersAndJobs.Add(job, new[] { trigger });
            }

            return triggersAndJobs;
        }
    }
}
