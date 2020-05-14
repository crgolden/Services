namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Spi;

    /// <inheritdoc />
    [PublicAPI]
    public class QuartzJobService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> _triggersAndJobs;

        /// <summary>Initializes a new instance of the <see cref="QuartzJobService"/> class.</summary>
        /// <param name="schedulerFactory">The scheduler factory.</param>
        /// <param name="jobFactory">The job factory.</param>
        /// <param name="jobDetails">The job details.</param>
        /// <exception cref="ArgumentNullException"><paramref name="schedulerFactory"/> is <see langword="null"/>
        /// or
        /// <paramref name="jobFactory"/> is <see langword="null"/>
        /// or
        /// <paramref name="jobDetails"/> is <see langword="null"/>.</exception>
        public QuartzJobService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<IJob> jobDetails)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            if (jobDetails == null)
            {
                throw new ArgumentNullException(nameof(jobDetails));
            }

            _triggersAndJobs = GetTriggersAndJobs(jobDetails);
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
            scheduler.JobFactory = _jobFactory;
            await scheduler.ScheduleJobs(_triggersAndJobs, true, cancellationToken).ConfigureAwait(false);
            await scheduler.Start(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
            scheduler.JobFactory = _jobFactory;
            var jobKeys = _triggersAndJobs.Keys.Select(x => x.Key).ToArray();
            await scheduler.DeleteJobs(jobKeys, cancellationToken).ConfigureAwait(false);
            await scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
        }

        private static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetTriggersAndJobs(IEnumerable<IJob> jobDetails)
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
