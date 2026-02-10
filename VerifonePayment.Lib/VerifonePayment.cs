using System;
using System.Collections.Generic;
using System.IO;
using VerifonePayment.Lib.Configuration;
using VerifonePayment.Lib.Models;
using VerifoneSdk;
using static VerifonePayment.Lib.Enums;

namespace VerifonePayment.Lib
{
    public class VerifonePayment
    {
        #region "Constants"

        #region "Verifone"

        /// <summary>
        /// Constant for device address key.
        /// </summary>
        private const string _DEVICE_ADDRESS_KEY = "DEVICE_ADDRESS_KEY";
        /// <summary>
        /// Constant for device connection type key.
        /// </summary>
        private const string _DEVICE_CONNECTION_TYPE_KEY = "DEVICE_CONNECTION_TYPE_KEY";
        /// <summary>
        /// Constant for device connection type key value.
        /// </summary>
        private const string _DEVICE_CONNECTION_TYPE_KEY_VALUE = "tcpip";

        #endregion

        /// <summary>
        /// Default log file.
        /// </summary>
        private const string _defaultLogFile = "psdk.log";
        /// <summary>
        /// Constant for ip address empty.
        /// </summary>
        private const string _TextIpAddressEmpty = "Ip address empty";

        #endregion

        #region "Members"

        /// <summary>
        /// Payment SDK.
        /// </summary>
        private readonly PaymentSdk payment_sdk_ = new PaymentSdk();
        /// <summary>
        /// Listener.
        /// </summary>
        private readonly VerifonePaymentListener listener_ = new VerifonePaymentListener();
        /// <summary>
        /// Configuration settings.
        /// </summary>
        private readonly PaymentConfiguration configuration_;

        #endregion

        #region "Events"

        /// <summary>
        /// Event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> StatusEventOccurred
        {
            add { listener_.StatusEventOccurred += value; }
            remove { listener_.StatusEventOccurred -= value; }
        }
        /// <summary>
        /// Transaction event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> TransactionEventOccurred
        {
            add { listener_.TransactionEventOccurred += value; }
            remove { listener_.TransactionEventOccurred -= value; }
        }
        /// <summary>
        /// Device vitals information event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> DeviceVitalsInformationEventOccurred
        {
            add { listener_.DeviceVitalsInformationEventOccurred += value; }
            remove { listener_.DeviceVitalsInformationEventOccurred -= value; }
        }
        /// <summary>
        /// Basket event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> BasketEventOccurred
        {
            add { listener_.BasketEventOccurred += value; }
            remove { listener_.BasketEventOccurred -= value; }
        }
        /// <summary>
        /// Notification event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> NotificationEventOccurred
        {
            add { listener_.NotificationEventOccurred += value; }
            remove { listener_.NotificationEventOccurred -= value; }
        }
        /// <summary>
        /// Payment completed event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> PaymentCompletedEventOccurred
        {
            add { listener_.PaymentCompletedEventOccurred += value; }
            remove { listener_.PaymentCompletedEventOccurred -= value; }
        }
        /// <summary>
        /// Commerce event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> CommerceEventOccurred
        {
            add { listener_.CommerceEventOccurred += value; }
            remove { listener_.CommerceEventOccurred -= value; }
        }

        /// <summary>
        /// Refund completed event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> RefundCompletedEventOccurred
        {
            add { listener_.RefundCompletedEventOccurred += value; }
            remove { listener_.RefundCompletedEventOccurred -= value; }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Log file.
        /// </summary>
        public string LogFile { get; set; }
        /// <summary>
        /// Ip address.
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// Delete log file.
        /// </summary>
        public bool DeleteLogFile { get; set; } = false;
        /// <summary>
        /// Configuration settings.
        /// </summary>
        public PaymentConfiguration Configuration => configuration_;

        #endregion

        #region "Constructor"

        /// <summary>
        /// Initializes a new instance using configuration from app.config
        /// </summary>
        public VerifonePayment()
        {
            configuration_ = new PaymentConfiguration();
            
            if (!configuration_.IsValid())
                throw new InvalidOperationException("Invalid payment configuration. Please check your app.config settings.");
                
            InitializeFromConfiguration();
        }

        /// <summary>
        /// Initializes a new instance with specific IP address (overrides configuration)
        /// </summary>
        /// <param name="ipAddress">Device IP address</param>
        public VerifonePayment(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException(_TextIpAddressEmpty, nameof(ipAddress));

            configuration_ = new PaymentConfiguration(ipAddress);
            InitializeFromConfiguration();
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        /// Initialize properties from configuration.
        /// </summary>
        private void InitializeFromConfiguration()
        {
            IpAddress = configuration_.DeviceIpAddress;
            DeleteLogFile = configuration_.DeleteLogFile;
            ConfigureLogFile();
        }

        /// <summary>
        /// Configure log file.
        /// </summary>
        private void ConfigureLogFile()
        {
            // Use configured log file path or default to temp directory
            if (string.IsNullOrEmpty(LogFile))
            {
                var logFileName = !string.IsNullOrEmpty(configuration_.LogFilePath) 
                    ? configuration_.LogFilePath 
                    : Path.Combine(Path.GetTempPath(), _defaultLogFile);

                if (DeleteLogFile && File.Exists(logFileName))
                    File.Delete(logFileName);

                LogFile = logFileName;
            }

            PaymentSdk.ConfigureLogFile(LogFile, 2048);
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Communicate with payment SDK.
        /// </summary>
        public void CommunicateWithPaymentSDK()
        {
            var dict = new Dictionary<string, string>
            {
                { _DEVICE_ADDRESS_KEY, configuration_.DeviceIpAddress },
                { _DEVICE_CONNECTION_TYPE_KEY, configuration_.DeviceConnectionType }
            };

            payment_sdk_.InitializeFromValues(listener_, dict);
        }

        /// <summary>
        /// Login with credentials.
        /// </summary>
        /// <returns>The status of login.</returns>
        public bool LoginWithCredentials()
        {
            var credentials = LoginCredentials.Create();

            credentials.UserId = configuration_.DefaultUsername;
            credentials.Password = configuration_.DefaultPassword;
            credentials.ShiftNumber = configuration_.DefaultShiftNumber;

            var status = payment_sdk_.TransactionManager.LoginWithCredentials(credentials);

            return status.StatusCode == 0;
        }

        /// <summary>
        /// Login with custom credentials (overrides configuration).
        /// </summary>
        /// <param name="username">Username for login</param>
        /// <param name="password">Password for login</param>
        /// <param name="shiftNumber">Shift number for login</param>
        /// <returns>The status of login.</returns>
        public bool LoginWithCredentials(string username, string password, string shiftNumber)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            if (string.IsNullOrWhiteSpace(shiftNumber))
                throw new ArgumentException("Shift number cannot be null or empty", nameof(shiftNumber));

            var credentials = LoginCredentials.Create();
            credentials.UserId = username;
            credentials.Password = password;
            credentials.ShiftNumber = shiftNumber;

            var status = payment_sdk_.TransactionManager.LoginWithCredentials(credentials);

            return status.StatusCode == 0;
        }

        /// <summary>
        /// Start session.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The status of session.</returns>
        public bool StartSession(string invoiceId)
        {
            Transaction transaction = Transaction.Create();
            transaction.InvoiceId = invoiceId;
            return payment_sdk_.TransactionManager.StartSession(transaction);
        }

        /// <summary>
        /// End session.
        /// </summary>
        /// <returns>The status of session.</returns>
        public bool EndSession()
        {
            return payment_sdk_.TransactionManager.EndSession();
        }

        /// <summary>
        /// Add merchandise.
        /// </summary>
        public void AddMerchandise()
        {
            Merchandise merchandise = Merchandise.Create();
            var current_amount_totals = payment_sdk_.TransactionManager.BasketManager.CurrentAmountTotals;

            VerifoneSdk.Decimal gratuity = new VerifoneSdk.Decimal(0);
            if (current_amount_totals == null)
            {
                current_amount_totals = AmountTotals.Create(true);
                current_amount_totals.SetWithAmounts(merchandise.ExtendedPrice, merchandise.Tax, gratuity,
                        new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(108));
            }
            else
            {
                current_amount_totals.AddAmounts(merchandise.ExtendedPrice, merchandise.Tax, gratuity,
                        new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(108));
            }

            var merch_list = new List<Merchandise>
            {
                merchandise
            };

            payment_sdk_.TransactionManager.BasketManager.AddMerchandise(merch_list, current_amount_totals);
        }

        /// <summary>
        /// Remove merchandise.
        /// </summary>
        public void RemoveMerchandise()
        {
            var basket_manager = payment_sdk_.TransactionManager.BasketManager;
            var basket = basket_manager.Basket;
            IList<Merchandise> merchandise_list = new List<Merchandise>();
            if (basket != null)
            {
                merchandise_list = basket.Merchandise;
            }
            if (merchandise_list.Count > 0)
            {
                var merchandise = merchandise_list[merchandise_list.Count - 1];
                var amount = merchandise.Amount;
                var amount_totals = basket_manager.CurrentAmountTotals;
                if (amount_totals != null)
                {
                    amount_totals.SubtractAmounts(amount, new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0),
                       new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), new VerifoneSdk.Decimal(0), amount);
                    var removed = new List<Merchandise>(); removed.Add(merchandise);
                    basket_manager.RemoveMerchandise(removed, amount_totals);
                }
            }
        }

        /// <summary>
        /// Payment transaction.
        /// </summary>
        /// <param name="total">The total amount.</param>
        public void PaymentTransaction(decimal totalAmount, string InvoiceId, string currency, Enums.PaymentType paymentType = Enums.PaymentType.CREDIT, int scale = 2)
        {
            VerifoneSdk.Decimal requestedAmount = VerifoneSdk.Decimal.FromDecimal(ref totalAmount);

            using (Payment payment = Payment.Create())
            using (AmountTotals total = AmountTotals.Create(true))
            {
                payment.LocalPaymentId = InvoiceId;
                payment.Invoice = InvoiceId;
                //payment.SaleNote = request.SaleNote;
                payment.SaleNote = "";
                payment.Currency = currency;
                total.Total = requestedAmount;
                payment.RequestedAmounts = total;

                payment_sdk_.TransactionManager.StartPayment(payment);
            }
        }

        /// <summary>
        /// Process a linked refund with reference to the original payment.
        /// </summary>
        /// <param name="originalPaymentId">The original payment ID to link to</param>
        /// <param name="refundAmount">The amount to refund (null for full refund)</param>
        /// <param name="currency">The currency</param>
        /// <param name="scale">The decimal scale</param>
        public void ProcessLinkedRefund(string originalPaymentId, decimal? refundAmount = null, string currency = "EUR", int scale = 2)
        {
            if (string.IsNullOrWhiteSpace(originalPaymentId))
                throw new ArgumentException("Original payment ID cannot be null or empty", nameof(originalPaymentId));

            using (Payment refundPayment = Payment.Create())
            {
                // Link to original payment
                refundPayment.LocalPaymentId = originalPaymentId;
                refundPayment.Invoice = $"REFUND-{originalPaymentId}-{DateTime.Now:yyyyMMddHHmmss}";
                refundPayment.Currency = currency;

                // Set refund amount if specified (for partial refunds)
                if (refundAmount.HasValue)
                {
                    decimal amount = refundAmount.Value;
                    VerifoneSdk.Decimal requestedAmount = VerifoneSdk.Decimal.FromDecimal(ref amount);
                    using (AmountTotals total = AmountTotals.Create(true))
                    {
                        total.Total = requestedAmount;
                        refundPayment.RequestedAmounts = total;
                    }
                }
                // If refundAmount is null, it will refund the entire original amount

                payment_sdk_.TransactionManager.ProcessRefund(refundPayment);
            }
        }

        /// <summary>
        /// Process an unlinked refund (standalone refund without reference to original payment).
        /// </summary>
        /// <param name="refundAmount">The amount to refund</param>
        /// <param name="currency">The currency</param>
        /// <param name="invoiceId">Optional invoice ID for the refund</param>
        /// <param name="scale">The decimal scale</param>
        public void ProcessUnlinkedRefund(decimal refundAmount, string currency = "EUR", string invoiceId = null, int scale = 2)
        {
            if (refundAmount <= 0)
                throw new ArgumentException("Refund amount must be greater than zero", nameof(refundAmount));

            decimal amount = refundAmount;
            VerifoneSdk.Decimal requestedAmount = VerifoneSdk.Decimal.FromDecimal(ref amount);

            using (Payment refundPayment = Payment.Create())
            using (AmountTotals total = AmountTotals.Create(true))
            {
                refundPayment.LocalPaymentId = invoiceId ?? $"UNLINKED-REFUND-{DateTime.Now:yyyyMMddHHmmss}";
                refundPayment.Invoice = refundPayment.LocalPaymentId;
                refundPayment.Currency = currency;
                
                total.Total = requestedAmount;
                refundPayment.RequestedAmounts = total;

                payment_sdk_.TransactionManager.ProcessRefund(refundPayment);
            }
        }

        /// <summary>
        /// Process a refund using an existing Payment object (advanced usage).
        /// </summary>
        /// <param name="refundPayment">The configured Payment object for the refund</param>
        public void ProcessRefund(Payment refundPayment)
        {
            if (refundPayment == null)
                throw new ArgumentNullException(nameof(refundPayment));

            payment_sdk_.TransactionManager.ProcessRefund(refundPayment);
        }

        /// <summary>
        /// Tear down.
        /// </summary>
        public void TearDown()
        {
            payment_sdk_.TearDown();
        }

        /// <summary>
        /// Gets the current configuration summary.
        /// </summary>
        /// <returns>Configuration summary string</returns>
        public string GetConfigurationSummary()
        {
            return configuration_.GetConfigurationSummary();
        }

        /// <summary>
        /// Validates the current configuration.
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool ValidateConfiguration()
        {
            return configuration_.IsValid();
        }

        #endregion
    }
}
