namespace Services
{
    using System;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Stripe;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStripePaymentService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(StripePaymentOptions));
            if (!section.Exists())
            {
                throw new Exception("StripePaymentOptions section doesn't exist");
            }

            services.Configure<StripePaymentOptions>(section);
            var stripePaymentOptions = section.Get<StripePaymentOptions>();
            if (stripePaymentOptions == default ||
                string.IsNullOrEmpty(stripePaymentOptions.SecretKey))
            {
                throw new Exception("StripePaymentOptions section is invalid");
            }

            var stripeClient = new StripeClient(stripePaymentOptions.SecretKey);
            services.AddTransient(
                implementationFactory: sp => new CustomerService(
                    client: stripeClient));
            services.AddTransient(
                implementationFactory: sp => new ChargeService(
                    client: stripeClient));
            services.AddTransient<IPaymentService, StripePaymentService>();
            return services;
        }
    }
}
