namespace Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Stripe;

    public class StripePaymentService : IPaymentService
    {
        private readonly CustomerService _customerService;
        private readonly ChargeService _chargeService;
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(
            CustomerService? customerService,
            ChargeService? chargeService,
            ILogger<StripePaymentService>? logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _chargeService = chargeService ?? throw new ArgumentNullException(nameof(chargeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string?> GetCustomerAsync(
            string? customerId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var customer = await _customerService.GetAsync(
                customerId,
                default,
                default,
                cancellationToken).ConfigureAwait(false);
            return customer?.Id;
        }

        public virtual async Task<string?> CreateCustomerAsync(
            string? email,
            string? tokenId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(tokenId))
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            var customerCreateOptions = new CustomerCreateOptions
            {
                Email = email,
                Source = tokenId
            };
            var customer = await _customerService.CreateAsync(
                customerCreateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
            return customer?.Id;
        }

        public virtual async Task<string?> AuthorizeAsync(
            string? customerId,
            decimal? amount,
            string? currency,
            string? description = default,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (!amount.HasValue)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            var chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = (long?)amount.Value * 100,
                Currency = currency,
                Description = description,
                Customer = customerId,
                Capture = false
            };
            var charge = await _chargeService.CreateAsync(
                chargeCreateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
            return charge?.Id;
        }

        public virtual async Task<string?> CaptureAsync(
            string? customerId,
            decimal? amount,
            string? currency,
            string? description = default,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (!amount.HasValue)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            var chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = (long?)amount * 100,
                Currency = currency,
                Description = description,
                Customer = customerId,
                Capture = true
            };
            var charge = await _chargeService.CreateAsync(
                chargeCreateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
            return charge?.Id;
        }

        public virtual async Task UpdateAsync(
            string? chargeId,
            string? description,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(chargeId))
            {
                throw new ArgumentNullException(nameof(chargeId));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            var chargeUpdateOptions = new ChargeUpdateOptions
            {
                Description = description
            };
            await _chargeService.UpdateAsync(
                chargeId,
                chargeUpdateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
