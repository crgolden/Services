namespace Services
{
    using System;
    using Hangfire.Annotations;
    using Hangfire.Common;
    using Hangfire.States;
    using Hangfire.Storage;
    using Microsoft.Extensions.Options;

    // https://discuss.hangfire.io/t/how-to-configure-the-retention-time-of-job

    /// <inheritdoc cref="IApplyStateFilter" />
    [PublicAPI]
    public class JobExpirationTimeoutAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly TimeSpan _jobExpirationTimeout;

        /// <summary>Initializes a new instance of the <see cref="JobExpirationTimeoutAttribute"/> class.</summary>
        /// <param name="hangfireJobOptions">The hangfire job options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hangfireJobOptions"/> is <see langword="null"/>.</exception>
        public JobExpirationTimeoutAttribute(IOptions<HangfireJobOptions> hangfireJobOptions)
        {
            if (hangfireJobOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(hangfireJobOptions));
            }

            _jobExpirationTimeout = hangfireJobOptions.Value.JobExpirationTimeout;
        }

        /// <inheritdoc />
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            ApplyJobExpirationTimeout(context);
        }

        /// <inheritdoc />
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            ApplyJobExpirationTimeout(context);
        }

        private void ApplyJobExpirationTimeout(ApplyStateContext context)
        {
            if (context == default)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.JobExpirationTimeout = _jobExpirationTimeout;
        }
    }
}
