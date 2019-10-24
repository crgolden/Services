namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Stripe;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStripePaymentService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(StripePaymentOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(StripePaymentOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<StripePaymentOptions>(section);
            var options = section.Get<StripePaymentOptions>();
            if (options == default ||
                IsNullOrEmpty(options.SecretKey))
            {
                throw new ArgumentException(
                    message: $"{nameof(StripePaymentOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            var stripeClient = new StripeClient(options.SecretKey);
            services.AddTransient(
                implementationFactory: sp => new CustomerService(stripeClient));
            services.AddTransient(
                implementationFactory: sp => new ChargeService(stripeClient));
            services.AddTransient<IPaymentService, StripePaymentService>();
            return services;
        }
    }
}
