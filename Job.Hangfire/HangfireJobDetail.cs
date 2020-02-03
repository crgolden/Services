namespace Services
{
    using Hangfire;
    using Hangfire.Common;
    using JetBrains.Annotations;

    /// <summary>Details of a Hangfire job.</summary>
    [PublicAPI]
    public class HangfireJobDetail
    {
        /// <summary>Initializes a new instance of the <see cref="HangfireJobDetail"/> class.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="job">The job.</param>
        /// <param name="cronExpression">The cron expression.</param>
        /// <param name="options">The options.</param>
        public HangfireJobDetail(
            string id,
            Job job,
            string cronExpression,
            RecurringJobOptions options = default)
        {
            Id = id;
            Job = job;
            CronExpression = cronExpression;
            Options = options;
        }

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>Gets the job.</summary>
        /// <value>The job.</value>
        public Job Job { get; }

        /// <summary>Gets the cron expression.</summary>
        /// <value>The cron expression.</value>
        public string CronExpression { get; }

        /// <summary>Gets the options.</summary>
        /// <value>The options.</value>
        public RecurringJobOptions Options { get; }
    }
}
