namespace Services
{
    using System.Threading.Tasks;
    using Quartz;

    /// <inheritdoc />
    public abstract class QuartzJobDetail : IJob
    {
        public readonly string CronExpression;

        protected QuartzJobDetail(string cronExpression)
        {
            CronExpression = cronExpression;
        }

        /// <inheritdoc />
        public abstract Task Execute(IJobExecutionContext context);
    }
}
