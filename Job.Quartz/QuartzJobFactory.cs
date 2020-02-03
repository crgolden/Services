namespace Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Spi;

    /// <inheritdoc />
    [PublicAPI]
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>Initializes a new instance of the <see cref="QuartzJobFactory"/> class.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc />
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle?.JobDetail?.JobType) as IJob;
        }

        /// <inheritdoc />
        [SuppressMessage("Critical Code Smell", "S1186:Methods should not be empty", Justification = "No implementation")]
        public void ReturnJob(IJob job)
        {
        }
    }
}
