namespace Services
{
    using System;
    using Hangfire.Common;
    using Hangfire.States;
    using Hangfire.Storage;
    using Microsoft.Extensions.Options;

    // https://discuss.hangfire.io/t/how-to-configure-the-retention-time-of-job
    public class ApplyStateFilter : JobFilterAttribute, IApplyStateFilter
    {
        private readonly TimeSpan _jobExpirationTimeout;

        public ApplyStateFilter(IOptions<HangfireJobOptions> hangfireJobOptions)
        {
            _jobExpirationTimeout = hangfireJobOptions.Value.JobExpirationTimeout;
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = _jobExpirationTimeout;
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = _jobExpirationTimeout;
        }
    }
}
