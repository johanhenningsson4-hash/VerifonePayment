using System;
using System.Configuration;
using System.Threading;
using System.Reflection;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VerifonePayment.Test
{
    internal class Program
    {
        #region "Events"

        private static ManualResetEvent statusEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent loginEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent startSessionStatusEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent basketEventStatusEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent paymentCompletedEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent refundCompletedEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent reconciliationEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent transactionQueryEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent printEventReceived = new ManualResetEvent(false);
        private static ManualResetEvent receiptDeliveryEventReceived = new ManualResetEvent(false);

        // State management for workflow validation
        private static bool isSDKInitialized = false;
        private static bool isLoggedIn = false;
        private static bool isSessionStarted = false;
        private static bool hasMerchandise = false;
        private static bool isPaymentEnabled = false;
        private static bool hasCompletedPayment = false;
        private static bool isRefundEnabled = false;
        private static string lastPaymentId = null;
        private static decimal lastPaymentAmount = 0;
        private static Lib.Models.ReceiptWrapper lastReceipt = null;

        // Make verifonePayment accessible to event handlers
        private static Lib.VerifonePayment verifonePayment = null;

        #endregion

        static void Main(string[] args)
        {
            try
            {
                // Use configuration from app.config by default
                verifonePayment = new Lib.VerifonePayment();

                Console.WriteLine("=== Verifone Payment Test Application ===");
                Console.WriteLine($"Configuration: {verifonePayment.Configuration.GetConfigurationSummary()}");
                Console.WriteLine();

                // Subscribe to the event
                verifonePayment.StatusEventOccurred += VerifonePayment_StatusEventOccurred;
                verifonePayment.TransactionEventOccurred += VerifonePayment_TransactionEventOccurred;
                verifonePayment.DeviceVitalsInformationEventOccurred += VerifonePayment_DeviceVitalsInformationEventOccurred;
                verifonePayment.BasketEventOccurred += VerifonePayment_BasketEventOccurred;
                verifonePayment.NotificationEventOccurred += VerifonePayment_NotificationEventOccurred;
                verifonePayment.PaymentCompletedEventOccurred += VerifonePayment_PaymentCompletedEventOccurred;
                verifonePayment.RefundCompletedEventOccurred += VerifonePayment_RefundCompletedEventOccurred;
                verifonePayment.ReconciliationEventOccurred += VerifonePayment_ReconciliationEventOccurred;
                verifonePayment.TransactionQueryEventOccurred += VerifonePayment_TransactionQueryEventOccurred;
                verifonePayment.PrintEventOccurred += VerifonePayment_PrintEventOccurred;
                verifonePayment.ReceiptDeliveryMethodEventOccurred += VerifonePayment_ReceiptDeliveryMethodEventOccurred;
                verifonePayment.UserInputRequestOccurred += VerifonePayment_UserInputRequestOccurred;
                verifonePayment.CommerceEventOccurred += VerifonePayment_CommerceEventOccurred;

                bool running = true;

                while (running)
                {
                    Console.WriteLine("\nChoose an action:");
                    Console.WriteLine($"1. CommunicateWithPaymentSDK {(isSDKInitialized ? "✓" : "○")}");
                    Console.WriteLine($"2. LoginWithCredentials {(isLoggedIn ? "✓" : "○")} {(!isSDKInitialized ? "(Requires SDK)" : "")}");
                    Console.WriteLine($"3. StartSession {(isSessionStarted ? "✓" : "○")} {(!isLoggedIn ? "(Requires Login)" : "")}");
                    Console.WriteLine($"4. AddMerchandise {(hasMerchandise ? "✓" : "○")} {(!isSessionStarted ? "(Requires Session)" : "")}");
                    Console.WriteLine($"5. PaymentTransaction {(isPaymentEnabled ? "✓ ENABLED" : "✗ DISABLED")} {(!hasMerchandise ? "(Requires Merchandise)" : "")}");
                    Console.WriteLine($"6. RemoveMerchandise {(!hasMerchandise ? "(No merchandise)" : "")}");
                    Console.WriteLine($"7. LinkedRefund {(isRefundEnabled ? "✓ ENABLED" : "✗ DISABLED")} {(!hasCompletedPayment ? "(Requires completed payment)" : "")}");
                    Console.WriteLine($"8. UnlinkedRefund {(isSessionStarted ? "○ AVAILABLE" : "✗ DISABLED")} {(!isSessionStarted ? "(Requires Session)" : "")}");
                    Console.WriteLine("9. EndSession");
                    Console.WriteLine("10. ClosePeriod (End of Day)");
                    Console.WriteLine("11. QueryTransactions");
                    Console.WriteLine("12. GetSupportedCapabilities");
                    Console.WriteLine("13. TestReceiptHandling");
                    Console.WriteLine("14. SaveLastReceipt");
                    Console.WriteLine("15. TearDown");
                    Console.WriteLine("16. Run Unit Tests");
                    Console.WriteLine("S. Show Workflow Status");
                    Console.WriteLine("0. Exit");
                    
                    if (hasMerchandise && isPaymentEnabled)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n💳 PAYMENT IS NOW ENABLED - You can process transactions!");
                        Console.ResetColor();
                    }

                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.WriteLine("Initializing Payment SDK...");
                            verifonePayment.CommunicateWithPaymentSDK();
                            WaitForEvent(statusEventReceived, "CommunicateWithPaymentSDK");
                            isSDKInitialized = true;
                            Console.WriteLine("✓ SDK Initialized successfully!");
                            break;

                        case "2":
                            if (!isSDKInitialized)
                            {
                                Console.WriteLine("❌ Please initialize the SDK first (option 1)");
                                break;
                            }
                            Console.WriteLine("Logging in with credentials...");
                            verifonePayment.LoginWithCredentials();
                            WaitForEvent(loginEventReceived, "LoginWithCredentials");
                            isLoggedIn = true;
                            Console.WriteLine("✓ Login successful!");
                            break;

                        case "3":
                            if (!isLoggedIn)
                            {
                                Console.WriteLine("❌ Please login first (option 2)");
                                break;
                            }
                            Console.WriteLine("Starting new session...");
                            verifonePayment.StartSession(Guid.NewGuid().ToString());
                            WaitForEvent(startSessionStatusEventReceived, "StartSession");
                            isSessionStarted = true;
                            Console.WriteLine("✓ Session started successfully!");
                            break;

                        case "4":
                            if (!isSessionStarted)
                            {
                                Console.WriteLine("❌ Please start a session first (option 3)");
                                break;
                            }
                            Console.WriteLine("Adding merchandise to basket...");
                            verifonePayment.AddMerchandise();
                            WaitForEvent(basketEventStatusEventReceived, "AddMerchandise");
                            hasMerchandise = true;
                            isPaymentEnabled = true; // Enable payment after merchandise is added
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✓ Merchandise added successfully!");
                            Console.WriteLine("💳 Payment is now ENABLED - you can process transactions!");
                            Console.ResetColor();
                            break;

                        case "5":
                            if (!isPaymentEnabled || !hasMerchandise)
                            {
                                Console.WriteLine("❌ Payment is not enabled. Please add merchandise first (option 4)");
                                break;
                            }
                            Console.WriteLine("Processing payment transaction...");
                            decimal paymentAmount = (decimal)(new Random().NextDouble() * 99) + 1;
                            string paymentId = System.Guid.NewGuid().ToString();
                            lastPaymentAmount = paymentAmount;
                            lastPaymentId = paymentId;
                            verifonePayment.PaymentTransaction((long)paymentAmount, paymentId, "EUR");
                            WaitForEvent(paymentCompletedEventReceived, "PaymentTransaction");
                            Console.WriteLine("✓ Payment transaction completed!");
                            // Reset state after successful payment
                            hasMerchandise = false;
                            isPaymentEnabled = false;
                            hasCompletedPayment = true;
                            isRefundEnabled = true;
                            break;

                        case "6":
                            if (!hasMerchandise)
                            {
                                Console.WriteLine("❌ No merchandise to remove");
                                break;
                            }
                            Console.WriteLine("Removing merchandise from basket...");
                            verifonePayment.RemoveMerchandise();
                            WaitForEvent(basketEventStatusEventReceived, "RemoveMerchandise");
                            hasMerchandise = false;
                            isPaymentEnabled = false; // Disable payment when merchandise is removed
                            Console.WriteLine("✓ Merchandise removed - Payment is now DISABLED");
                            break;

                        case "7":
                            if (!isRefundEnabled || !hasCompletedPayment)
                            {
                                Console.WriteLine("❌ Linked refund is not available. Please complete a payment first (option 5)");
                                break;
                            }
                            Console.WriteLine("Processing linked refund...");
                            Console.WriteLine($"Refunding payment ID: {lastPaymentId}");
                            Console.WriteLine("1. Full refund");
                            Console.WriteLine("2. Partial refund");
                            Console.Write("Choose refund type (1 or 2): ");
                            var refundChoice = Console.ReadLine();
                            
                            if (refundChoice == "1")
                            {
                                // Full refund
                                verifonePayment.ProcessLinkedRefund(lastPaymentId, null, "EUR");
                                Console.WriteLine($"Processing full refund for payment {lastPaymentId}...");
                            }
                            else if (refundChoice == "2")
                            {
                                // Partial refund
                                Console.Write($"Enter refund amount (max: {lastPaymentAmount:F2}): ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal refundAmount) && refundAmount > 0 && refundAmount <= lastPaymentAmount)
                                {
                                    verifonePayment.ProcessLinkedRefund(lastPaymentId, refundAmount, "EUR");
                                    Console.WriteLine($"Processing partial refund of {refundAmount:F2} for payment {lastPaymentId}...");
                                }
                                else
                                {
                                    Console.WriteLine("❌ Invalid refund amount");
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid choice");
                                break;
                            }
                            WaitForEvent(refundCompletedEventReceived, "LinkedRefund");
                            Console.WriteLine("✓ Linked refund completed!");
                            isRefundEnabled = false; // Disable further refunds for this payment
                            break;

                        case "8":
                            if (!isSessionStarted)
                            {
                                Console.WriteLine("❌ Please start a session first (option 3)");
                                break;
                            }
                            Console.WriteLine("Processing unlinked refund...");
                            Console.Write("Enter refund amount: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal unlinkAmount) && unlinkAmount > 0)
                            {
                                string unlinkRefundId = $"UNLINK-{System.Guid.NewGuid().ToString()}";
                                verifonePayment.ProcessUnlinkedRefund(unlinkAmount, "EUR", unlinkRefundId);
                                Console.WriteLine($"Processing unlinked refund of {unlinkAmount:F2}...");
                                WaitForEvent(refundCompletedEventReceived, "UnlinkedRefund");
                                Console.WriteLine("✓ Unlinked refund completed!");
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid refund amount");
                            }
                            break;

                        case "9":
                            Console.WriteLine("Ending session...");
                            verifonePayment.EndSession();
                            WaitForEvent(statusEventReceived, "EndSession");
                            // Reset session state
                            isSessionStarted = false;
                            hasMerchandise = false;
                            isPaymentEnabled = false;
                            hasCompletedPayment = false;
                            isRefundEnabled = false;
                            lastPaymentId = null;
                            lastPaymentAmount = 0;
                            Console.WriteLine("✓ Session ended");
                            break;
                            
                        case "10":
                            if (!isSessionStarted)
                            {
                                Console.WriteLine("❌ Please start a session first (option 3)");
                                break;
                            }
                            Console.WriteLine("Closing period (End of Day)...");
                            try
                            {
                                if (verifonePayment.IsReportingCapable("CLOSE_PERIOD_CAPABILITY"))
                                {
                                    verifonePayment.ClosePeriod();
                                    WaitForEvent(reconciliationEventReceived, "ClosePeriod");
                                    Console.WriteLine("✓ Period closed successfully!");
                                }
                                else
                                {
                                    Console.WriteLine("⚠️ CLOSE_PERIOD_CAPABILITY not supported by current Payment App or Host");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error closing period: {ex.Message}");
                            }
                            break;

                        case "11":
                            if (!isSessionStarted)
                            {
                                Console.WriteLine("❌ Please start a session first (option 3)");
                                break;
                            }
                            Console.WriteLine("Querying transactions...");
                            try
                            {
                                if (verifonePayment.IsReportingCapable("TRANSACTION_QUERY_CAPABILITY"))
                                {
                                    Console.WriteLine("1. Query all transactions");
                                    Console.WriteLine("2. Query SAF (offline) transactions");
                                    Console.WriteLine("3. Query with time range");
                                    Console.Write("Choose query type (1, 2, or 3): ");
                                    var queryChoice = Console.ReadLine();

                                    switch (queryChoice)
                                    {
                                        case "1":
                                            verifonePayment.QueryTransactions();
                                            break;
                                        case "2":
                                            // Query SAF transactions from 1 hour ago
                                            long oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds();
                                            verifonePayment.QuerySAFTransactions(oneHourAgo);
                                            break;
                                        case "3":
                                            Console.Write("Enter start time (Unix timestamp, or press Enter for 1 hour ago): ");
                                            var startInput = Console.ReadLine();
                                            long startTime = string.IsNullOrEmpty(startInput) ? 
                                                DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds() :
                                                long.Parse(startInput);
                                            verifonePayment.QueryTransactions(startTime: startTime);
                                            break;
                                        default:
                                            Console.WriteLine("❌ Invalid choice");
                                            break;
                                    }
                                    if (queryChoice == "1" || queryChoice == "2" || queryChoice == "3")
                                    {
                                        WaitForEvent(transactionQueryEventReceived, "QueryTransactions");
                                        Console.WriteLine("✓ Transaction query completed!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("⚠️ TRANSACTION_QUERY_CAPABILITY not supported by current Payment App or Host");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error querying transactions: {ex.Message}");
                            }
                            break;

                        case "12":
                            Console.WriteLine("Checking supported capabilities...");
                            try
                            {
                                var capabilities = verifonePayment.GetSupportedCapabilities();
                                Console.WriteLine("\n=== Supported Capabilities ===");
                                foreach (var capability in capabilities)
                                {
                                    string status = capability.Value ? "✓ SUPPORTED" : "✗ NOT SUPPORTED";
                                    Console.WriteLine($"{capability.Key}: {status}");
                                }
                                Console.WriteLine("===============================");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error checking capabilities: {ex.Message}");
                            }
                            break;

                        case "13":
                            Console.WriteLine("Testing receipt handling capabilities...");
                            try
                            {
                                Console.WriteLine($"Printing supported: {(verifonePayment.IsPrintingSupported() ? "✓" : "✗")}");
                                
                                if (lastReceipt != null)
                                {
                                    Console.WriteLine("\n--- Last Receipt Information ---");
                                    Console.WriteLine(lastReceipt.GetReceiptSummary());
                                    Console.WriteLine();

                                    var validation = verifonePayment.ValidateReceipt(lastReceipt);
                                    Console.WriteLine(validation.GetCompactSummary());
                                    
                                    if (validation.HasIssues || validation.HasWarnings)
                                    {
                                        Console.WriteLine("\nValidation Details:");
                                        Console.WriteLine(validation.GetDetailedReport());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("⚠️ No receipt available. Complete a payment transaction first.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error testing receipt handling: {ex.Message}");
                            }
                            break;

                        case "14":
                            if (lastReceipt == null)
                            {
                                Console.WriteLine("❌ No receipt available to save. Complete a payment transaction first.");
                                break;
                            }
                            Console.WriteLine("Saving last receipt...");
                            try
                            {
                                Console.WriteLine("Choose format:");
                                Console.WriteLine("1. Plain text");
                                Console.WriteLine("2. HTML");
                                Console.WriteLine("3. Full metadata");
                                Console.Write("Select format (1, 2, or 3): ");
                                var formatChoice = Console.ReadLine();

                                string format = "txt";
                                string extension = "txt";
                                switch (formatChoice)
                                {
                                    case "2":
                                        format = "html";
                                        extension = "html";
                                        break;
                                    case "3":
                                        format = "metadata";
                                        extension = "txt";
                                        break;
                                    case "1":
                                    default:
                                        format = "txt";
                                        extension = "txt";
                                        break;
                                }

                                string fileName = $"receipt_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
                                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                                if (verifonePayment.SaveReceipt(lastReceipt, filePath, format))
                                {
                                    Console.WriteLine($"✓ Receipt saved to: {filePath}");
                                }
                                else
                                {
                                    Console.WriteLine("❌ Failed to save receipt");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Error saving receipt: {ex.Message}");
                            }
                            break;

                        case "15":
                            Console.WriteLine("Tearing down SDK...");
                            verifonePayment.TearDown();
                            WaitForEvent(statusEventReceived, "TearDown");
                            // Reset all state
                            isSDKInitialized = false;
                            isLoggedIn = false;
                            isSessionStarted = false;
                            hasMerchandise = false;
                            isPaymentEnabled = false;
                            hasCompletedPayment = false;
                            isRefundEnabled = false;
                            lastPaymentId = null;
                            lastPaymentAmount = 0;
                            lastReceipt = null;
                            Console.WriteLine("✓ SDK teardown completed");
                            break;

                        case "16":
                            RunUnitTests();
                            break;

                        case "S":
                        case "s":
                            DisplayWorkflowStatus();
                            break;

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        #region "Private Methods"

        /// <summary>
        /// Event wait handler
        /// </summary>
        /// <param name="eventHandle">The event handle</param>
        /// <param name="actionName">The action name</param>
        private static void WaitForEvent(ManualResetEvent eventHandle, string actionName)
        {
            Console.WriteLine($"{actionName}: Waiting for status event...");
            eventHandle.WaitOne();
            eventHandle.Reset();
        }

        /// <summary>
        /// Displays the current workflow status
        /// </summary>
        private static void DisplayWorkflowStatus()
        {
            Console.WriteLine("\n=== Current Workflow Status ===");
            Console.WriteLine($"SDK Initialized: {(isSDKInitialized ? "✓" : "✗")}");
            Console.WriteLine($"Logged In: {(isLoggedIn ? "✓" : "✗")}");
            Console.WriteLine($"Session Started: {(isSessionStarted ? "✓" : "✗")}");
            Console.WriteLine($"Has Merchandise: {(hasMerchandise ? "✓" : "✗")}");
            Console.WriteLine($"Payment Enabled: {(isPaymentEnabled ? "✓" : "✗")}");
            Console.WriteLine($"Has Completed Payment: {(hasCompletedPayment ? "✓" : "✗")}");
            Console.WriteLine($"Refund Enabled: {(isRefundEnabled ? "✓" : "✗")}");
            Console.WriteLine($"Has Receipt: {(lastReceipt != null ? "✓" : "✗")}");
            
            if (!string.IsNullOrEmpty(lastPaymentId))
            {
                Console.WriteLine($"Last Payment ID: {lastPaymentId}");
                Console.WriteLine($"Last Payment Amount: {lastPaymentAmount:F2} EUR");
            }

            if (lastReceipt != null)
            {
                Console.WriteLine($"Receipt Type: {lastReceipt.ReceiptType}");
                Console.WriteLine($"Receipt Valid: {(lastReceipt.IsValid() ? "✓" : "✗")}");
            }
            
            if (isPaymentEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("💳 READY FOR PAYMENT PROCESSING");
                Console.ResetColor();
            }
            else if (isRefundEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("🔄 READY FOR REFUND PROCESSING");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  Payment and refund processing are currently disabled");
                Console.ResetColor();
            }
            Console.WriteLine("===============================\n");
        }

        /// <summary>
        /// Event handler for DeviceVitalsInformationEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_DeviceVitalsInformationEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
            statusEventReceived.Set();
        }

        /// <summary>
        /// Event handler for TransactionEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_TransactionEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            if (e.Type == Lib.Enums.EventType.SESSION_STARTED && e.Status == "0")
            {
                startSessionStatusEventReceived.Set();
            }
            if (e.Type == Lib.Enums.EventType.SESSION_ENDED && e.Status == "0")
            {
                statusEventReceived.Set();
            }
            if (e.Type == Lib.Enums.EventType.LOGIN_COMPLETED)
            {
                loginEventReceived.Set();

                if (e.Status == "-20")
                    statusEventReceived.Set();
            }
        }

        /// <summary>
        /// Event handler for StatusEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_StatusEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            statusEventReceived.Set();
        }

        /// <summary>
        /// Event handler for BasketEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_BasketEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
            basketEventStatusEventReceived.Set();
        }

        /// <summary>
        /// Event handler for NotificationEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_NotificationEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
        }

        /// <summary>
        /// Event handler for PaymentCompletedEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_PaymentCompletedEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            if (e.Type == Lib.Enums.EventType.NOTIFICATION_EVENT && e.Message == "Transaction Completed")
                paymentCompletedEventReceived.Set();
            if (e.Type == Lib.Enums.EventType.TRANSACTION_PAYMENT_COMPLETED)
                paymentCompletedEventReceived.Set();
        }

        /// <summary>
        /// Event handler for RefundCompletedEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_RefundCompletedEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Refund Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            // Set the refund completed event for any refund completion
            refundCompletedEventReceived.Set();
        }

        /// <summary>
        /// Event handler for CommerceEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_CommerceEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            if (e.Type == Lib.Enums.EventType.STATUS_ERROR && e.Status == "-20")
                startSessionStatusEventReceived.Set();
        }

        /// <summary>
        /// Event handler for ReconciliationEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_ReconciliationEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Reconciliation Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
            
            // Parse reconciliation event details
            if (e.Status == "0") // Success
            {
                Console.WriteLine("✓ Reconciliation completed successfully");
            }
            else
            {
                Console.WriteLine($"⚠️ Reconciliation issue: Status {e.Status}");
            }
            
            reconciliationEventReceived.Set();
        }

        /// <summary>
        /// Event handler for TransactionQueryEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_TransactionQueryEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Transaction Query Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
            
            if (e.Status == "0") // Success
            {
                Console.WriteLine("✓ Transaction query completed successfully");
            }
            else
            {
                Console.WriteLine($"⚠️ Transaction query issue: Status {e.Status}");
            }
            
            transactionQueryEventReceived.Set();
        }

        /// <summary>
        /// Event handler for PrintEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_PrintEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Print Event Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
            
            if (e.Status == "0") // Success
            {
                Console.WriteLine("✓ Print event completed successfully");
            }
            else
            {
                Console.WriteLine($"⚠️ Print event issue: Status {e.Status}");
            }
            
            printEventReceived.Set();
        }

        /// <summary>
        /// Event handler for ReceiptDeliveryMethodEventOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_ReceiptDeliveryMethodEventOccurred(object sender, Lib.Models.PaymentEventArgs e)
        {
            Console.WriteLine($"   - Receipt Delivery Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");

            if (e.Status == "0") // Success
            {
                Console.WriteLine("✓ Receipt delivery method completed successfully");
            }
            else
            {
                Console.WriteLine($"⚠️ Receipt delivery issue: Status {e.Status}");
            }

            receiptDeliveryEventReceived.Set();
        }

        /// <summary>
        /// Event handler for UserInputRequestOccurred
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private static void VerifonePayment_UserInputRequestOccurred(object sender, Lib.Models.UserInputRequestEventArgs e)
        {
            Console.WriteLine($"\n🔔 USER INPUT REQUEST RECEIVED 🔔");
            Console.WriteLine($"Input Type: {e.Request.InputType}");
            Console.WriteLine($"Message: {e.Request.Message}");
            Console.WriteLine($"Prompt: {e.Request.Prompt}");
            Console.WriteLine($"Requires Input: {e.Request.RequiresInput}");
            Console.WriteLine($"Is Masked: {e.Request.IsMasked}");

            try
            {
                if (e.Request.RequiresInput)
                {
                    // Handle different input types
                    string inputType = e.Request.InputType.ToUpperInvariant();

                    if (inputType.Contains("TEXT") || inputType.Contains("STRING"))
                    {
                        Console.Write($"Enter text response: ");
                        string textInput = Console.ReadLine();

                        if (verifonePayment.RespondToUserInput(e.Request, textInput))
                        {
                            Console.WriteLine("✓ Text response sent successfully");
                        }
                        else
                        {
                            Console.WriteLine("❌ Failed to send text response");
                        }
                    }
                    else if (inputType.Contains("NUMERIC") || inputType.Contains("AMOUNT"))
                    {
                        Console.Write($"Enter numeric response: ");
                        string numericInput = Console.ReadLine();

                        if (decimal.TryParse(numericInput, out decimal numericValue))
                        {
                            if (verifonePayment.RespondToUserInput(e.Request, numericValue))
                            {
                                Console.WriteLine("✓ Numeric response sent successfully");
                            }
                            else
                            {
                                Console.WriteLine("❌ Failed to send numeric response");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid numeric input, sending 0");
                            verifonePayment.RespondToUserInput(e.Request, 0m);
                        }
                    }
                    else if (inputType.Contains("SELECT") || inputType.Contains("CHOICE"))
                    {
                        Console.WriteLine("Available options:");
                        for (int i = 0; i < e.Request.Options.Count; i++)
                        {
                            Console.WriteLine($"  {i}: {e.Request.Options[i]}");
                        }

                        if (e.Request.Options.Count == 0)
                        {
                            Console.WriteLine("  0: Default Option");
                        }

                        Console.Write($"Select option (0-{Math.Max(0, e.Request.Options.Count - 1)}): ");
                        string selectionInput = Console.ReadLine();

                        if (int.TryParse(selectionInput, out int selectedIndex))
                        {
                            if (verifonePayment.RespondToUserInput(e.Request, selectedIndex))
                            {
                                Console.WriteLine("✓ Selection response sent successfully");
                            }
                            else
                            {
                                Console.WriteLine("❌ Failed to send selection response");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid selection, sending 0");
                            verifonePayment.RespondToUserInput(e.Request, 0);
                        }
                    }
                    else if (inputType.Contains("CONFIRM") || inputType.Contains("ACKNOWLEDGE"))
                    {
                        Console.Write($"Confirm (Y/N): ");
                        string confirmInput = Console.ReadLine();
                        bool confirmed = confirmInput?.ToUpperInvariant().StartsWith("Y") == true;

                        if (verifonePayment.RespondToUserInput(e.Request, confirmed))
                        {
                            Console.WriteLine($"✓ Confirmation response sent: {(confirmed ? "YES" : "NO")}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Failed to send confirmation response");
                        }
                    }
                    else
                    {
                        // Unknown input type - provide generic confirmation
                        Console.WriteLine($"⚠️ Unknown input type '{e.Request.InputType}', sending default confirmation");
                        verifonePayment.RespondToUserInput(e.Request, true);
                    }
                }
                else
                {
                    // Display-only message - just acknowledge
                    Console.WriteLine("📋 Display-only message - automatically acknowledging");
                    verifonePayment.RespondToUserInput(e.Request, true);
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error handling user input request: {ex.Message}");

                // Try to send a default response to avoid blocking the payment flow
                try
                {
                    verifonePayment.RespondToUserInput(e.Request, true);
                    e.Handled = true;
                }
                catch
                {
                    Console.WriteLine("❌ Failed to send default response");
                }
            }

            Console.WriteLine("🔔 USER INPUT REQUEST COMPLETED 🔔\n");
        }

        /// <summary>
        /// Runs all unit tests using reflection
        /// </summary>
        private static void RunUnitTests()
        {
            Console.WriteLine("\n=== Running Unit Tests ===");
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            
            int totalTests = 0;
            int passedTests = 0;
            int failedTests = 0;
            
            foreach (Type type in types)
            {
                if (type.GetCustomAttribute<TestClassAttribute>() != null)
                {
                    Console.WriteLine($"\nRunning tests in class: {type.Name}");
                    
                    MethodInfo[] methods = type.GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetCustomAttribute<TestMethodAttribute>() != null)
                        {
                            totalTests++;
                            Console.Write($"  Running {method.Name}... ");
                            
                            try
                            {
                                object instance = Activator.CreateInstance(type);
                                method.Invoke(instance, null);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("PASSED");
                                Console.ResetColor();
                                passedTests++;
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("FAILED");
                                Console.ResetColor();
                                Console.WriteLine($"    Error: {ex.InnerException?.Message ?? ex.Message}");
                                failedTests++;
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine($"\n=== Test Results ===");
            Console.WriteLine($"Total tests: {totalTests}");
            Console.WriteLine($"Passed: {passedTests}");
            Console.WriteLine($"Failed: {failedTests}");
            
            if (failedTests == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All tests passed!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{failedTests} test(s) failed!");
            }
            Console.ResetColor();
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        #endregion
    }
}