namespace Services
{
    using System;
    using Hangfire.Common;
    using Hangfire.States;
    using Hangfire.Storage;
    using Microsoft.Extensions.Options;

    // https://discuss.hangfire.io/t/how-to-configure-the-retention-time-of-job
    public class JobExpirationTimeoutAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly TimeSpan _jobExpirationTimeout;

        public JobExpirationTimeoutAttribute(IOptions<HangfireJobOptions>? hangfireJobOptions)
        {
            if (hangfireJobOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(hangfireJobOptions));
            }

            _jobExpirationTimeout = hangfireJobOptions.Value.JobExpirationTimeout;
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context == default)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.JobExpirationTimeout = _jobExpirationTimeout;
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context == default)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.JobExpirationTimeout = _jobExpirationTimeout;
        }
    }
}
