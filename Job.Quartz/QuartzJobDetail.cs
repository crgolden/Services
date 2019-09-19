namespace Services
{
    using System;

    public class QuartzJobDetail
    {
        public readonly Type Type;

        public readonly string CronExpression;

        public QuartzJobDetail(Type type, string cronExpression)
        {
            Type = type;
            CronExpression = cronExpression;
        }
    }
}
