namespace Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using JetBrains.Annotations;
    using Stripe;
    using static System.String;

    /// <inheritdoc />
    [PublicAPI]
    public class StripePaymentService : IPaymentService
    {
        private readonly IStripeClient _stripeClient;

        /// <summary>Initializes a new instance of the <see cref="StripePaymentService"/> class.</summary>
        /// <param name="stripeClient">The Stripe client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stripeClient"/> is <see langword="null"/>.</exception>
        public StripePaymentService(IStripeClient stripeClient)
        {
            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
        }

        /// <inheritdoc />
        public Task<string> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            async Task<string> GetCustomerAsync()
            {
                var customerService = new CustomerService(_stripeClient);
                var customerGetOptions = new CustomerGetOptions();
                var customer = await customerService
                    .GetAsync(
                        customerId: customerId,
                        options: customerGetOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return customer.Id;
            }

            return GetCustomerAsync();
        }

        /// <inheritdoc />
        public Task<string> CreateCustomerAsync(string email, string tokenId, CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (IsNullOrWhiteSpace(tokenId))
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            async Task<string> CreateCustomerAsync()
            {
                var customerService = new CustomerService(_stripeClient);
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Email = email,
                    Source = tokenId
                };
                var customer = await customerService
                    .CreateAsync(
                        options: customerCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return customer.Id;
            }

            return CreateCustomerAsync();
        }

        /// <inheritdoc />
        public Task<string> AuthorizeAsync(
            string customerId,
            decimal amount,
            string currency,
            string description = default,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            async Task<string> AuthorizeAsync()
            {
                var chargeService = new ChargeService(_stripeClient);
                var chargeCreateOptions = new ChargeCreateOptions
                {
                    Amount = (long?)amount * 100,
                    Currency = currency,
                    Description = description,
                    Customer = customerId,
                    Capture = false
                };
                var charge = await chargeService
                    .CreateAsync(
                        options: chargeCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return charge.Id;
            }

            return AuthorizeAsync();
        }

        /// <inheritdoc />
        public Task<string> CaptureAsync(
            string customerId,
            decimal amount,
            string currency,
            string description = default,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            async Task<string> CaptureAsync()
            {
                var chargeService = new ChargeService(_stripeClient);
                var chargeCreateOptions = new ChargeCreateOptions
                {
                    Amount = (long?)amount * 100,
                    Currency = currency,
                    Description = description,
                    Customer = customerId,
                    Capture = true
                };
                var charge = await chargeService
                    .CreateAsync(
                        options: chargeCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return charge.Id;
            }

            return CaptureAsync();
        }

        /// <inheritdoc />
        public Task UpdateAsync(
            string chargeId,
            string description,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(chargeId))
            {
                throw new ArgumentNullException(nameof(chargeId));
            }

            var chargeUpdateOptions = new ChargeUpdateOptions
            {
                Description = description
            };
            var chargeService = new ChargeService(_stripeClient);
            return chargeService.UpdateAsync(
                chargeId: chargeId,
                options: chargeUpdateOptions,
                requestOptions: default,
                cancellationToken: cancellationToken);
        }
    }
}
