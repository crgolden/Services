namespace Services
{
    using System.Threading.Tasks;
    using Quartz;

    public abstract class QuartzJobDetail : IJob
    {
        public readonly string CronExpression;

        protected QuartzJobDetail(string cronExpression)
        {
            CronExpression = cronExpression;
        }

        public abstract Task Execute(IJobExecutionContext context);
    }
}
