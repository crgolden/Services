namespace Services
{
    using System;

    public class HangfireJobOptions
    {
        public string ConnectionString { get; set; }

        public TimeSpan JobExpirationTimeout { get; set; } = TimeSpan.FromDays(7);
    }
}
