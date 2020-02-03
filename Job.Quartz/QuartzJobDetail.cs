namespace Services
{
    using System;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Quartz;
    using static System.String;

    /// <inheritdoc />
    [PublicAPI]
    public abstract class QuartzJobDetail : IJob
    {
        /// <summary>Initializes a new instance of the <see cref="QuartzJobDetail"/> class.</summary>
        /// <param name="cronExpression">The cron expression.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cronExpression"/> is <see langword="null"/>.</exception>
        protected QuartzJobDetail(string cronExpression)
        {
            if (IsNullOrWhiteSpace(cronExpression))
            {
                throw new ArgumentNullException(nameof(cronExpression));
            }

            CronExpression = cronExpression;
        }

        /// <summary>Gets the cron expression.</summary>
        /// <value>The cron expression.</value>
        public string CronExpression { get; }

        /// <inheritdoc />
        public abstract Task Execute(IJobExecutionContext context);
    }
}
