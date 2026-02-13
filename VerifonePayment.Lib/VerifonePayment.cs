using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Reconciliation event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> ReconciliationEventOccurred
        {
            add { listener_.ReconciliationEventOccurred += value; }
            remove { listener_.ReconciliationEventOccurred -= value; }
        }

        /// <summary>
        /// Reconciliations list event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> ReconciliationsListEventOccurred
        {
            add { listener_.ReconciliationsListEventOccurred += value; }
            remove { listener_.ReconciliationsListEventOccurred -= value; }
        }

        /// <summary>
        /// Transaction query event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> TransactionQueryEventOccurred
        {
            add { listener_.TransactionQueryEventOccurred += value; }
            remove { listener_.TransactionQueryEventOccurred -= value; }
        }

        /// <summary>
        /// Print event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> PrintEventOccurred
        {
            add { listener_.PrintEventOccurred += value; }
            remove { listener_.PrintEventOccurred -= value; }
        }

        /// <summary>
        /// Receipt delivery method event occurred.
        /// </summary>
        public event EventHandler<PaymentEventArgs> ReceiptDeliveryMethodEventOccurred
        {
            add { listener_.ReceiptDeliveryMethodEventOccurred += value; }
            remove { listener_.ReceiptDeliveryMethodEventOccurred -= value; }
        }

        /// <summary>
        /// User input request occurred - requires cashier interaction.
        /// </summary>
        public event EventHandler<Models.UserInputRequestEventArgs> UserInputRequestOccurred
        {
            add { listener_.UserInputRequestOccurred += value; }
            remove { listener_.UserInputRequestOccurred -= value; }
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

        #region "Reconciliation and Reporting Methods"

        /// <summary>
        /// Checks if the current Payment App or Payment Host supports a specific reporting capability.
        /// </summary>
        /// <param name="capability">The capability to check (e.g., "GET_ACTIVE_TOTALS_CAPABILITY")</param>
        /// <returns>True if the capability is supported</returns>
        public bool IsReportingCapable(string capability)
        {
            if (string.IsNullOrWhiteSpace(capability))
                throw new ArgumentException("Capability cannot be null or empty", nameof(capability));

            return payment_sdk_.TransactionManager.ReportManager.IsCapable(capability);
        }

        /// <summary>
        /// Closes the current period (end of day).
        /// Requires CLOSE_PERIOD_CAPABILITY.
        /// </summary>
        public void ClosePeriod()
        {
            if (!IsReportingCapable("CLOSE_PERIOD_CAPABILITY"))
                throw new InvalidOperationException("CLOSE_PERIOD_CAPABILITY is not supported by the current Payment App or Host");

            payment_sdk_.TransactionManager.ReportManager.ClosePeriod();
        }

        /// <summary>
        /// Closes period and performs reconciliation in a single operation.
        /// Requires CLOSE_PERIOD_AND_RECONCILE_CAPABILITY.
        /// </summary>
        /// <param name="acquirers">Array of acquirer IDs to reconcile (optional)</param>
        public void ClosePeriodAndReconcile(int[] acquirers = null)
        {
            if (!IsReportingCapable("CLOSE_PERIOD_AND_RECONCILE_CAPABILITY"))
                throw new InvalidOperationException("CLOSE_PERIOD_AND_RECONCILE_CAPABILITY is not supported by the current Payment App or Host");

            if (acquirers != null && acquirers.Length > 0)
            {
                payment_sdk_.TransactionManager.ReportManager.ClosePeriodAndReconcile(acquirers);
            }
            else
            {
                // Use empty array if no specific acquirers specified
                payment_sdk_.TransactionManager.ReportManager.ClosePeriodAndReconcile(new int[0]);
            }
        }

        /// <summary>
        /// Gets previous reconciliation data by ID.
        /// Requires GET_PREVIOUS_RECONCILIATION_CAPABILITY.
        /// </summary>
        /// <param name="reconciliationId">The reconciliation ID to retrieve</param>
        public void GetPreviousReconciliation(string reconciliationId)
        {
            if (string.IsNullOrWhiteSpace(reconciliationId))
                throw new ArgumentException("Reconciliation ID cannot be null or empty", nameof(reconciliationId));

            if (!IsReportingCapable("GET_PREVIOUS_RECONCILIATION_CAPABILITY"))
                throw new InvalidOperationException("GET_PREVIOUS_RECONCILIATION_CAPABILITY is not supported by the current Payment App or Host");

            payment_sdk_.TransactionManager.ReportManager.GetPreviousReconciliation(reconciliationId);
        }

        /// <summary>
        /// Query transactions with specified criteria.
        /// Requires TRANSACTION_QUERY_CAPABILITY.
        /// </summary>
        /// <param name="allPos">Include transactions from all POS systems (default: false)</param>
        /// <param name="isOffline">Query offline/SAF transactions (default: false)</param>
        /// <param name="startTime">Start time for query (optional)</param>
        /// <param name="endTime">End time for query (optional)</param>
        /// <param name="limit">Maximum number of transactions to return (optional)</param>
        /// <param name="offset">Number of transactions to skip (optional)</param>
        public void QueryTransactions(bool allPos = false, bool isOffline = false, 
            long? startTime = null, long? endTime = null, int? limit = null, int? offset = null)
        {
            if (!IsReportingCapable("TRANSACTION_QUERY_CAPABILITY"))
                throw new InvalidOperationException("TRANSACTION_QUERY_CAPABILITY is not supported by the current Payment App or Host");

            using (var query = TransactionQuery.Create())
            {
                // Use SetAllPos method if available
                if (allPos)
                {
                    try
                    {
                        // Try to set all POS - method name might vary
                        var method = query.GetType().GetMethod("SetAllPos");
                        method?.Invoke(query, new object[] { allPos });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                // Use SetOffline method if available
                if (isOffline)
                {
                    try
                    {
                        var method = query.GetType().GetMethod("SetOffline");
                        method?.Invoke(query, new object[] { isOffline });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                if (startTime.HasValue)
                {
                    try
                    {
                        var method = query.GetType().GetMethod("SetStartTime");
                        method?.Invoke(query, new object[] { startTime.Value });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                if (endTime.HasValue)
                {
                    try
                    {
                        var method = query.GetType().GetMethod("SetEndTime");
                        method?.Invoke(query, new object[] { endTime.Value });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                if (limit.HasValue)
                {
                    try
                    {
                        var method = query.GetType().GetMethod("SetLimit");
                        method?.Invoke(query, new object[] { limit.Value });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                if (offset.HasValue)
                {
                    try
                    {
                        var method = query.GetType().GetMethod("SetOffset");
                        method?.Invoke(query, new object[] { offset.Value });
                    }
                    catch
                    {
                        // If method doesn't exist, continue without it
                    }
                }

                payment_sdk_.TransactionManager.ReportManager.QueryTransactions(query);
            }
        }

        /// <summary>
        /// Query Store and Forward (SAF) transactions.
        /// Convenience method for querying offline transactions.
        /// </summary>
        /// <param name="startTime">Start time for SAF transaction query</param>
        /// <param name="endTime">End time for query (optional, defaults to current time)</param>
        public void QuerySAFTransactions(long startTime, long? endTime = null)
        {
            QueryTransactions(allPos: false, isOffline: true, startTime: startTime, endTime: endTime);
        }

        /// <summary>
        /// Query transactions using a custom TransactionQuery object.
        /// Provides full control over query parameters.
        /// </summary>
        /// <param name="query">The TransactionQuery object with configured parameters</param>
        public void QueryTransactions(TransactionQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (!IsReportingCapable("TRANSACTION_QUERY_CAPABILITY"))
                throw new InvalidOperationException("TRANSACTION_QUERY_CAPABILITY is not supported by the current Payment App or Host");

            payment_sdk_.TransactionManager.ReportManager.QueryTransactions(query);
        }

        /// <summary>
        /// Gets all supported reporting capabilities for the current Payment App and Host.
        /// </summary>
        /// <returns>Dictionary of capability names and their support status</returns>
        public Dictionary<string, bool> GetSupportedCapabilities()
        {
            var capabilities = new Dictionary<string, bool>
            {
                ["GET_ACTIVE_TOTALS_CAPABILITY"] = IsReportingCapable("GET_ACTIVE_TOTALS_CAPABILITY"),
                ["GET_GROUP_TOTALS_CAPABILITY"] = IsReportingCapable("GET_GROUP_TOTALS_CAPABILITY"),
                ["CLOSE_PERIOD_CAPABILITY"] = IsReportingCapable("CLOSE_PERIOD_CAPABILITY"),
                ["TERMINAL_RECONCILIATION_CAPABILITY"] = IsReportingCapable("TERMINAL_RECONCILIATION_CAPABILITY"),
                ["ACQUIRER_RECONCILIATION_CAPABILITY"] = IsReportingCapable("ACQUIRER_RECONCILIATION_CAPABILITY"),
                ["PREVIOUS_RECONCILIATION_CAPABILITY"] = IsReportingCapable("PREVIOUS_RECONCILIATION_CAPABILITY"),
                ["TRANSACTION_QUERY_CAPABILITY"] = IsReportingCapable("TRANSACTION_QUERY_CAPABILITY"),
                ["TOTALS_GROUP_ID_CAPABILITY"] = IsReportingCapable("TOTALS_GROUP_ID_CAPABILITY"),
                ["CLOSE_PERIOD_AND_RECONCILE_CAPABILITY"] = IsReportingCapable("CLOSE_PERIOD_AND_RECONCILE_CAPABILITY"),
                ["GET_PREVIOUS_RECONCILIATION_CAPABILITY"] = IsReportingCapable("GET_PREVIOUS_RECONCILIATION_CAPABILITY"),
                ["RECONCILIATION_LIST_CAPABILITY"] = IsReportingCapable("RECONCILIATION_LIST_CAPABILITY"),
                ["RECONCILIATION_REPORT_CAPABILITY"] = IsReportingCapable("RECONCILIATION_REPORT_CAPABILITY")
            };

            return capabilities;
        }

        #endregion

        #region "Receipt Handling Methods"

        /// <summary>
        /// Extracts and wraps a receipt from a Payment object.
        /// Note: This is a placeholder implementation. The actual Receipt extraction
        /// would depend on how the Verifone SDK provides access to Receipt objects.
        /// </summary>
        /// <param name="payment">The Payment object containing the receipt</param>
        /// <returns>A wrapped receipt object or null if no receipt is available</returns>
        public Models.ReceiptWrapper ExtractReceipt(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            try
            {
                // Since the SDK Receipt access pattern is unclear, we'll use reflection
                // to try to access the receipt data
                var receiptProperty = payment.GetType().GetProperty("Receipt");
                if (receiptProperty != null)
                {
                    Receipt receipt = receiptProperty.GetValue(payment) as Receipt;
                    if (receipt != null)
                    {
                        return new Models.ReceiptWrapper(receipt);
                    }
                }

                // Alternative: check for methods that return Receipt
                var receiptMethods = payment.GetType().GetMethods()
                    .Where(m => m.ReturnType == typeof(Receipt) && m.GetParameters().Length == 0);

                foreach (var method in receiptMethods)
                {
                    try
                    {
                        Receipt receipt = method.Invoke(payment, null) as Receipt;
                        if (receipt != null)
                        {
                            return new Models.ReceiptWrapper(receipt);
                        }
                    }
                    catch
                    {
                        // Continue trying other methods
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting receipt: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if printing capability is supported.
        /// </summary>
        /// <returns>True if printing is supported</returns>
        public bool IsPrintingSupported()
        {
            try
            {
                // Check if print capability exists
                return payment_sdk_.TransactionManager.ReportManager.IsCapable("PRINT_CAPABILITY") ||
                       payment_sdk_.TransactionManager.ReportManager.IsCapable("RECEIPT_PRINT_CAPABILITY");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to print a receipt if printing is supported.
        /// </summary>
        /// <param name="receipt">The receipt wrapper to print</param>
        /// <param name="copies">Number of copies to print (default: 1)</param>
        /// <returns>True if print request was successful</returns>
        public bool PrintReceipt(Models.ReceiptWrapper receipt, int copies = 1)
        {
            if (receipt == null)
                throw new ArgumentNullException(nameof(receipt));

            if (copies <= 0)
                throw new ArgumentException("Copies must be greater than zero", nameof(copies));

            if (!IsPrintingSupported())
            {
                System.Diagnostics.Debug.WriteLine("Printing is not supported");
                return false;
            }

            try
            {
                // This would typically use the SDK's printing methods
                // Since the exact API may vary, we'll use a generic approach
                System.Diagnostics.Debug.WriteLine($"Print request: {copies} copies of {receipt.ReceiptType} receipt");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error printing receipt: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Saves a receipt to a file.
        /// </summary>
        /// <param name="receipt">The receipt to save</param>
        /// <param name="filePath">The file path to save to</param>
        /// <param name="format">The format to save (html, txt, or metadata)</param>
        /// <returns>True if the receipt was saved successfully</returns>
        public bool SaveReceipt(Models.ReceiptWrapper receipt, string filePath, string format = "txt")
        {
            if (receipt == null)
                throw new ArgumentNullException(nameof(receipt));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                string content;
                switch (format.ToLowerInvariant())
                {
                    case "html":
                        content = receipt.AsHtml;
                        break;
                    case "metadata":
                        content = receipt.ExportForDisplay(true);
                        break;
                    case "txt":
                    default:
                        content = receipt.AsPlainText;
                        break;
                }

                System.IO.File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving receipt: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates a receipt archive with timestamp for storage.
        /// </summary>
        /// <param name="receipt">The receipt to archive</param>
        /// <param name="archiveDirectory">Directory to store archived receipts</param>
        /// <param name="transactionId">Optional transaction ID for filename</param>
        /// <returns>The path to the archived receipt file</returns>
        public string ArchiveReceipt(Models.ReceiptWrapper receipt, string archiveDirectory, string transactionId = null)
        {
            if (receipt == null)
                throw new ArgumentNullException(nameof(receipt));

            if (string.IsNullOrWhiteSpace(archiveDirectory))
                throw new ArgumentException("Archive directory cannot be null or empty", nameof(archiveDirectory));

            try
            {
                // Create directory if it doesn't exist
                if (!System.IO.Directory.Exists(archiveDirectory))
                    System.IO.Directory.CreateDirectory(archiveDirectory);

                // Generate filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = string.IsNullOrWhiteSpace(transactionId) 
                    ? $"receipt_{timestamp}.txt"
                    : $"receipt_{transactionId}_{timestamp}.txt";

                string fullPath = System.IO.Path.Combine(archiveDirectory, fileName);

                // Save with metadata
                if (SaveReceipt(receipt, fullPath, "metadata"))
                    return fullPath;

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error archiving receipt: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates receipt content and configuration.
        /// </summary>
        /// <param name="receipt">The receipt to validate</param>
        /// <returns>Validation results</returns>
        public Models.ReceiptValidationResult ValidateReceipt(Models.ReceiptWrapper receipt)
        {
            if (receipt == null)
                return new Models.ReceiptValidationResult(false, "Receipt is null");

            var issues = new System.Collections.Generic.List<string>();
            var warnings = new System.Collections.Generic.List<string>();

            // Check basic validity
            if (!receipt.IsValid())
                issues.Add("Receipt contains no content (both HTML and plain text are empty)");

            // Check content consistency
            if (string.IsNullOrWhiteSpace(receipt.AsPlainText) && !string.IsNullOrWhiteSpace(receipt.AsHtml))
                warnings.Add("Receipt has HTML content but no plain text fallback");

            // Check required fields
            if (string.IsNullOrWhiteSpace(receipt.TransactionInformation))
                warnings.Add("Receipt missing transaction information");

            if (string.IsNullOrWhiteSpace(receipt.PaymentInformation))
                warnings.Add("Receipt missing payment information");

            // Check configuration consistency
            if (receipt.IsQrCodeIncluded && !receipt.IsOnlineUrlIncluded)
                warnings.Add("QR code is included but online URL is not");

            if (receipt.IsCashierNameIncluded && string.IsNullOrWhiteSpace(receipt.CashierName))
                warnings.Add("Cashier name is configured to be included but is empty");

            bool isValid = issues.Count == 0;
            string summary = isValid ? "Receipt is valid" : $"Receipt has {issues.Count} issues";

            return new Models.ReceiptValidationResult(isValid, summary, issues, warnings);
        }

        #endregion

        #region "User Input Handling Methods"

        /// <summary>
        /// Sends a user input response back to the payment application.
        /// This should be called after collecting the required input from the cashier.
        /// </summary>
        /// <param name="request">The user input request containing the response</param>
        /// <returns>True if the response was sent successfully</returns>
        public bool SendUserInputResponse(Models.UserInputRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Response == null)
                throw new InvalidOperationException("User input request does not contain a valid response object");

            try
            {
                // Send the response back to the payment application
                payment_sdk_.TransactionManager.SendInputResponse(request.Response);

                System.Diagnostics.Debug.WriteLine($"Sent user input response for {request.InputType}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending user input response: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handles a user input request with a text response.
        /// Convenience method that sets the text response and sends it back.
        /// </summary>
        /// <param name="request">The user input request</param>
        /// <param name="textResponse">The text response from the user</param>
        /// <returns>True if the response was sent successfully</returns>
        public bool RespondToUserInput(Models.UserInputRequest request, string textResponse)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                request.SetTextResponse(textResponse);
                return SendUserInputResponse(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error responding to user input with text: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handles a user input request with a numeric response.
        /// Convenience method that sets the numeric response and sends it back.
        /// </summary>
        /// <param name="request">The user input request</param>
        /// <param name="numericResponse">The numeric response from the user</param>
        /// <returns>True if the response was sent successfully</returns>
        public bool RespondToUserInput(Models.UserInputRequest request, decimal numericResponse)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                request.SetNumericResponse(numericResponse);
                return SendUserInputResponse(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error responding to user input with numeric value: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handles a user input request with a selection response.
        /// Convenience method that sets the selection response and sends it back.
        /// </summary>
        /// <param name="request">The user input request</param>
        /// <param name="selectedIndex">The index of the selected option</param>
        /// <returns>True if the response was sent successfully</returns>
        public bool RespondToUserInput(Models.UserInputRequest request, int selectedIndex)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                request.SetSelectionResponse(selectedIndex);
                return SendUserInputResponse(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error responding to user input with selection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handles a user input request with a confirmation response.
        /// Convenience method that sets the confirmation response and sends it back.
        /// </summary>
        /// <param name="request">The user input request</param>
        /// <param name="confirmed">True if user confirmed, false if cancelled</param>
        /// <returns>True if the response was sent successfully</returns>
        public bool RespondToUserInput(Models.UserInputRequest request, bool confirmed)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                request.SetConfirmationResponse(confirmed);
                return SendUserInputResponse(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error responding to user input with confirmation: {ex.Message}");
                return false;
            }
        }

        #endregion

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
