namespace Services
{
    using System;
    using JetBrains.Annotations;
    using static System.TimeSpan;

    /// <summary>Configuration settings for the <see cref="HangfireJobService"/> class.</summary>
    [PublicAPI]
    public class HangfireJobOptions
    {
        /// <summary>Gets or sets a value indicating whether to use Hangfire Serve.</summary>
        /// <value>
        /// <c>true</c> if set to use Hangfire Server; otherwise, <c>false</c>.</value>
        public bool UseHangfireServer { get; set; }

        /// <summary>Gets or sets the job expiration timeout.</summary>
        /// <value>The job expiration timeout.</value>
        public TimeSpan JobExpirationTimeout { get; set; } = FromDays(7);
    }
}
