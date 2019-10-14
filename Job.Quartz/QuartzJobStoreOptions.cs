namespace Services
{
    using static QuartzJobStore;

    public class QuartzJobStoreOptions
    {
        public string InstanceName { get; set; }

        public string InstanceId { get; set; }

        public string Type { get; set; } = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

        public string DriverDelegateType { get; set; } = StandardAdoDelegate;

        public string DataSource { get; set; } = "default";

        public string TablePrefix { get; set; } = "QRTZ_";

        public string UseProperties { get; set; } = "true";

        public string LockHandlerType { get; set; } = StandardLockHandler;

        public string DataSourceProvider { get; set; }

        public string DataSourceConnectionString { get; set; }
    }
}