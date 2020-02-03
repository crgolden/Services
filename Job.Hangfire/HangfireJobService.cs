namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Server;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static System.Threading.Tasks.Task;
    using static Hangfire.GlobalJobFilters;
    using static Hangfire.JobStorage;

    /// <inheritdoc />
    [PublicAPI]
    public class HangfireJobService : IHostedService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IEnumerable<HangfireJobDetail> _jobDetails;

        /// <summary>Initializes a new instance of the <see cref="HangfireJobService"/> class.</summary>
        /// <param name="recurringJobManager">The recurring job manager.</param>
        /// <param name="jobDetails">The job details.</param>
        /// <param name="hangfireJobOptions">The hangfire job options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="recurringJobManager"/> is <see langword="null"/>
        /// or
        /// <paramref name="jobDetails"/> is <see langword="null"/>
        /// or
        /// <paramref name="hangfireJobOptions"/> is <see langword="null"/>.</exception>
        public HangfireJobService(
            IRecurringJobManager recurringJobManager,
            IEnumerable<HangfireJobDetail> jobDetails,
            IOptions<HangfireJobOptions> hangfireJobOptions)
        {
            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
            _jobDetails = jobDetails ?? throw new ArgumentNullException(nameof(jobDetails));
            if (hangfireJobOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(hangfireJobOptions));
            }

            Filters.Add(new JobExpirationTimeoutAttribute(hangfireJobOptions));
        }

        /// <summary>Gets the compare date.</summary>
        /// <param name="context">The context.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>The compare date.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>
        /// or
        /// <paramref name="methodName"/> is <see langword="null"/>.</exception>
        public static DateTime? GetCompareDate(PerformContext context, string methodName)
        {
            if (context == default)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            return long.TryParse(context.BackgroundJob?.Id, out var currentJobId)
                ? Current
                    ?.GetMonitoringApi()
                    ?.SucceededJobs(0, (int)currentJobId)
                    ?.LastOrDefault(x => x.Value?.Job?.Method?.Name == methodName).Value?.SucceededAt
                : default;
        }

        /// <inheritdoc />
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

            return CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var jobDetail in _jobDetails)
            {
                _recurringJobManager.RemoveIfExists(jobDetail.Id);
            }

            return CompletedTask;
        }
    }
}
