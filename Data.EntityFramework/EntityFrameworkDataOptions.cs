namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="EntityFrameworkDataService"/> class.</summary>
    [PublicAPI]
    public class EntityFrameworkDataOptions
    {
        /// <summary>Gets or sets a value indicating whether to use pooling.</summary>
        /// <value>
        /// <c>true</c> if set to use pooling; otherwise, <c>false</c>.</value>
        public bool UsePooling { get; set; }

        /// <summary>Gets or sets the size of the pool.</summary>
        /// <value>The size of the pool.</value>
        public int? PoolSize { get; set; }
    }
}
