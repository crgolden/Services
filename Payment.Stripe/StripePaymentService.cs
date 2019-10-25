namespace Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Stripe;
    using static System.DateTime;
    using static System.String;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public Task<string?> GetCustomerAsync(
            string? customerId,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            return GetCustomer(customerId, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string?> CreateCustomerAsync(
            string? email,
            string? tokenId,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (IsNullOrEmpty(tokenId))
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            return CreateCustomer(email, tokenId, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string?> AuthorizeAsync(
            string? customerId,
            decimal? amount,
            string? currency,
            string? description = default,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (!amount.HasValue)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (IsNullOrEmpty(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            return Authorize(customerId, amount.Value, currency, description, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string?> CaptureAsync(
            string? customerId,
            decimal? amount,
            string? currency,
            string? description = default,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (!amount.HasValue)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (IsNullOrEmpty(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            return Capture(customerId, amount.Value, currency, description, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateAsync(
            string? chargeId,
            string? description,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(chargeId))
            {
                throw new ArgumentNullException(nameof(chargeId));
            }

            if (IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return Update(chargeId, description, logLevel, cancellationToken);
        }

        private async Task<string?> GetCustomer(
            string customerId,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var customerGetOptions = new CustomerGetOptions();
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentGetCustomerStart, $"{PaymentGetCustomerStart}"),
                    message: "Getting Customer Id {@CustomerId} with options {@Options} at {@Time}",
                    args: new object[] { customerId, customerGetOptions, UtcNow });
                var customer = await _customerService
                    .GetAsync(
                        customerId: customerId,
                        options: customerGetOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentGetCustomerEnd, $"{PaymentGetCustomerEnd}"),
                    message: "Got Customer {@Customer} with options {@Options} at {@Time}",
                    args: new object[] { customerId, customerGetOptions, UtcNow });
                return customer?.Id;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentGetCustomerError, $"{PaymentGetCustomerError}"),
                    exception: e,
                    message: "Error getting Customer Id {@CustomerId} with options {@Options} at {@Time}",
                    args: new object[] { customerId, customerGetOptions, UtcNow });
                throw;
            }
        }

        private async Task<string?> CreateCustomer(
            string email,
            string tokenId,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var customerCreateOptions = new CustomerCreateOptions
            {
                Email = email,
                Source = tokenId
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCreateCustomerStart, $"{PaymentCreateCustomerStart}"),
                    message: "Creating Customer Email {@CustomerEmail} with options {@Options} at {@Time}",
                    args: new object[] { email, customerCreateOptions, UtcNow });
                var customer = await _customerService
                    .CreateAsync(
                        options: customerCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCreateCustomerEnd, $"{PaymentCreateCustomerEnd}"),
                    message: "Created Customer {@Customer} with options {@Options} at {@Time}",
                    args: new object[] { customer, customerCreateOptions, UtcNow });
                return customer?.Id;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCreateCustomerError, $"{PaymentCreateCustomerError}"),
                    exception: e,
                    message: "Error creating Customer Email {@CustomerEmail} with options {@Options} at {@Time}",
                    args: new object[] { email, customerCreateOptions, UtcNow });
                throw;
            }
        }

        private async Task<string?> Authorize(
            string customerId,
            decimal amount,
            string currency,
            string? description,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = (long?)amount * 100,
                Currency = currency,
                Description = description,
                Customer = customerId,
                Capture = false
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentAuthorizeStart, $"{PaymentAuthorizeStart}"),
                    message: "Authorizing charge with options {@Options} at {@Time}",
                    args: new object[] { chargeCreateOptions, UtcNow });
                var charge = await _chargeService
                    .CreateAsync(
                        options: chargeCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentAuthorizeEnd, $"{PaymentAuthorizeEnd}"),
                    message: "Authorized charge {@Charge} with options {@Options} at {@Time}",
                    args: new object[] { charge, chargeCreateOptions, UtcNow });
                return charge?.Id;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentAuthorizeError, $"{PaymentAuthorizeError}"),
                    exception: e,
                    message: "Error authorizing charge with options {@Options} at {@Time}",
                    args: new object[] { chargeCreateOptions, UtcNow });
                throw;
            }
        }

        private async Task<string?> Capture(
            string customerId,
            decimal amount,
            string currency,
            string? description,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = (long?)amount * 100,
                Currency = currency,
                Description = description,
                Customer = customerId,
                Capture = true
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCaptureStart, $"{PaymentCaptureStart}"),
                    message: "Capturing charge with options {@Options} at {@Time}",
                    args: new object[] { chargeCreateOptions, UtcNow });
                var charge = await _chargeService
                    .CreateAsync(
                        options: chargeCreateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCaptureEnd, $"{PaymentCaptureEnd}"),
                    message: "Captured charge {@Charge} with options {@Options} at {@Time}",
                    args: new object[] { charge, chargeCreateOptions, UtcNow });
                return charge?.Id;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentCaptureError, $"{PaymentCaptureError}"),
                    exception: e,
                    message: "Error capturing charge with options {@Options} at {@Time}",
                    args: new object[] { chargeCreateOptions, UtcNow });
                throw;
            }
        }

        private async Task Update(
            string chargeId,
            string description,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var chargeUpdateOptions = new ChargeUpdateOptions
            {
                Description = description
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentUpdateStart, $"{PaymentUpdateStart}"),
                    message: "Updating charge with options {@Options} at {@Time}",
                    args: new object[] { chargeUpdateOptions, UtcNow });
                var charge = await _chargeService
                    .UpdateAsync(
                        chargeId: chargeId,
                        options: chargeUpdateOptions,
                        requestOptions: default,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentUpdateEnd, $"{PaymentUpdateEnd}"),
                    message: "Updated charge {@Charge} with options {@Options} at {@Time}",
                    args: new object[] { charge, chargeUpdateOptions, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)PaymentUpdateError, $"{PaymentUpdateError}"),
                    exception: e,
                    message: "Error updating charge with options {@Options} at {@Time}",
                    args: new object[] { chargeUpdateOptions, UtcNow });
                throw;
            }
        }
    }
}
