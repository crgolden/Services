namespace Services
{
    using System;
    using static System.TimeSpan;

    public class HangfireJobOptions
    {
        public string? ConnectionString { get; set; }

        public TimeSpan JobExpirationTimeout { get; set; } = FromDays(7);
    }
}
