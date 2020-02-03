namespace Services
{
    using JetBrains.Annotations;
    using static QuartzJobStore;

    /// <summary>Configuration settings for the <see cref="QuartzJobService"/> class.</summary>
    [PublicAPI]
    public class QuartzJobStoreOptions
    {
        /// <summary>Gets or sets the name of the instance.</summary>
        /// <value>The name of the instance.</value>
        public string InstanceName { get; set; }

        /// <summary>Gets or sets the instance identifier.</summary>
        /// <value>The instance identifier.</value>
        public string InstanceId { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; set; } = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

        /// <summary>Gets or sets the type of the driver delegate.</summary>
        /// <value>The type of the driver delegate.</value>
        public string DriverDelegateType { get; set; } = StandardAdoDelegate;

        /// <summary>Gets or sets the data source.</summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; } = "default";

        /// <summary>Gets or sets the table prefix.</summary>
        /// <value>The table prefix.</value>
        public string TablePrefix { get; set; } = "QRTZ_";

        /// <summary>Gets or sets the use properties.</summary>
        /// <value>The use properties.</value>
        public string UseProperties { get; set; } = "true";

        /// <summary>Gets or sets the type of the lock handler.</summary>
        /// <value>The type of the lock handler.</value>
        public string LockHandlerType { get; set; } = StandardLockHandler;

        /// <summary>Gets or sets the data source provider.</summary>
        /// <value>The data source provider.</value>
        public string DataSourceProvider { get; set; }

        /// <summary>Gets or sets the data source connection string.</summary>
        /// <value>The data source connection string.</value>
        public string DataSourceConnectionString { get; set; }
    }
}
