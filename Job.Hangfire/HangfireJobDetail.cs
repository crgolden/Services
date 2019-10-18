namespace Services
{
    using Hangfire;
    using Hangfire.Common;

    public class HangfireJobDetail
    {
        public readonly string Id;

        public readonly Job Job;

        public readonly string CronExpression;

        public readonly RecurringJobOptions? Options;

        public HangfireJobDetail(
            string id,
            Job job,
            string cronExpression,
            RecurringJobOptions? options = default)
        {
            Id = id;
            Job = job;
            CronExpression = cronExpression;
            Options = options;
        }
    }
}
