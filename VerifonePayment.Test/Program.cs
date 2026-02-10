using System;
using System.Configuration;
using System.Threading;
using System.Reflection;
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

        #endregion

        static void Main(string[] args)
        {
            try
            {
                // Use configuration from app.config by default
                var verifonePayment = new Lib.VerifonePayment();
                
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
                verifonePayment.CommerceEventOccurred += VerifonePayment_CommerceEventOccurred;

                bool running = true;

                while (running)
                {
                    Console.WriteLine("Choose an action:");
                    Console.WriteLine("1. CommunicateWithPaymentSDK");
                    Console.WriteLine("2. LoginWithCredentials");
                    Console.WriteLine("3. StartSession");
                    Console.WriteLine("4. AddMerchandise");
                    Console.WriteLine("5. PaymentTransaction");
                    Console.WriteLine("6. RemoveMerchandise");
                    Console.WriteLine("7. EndSession");
                    Console.WriteLine("8. TearDown");
                    Console.WriteLine("9. Run Unit Tests");
                    Console.WriteLine("0. Exit");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            verifonePayment.CommunicateWithPaymentSDK();
                            WaitForEvent(statusEventReceived, "CommunicateWithPaymentSDK");
                            break;

                        case "2":
                            verifonePayment.LoginWithCredentials();
                            WaitForEvent(loginEventReceived, "LoginWithCredentials");
                            break;

                        case "3":
                            verifonePayment.StartSession(Guid.NewGuid().ToString());
                            WaitForEvent(startSessionStatusEventReceived, "StartSession");
                            break;

                        case "4":
                            verifonePayment.AddMerchandise();
                            WaitForEvent(basketEventStatusEventReceived, "AddMerchandise");
                            break;

                        case "5":
                            verifonePayment.PaymentTransaction((long)(new Random().NextDouble() * 99) + 1, System.Guid.NewGuid().ToString(), "EUR");
                            WaitForEvent(paymentCompletedEventReceived, "PaymentTransaction");
                            break;

                        case "6":
                            verifonePayment.RemoveMerchandise();
                            WaitForEvent(basketEventStatusEventReceived, "RemoveMerchandise");
                            break;

                        case "7":
                            verifonePayment.EndSession();
                            WaitForEvent(statusEventReceived, "EndSession");
                            break;
                        case "8":
                            verifonePayment.TearDown();
                            WaitForEvent(statusEventReceived, "TearDown");
                            break;

                        case "9":
                            RunUnitTests();
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