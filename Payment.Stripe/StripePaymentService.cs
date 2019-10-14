namespace Services
{
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
            CustomerService customerService,
            ChargeService chargeService,
            ILogger<StripePaymentService> logger)
        {
            _customerService = customerService;
            _chargeService = chargeService;
            _logger = logger;
        }

        public async Task<string> GetCustomerAsync(
            string customerId,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerService.GetAsync(
                customerId,
                default,
                default,
                cancellationToken).ConfigureAwait(false);
            return customer.Id;
        }

        public virtual async Task<string> CreateCustomerAsync(
            string email,
            string tokenId,
            CancellationToken cancellationToken = default)
        {
            var customerCreateOptions = new CustomerCreateOptions
            {
                Email = email,
                Source = tokenId
            };
            var customer = await _customerService.CreateAsync(
                customerCreateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
            return customer.Id;
        }

        public virtual async Task<string> AuthorizeAsync(
            string customerId,
            decimal amount,
            string currency,
            string description = default,
            CancellationToken cancellationToken = default)
        {
            var chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = (long?)amount * 100,
                Currency = currency,
                Description = description,
                Customer = customerId,
                Capture = false
            };
            var charge = await _chargeService.CreateAsync(
                chargeCreateOptions,
                default,
                cancellationToken).ConfigureAwait(false);
            return charge.Id;
        }

        public virtual async Task<string> CaptureAsync(
            string customerId,
            decimal amount,
            string currency,
            string description = default,
            CancellationToken cancellationToken = default)
        {
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
            return charge.Id;
        }

        public virtual async Task UpdateAsync(
            string chargeId,
            string description,
            CancellationToken cancellationToken = default)
        {
            var chargeUpdateOptions = new ChargeUpdateOptions
            {
                Description = description
            };
            await _chargeService.UpdateAsync(
                chargeId,
                chargeUpdateOptions,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
