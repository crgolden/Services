namespace Services
{
    using System;

    public class HangfireJobOptions
    {
        public TimeSpan JobExpirationTimeout { get; set; }
    }
}
