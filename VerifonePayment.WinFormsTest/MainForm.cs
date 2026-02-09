using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using VerifonePayment.Lib;
using VerifonePayment.Lib.Models;

namespace VerifonePayment.WinFormsTest
{
    public partial class MainForm : Form
    {
        #region "Fields"
        
        private VerifonePayment.Lib.VerifonePayment _verifonePayment;
        
        // Event synchronization
        private ManualResetEvent _statusEventReceived = new ManualResetEvent(false);
        private ManualResetEvent _loginEventReceived = new ManualResetEvent(false);
        private ManualResetEvent _startSessionStatusEventReceived = new ManualResetEvent(false);
        private ManualResetEvent _basketEventStatusEventReceived = new ManualResetEvent(false);
        private ManualResetEvent _paymentCompletedEventReceived = new ManualResetEvent(false);
        
        // Stability monitoring
        private readonly object _logLock = new object();
        private bool _isDisposing = false;
        private static int _instanceCounter = 0;
        private readonly int _instanceId;
        private readonly Stopwatch _formLifetime = Stopwatch.StartNew();
        private System.Windows.Forms.Timer _monitoringTimer;
        
        #endregion
        
        #region "Constructor"
        
        public MainForm()
        {
            _instanceId = Interlocked.Increment(ref _instanceCounter);
            LogDebug($"MainForm constructor starting [Instance: {_instanceId}]");
            
            InitializeComponent();
            InitializePaymentSystem();
            EnablePerformanceMonitoring();
            
            LogDebug($"MainForm constructor completed [Instance: {_instanceId}]");
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializePaymentSystem()
        {
            LogOperation("InitializePaymentSystem");
            try
            {
                _verifonePayment = new VerifonePayment.Lib.VerifonePayment();
                
                // Subscribe to events with logging
                LogEventSubscription("StatusEventOccurred", "SUBSCRIBE");
                _verifonePayment.StatusEventOccurred += VerifonePayment_StatusEventOccurred;
                
                LogEventSubscription("TransactionEventOccurred", "SUBSCRIBE");
                _verifonePayment.TransactionEventOccurred += VerifonePayment_TransactionEventOccurred;
                
                LogEventSubscription("DeviceVitalsInformationEventOccurred", "SUBSCRIBE");
                _verifonePayment.DeviceVitalsInformationEventOccurred += VerifonePayment_DeviceVitalsInformationEventOccurred;
                
                LogEventSubscription("BasketEventOccurred", "SUBSCRIBE");
                _verifonePayment.BasketEventOccurred += VerifonePayment_BasketEventOccurred;
                
                LogEventSubscription("NotificationEventOccurred", "SUBSCRIBE");
                _verifonePayment.NotificationEventOccurred += VerifonePayment_NotificationEventOccurred;
                
                LogEventSubscription("PaymentCompletedEventOccurred", "SUBSCRIBE");
                _verifonePayment.PaymentCompletedEventOccurred += VerifonePayment_PaymentCompletedEventOccurred;
                
                LogEventSubscription("CommerceEventOccurred", "SUBSCRIBE");
                _verifonePayment.CommerceEventOccurred += VerifonePayment_CommerceEventOccurred;
                
                // Display configuration
                LogMessage("=== Verifone Payment WinForms Test Application ===");
                LogMessage($"Configuration: {_verifonePayment.Configuration.GetConfigurationSummary()}");
                LogMessage("Ready to test payment operations.");
                
                // Enable initial buttons
                _btnCommunicate.Enabled = true;
                _btnValidateConfig.Enabled = true;
                
                LogOperation("InitializePaymentSystem", "SUCCESS");
            }
            catch (Exception ex)
            {
                LogOperation("InitializePaymentSystem", "ERROR", ex);
                LogError($"Initialization failed: {ex.Message}");
                MessageBox.Show($"Failed to initialize payment system: {ex.Message}", "Initialization Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #endregion
        
        #region "Event Handlers - Payment Operations"
        
        private async void BtnCommunicate_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("CommunicateWithPaymentSDK", () =>
            {
                _verifonePayment.CommunicateWithPaymentSDK();
                WaitForEvent(_statusEventReceived, "CommunicateWithPaymentSDK");
                
                // Enable login button after successful communication
                this.Invoke((Action)(() => _btnLogin.Enabled = true));
            });
        }
        
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("LoginWithCredentials", () =>
            {
                _verifonePayment.LoginWithCredentials();
                WaitForEvent(_loginEventReceived, "LoginWithCredentials");
                
                // Enable session management buttons after successful login
                this.Invoke((Action)(() => 
                {
                    _btnStartSession.Enabled = true;
                    _txtInvoiceId.Enabled = true;
                }));
            });
        }
        
        private async void BtnStartSession_Click(object sender, EventArgs e)
        {
            string invoiceId = _txtInvoiceId.Text.Trim();
            if (string.IsNullOrEmpty(invoiceId))
            {
                invoiceId = Guid.NewGuid().ToString();
                this.Invoke((Action)(() => _txtInvoiceId.Text = invoiceId));
            }
            
            await ExecutePaymentOperation("StartSession", () =>
            {
                _verifonePayment.StartSession(invoiceId);
                WaitForEvent(_startSessionStatusEventReceived, "StartSession");
                
                // Enable transaction buttons after successful session start
                this.Invoke((Action)(() => 
                {
                    _btnAddMerchandise.Enabled = true;
                    _btnPayment.Enabled = true;
                    _btnRemoveMerchandise.Enabled = true;
                    _btnEndSession.Enabled = true;
                    _numAmount.Enabled = true;
                }));
            });
        }
        
        private async void BtnAddMerchandise_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("AddMerchandise", () =>
            {
                _verifonePayment.AddMerchandise();
                WaitForEvent(_basketEventStatusEventReceived, "AddMerchandise");
            });
        }
        
        private async void BtnPayment_Click(object sender, EventArgs e)
        {
            decimal amount = _numAmount.Value;
            if (amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Amount", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            await ExecutePaymentOperation("PaymentTransaction", () =>
            {
                long centAmount = (long)(amount * 100); // Convert to cents
                _verifonePayment.PaymentTransaction(centAmount);
                WaitForEvent(_paymentCompletedEventReceived, "PaymentTransaction");
            });
        }
        
        private async void BtnRemoveMerchandise_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("RemoveMerchandise", () =>
            {
                _verifonePayment.RemoveMerchandise();
                WaitForEvent(_basketEventStatusEventReceived, "RemoveMerchandise");
            });
        }
        
        private async void BtnEndSession_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("EndSession", () =>
            {
                _verifonePayment.EndSession();
                WaitForEvent(_statusEventReceived, "EndSession");
                
                // Disable transaction buttons after ending session
                this.Invoke((Action)(() => 
                {
                    _btnAddMerchandise.Enabled = false;
                    _btnPayment.Enabled = false;
                    _btnRemoveMerchandise.Enabled = false;
                    _btnEndSession.Enabled = false;
                    _btnTearDown.Enabled = true;
                    _numAmount.Enabled = false;
                }));
            });
        }
        
        private async void BtnTearDown_Click(object sender, EventArgs e)
        {
            await ExecutePaymentOperation("TearDown", () =>
            {
                _verifonePayment.TearDown();
                WaitForEvent(_statusEventReceived, "TearDown");
                
                // Reset all buttons to initial state
                this.Invoke((Action)(() => 
                {
                    _btnLogin.Enabled = false;
                    _btnStartSession.Enabled = false;
                    _btnTearDown.Enabled = false;
                    _txtInvoiceId.Enabled = false;
                    _txtInvoiceId.Clear();
                }));
            });
        }
        
        #endregion
        
        #region "Event Handlers - UI Operations"
        
        private void BtnValidateConfig_Click(object sender, EventArgs e)
        {
            bool isValid = _verifonePayment.Configuration.IsValid();
            string message = isValid ? "Configuration is valid." : "Configuration is invalid.";
            MessageBoxIcon icon = isValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
            
            MessageBox.Show(message, "Configuration Validation", MessageBoxButtons.OK, icon);
            LogMessage($"Configuration validation: {message}");
        }
        
        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            _txtLog.Clear();
            LogMessage("Log cleared.");
        }
        
        private void BtnGenerateInvoice_Click(object sender, EventArgs e)
        {
            _txtInvoiceId.Text = Guid.NewGuid().ToString();
            LogMessage($"Generated new invoice ID: {_txtInvoiceId.Text}");
        }
        
        #endregion
        
        #region "Payment Event Handlers"
        
        private void VerifonePayment_StatusEventOccurred(object sender, PaymentEventArgs e)
        {
            LogDebug($"StatusEvent received: {e.Status}, {e.Type}, {e.Message}");
            try
            {
                LogEvent("Status", e);
                _statusEventReceived.Set();
                LogDebug("StatusEvent processed successfully");
            }
            catch (Exception ex)
            {
                LogDebug($"StatusEvent processing failed: {ex.Message}");
                throw;
            }
        }
        
        private void VerifonePayment_TransactionEventOccurred(object sender, PaymentEventArgs e)
        {
            LogDebug($"TransactionEvent received: {e.Status}, {e.Type}, {e.Message}");
            try
            {
                LogEvent("Transaction", e);
                
                if (e.Type == Lib.Enums.EventType.SESSION_STARTED && e.Status == "0")
                {
                    _startSessionStatusEventReceived.Set();
                    LogDebug("SESSION_STARTED event signaled");
                }
                if (e.Type == Lib.Enums.EventType.SESSION_ENDED && e.Status == "0")
                {
                    _statusEventReceived.Set();
                    LogDebug("SESSION_ENDED event signaled");
                }
                if (e.Type == Lib.Enums.EventType.LOGIN_COMPLETED)
                {
                    _loginEventReceived.Set();
                    LogDebug("LOGIN_COMPLETED event signaled");
                    
                    if (e.Status == "-20")
                        _statusEventReceived.Set();
                }
            }
            catch (Exception ex)
            {
                LogDebug($"TransactionEvent processing failed: {ex.Message}");
                throw;
            }
        }
        
        private void VerifonePayment_DeviceVitalsInformationEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Device Vitals", e);
            _statusEventReceived.Set();
        }
        
        private void VerifonePayment_BasketEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Basket", e);
            _basketEventStatusEventReceived.Set();
        }
        
        private void VerifonePayment_NotificationEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Notification", e);
        }
        
        private void VerifonePayment_PaymentCompletedEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Payment Completed", e);
            
            if (e.Type == Lib.Enums.EventType.NOTIFICATION_EVENT && e.Message == "Transaction Completed")
                _paymentCompletedEventReceived.Set();
            if (e.Type == Lib.Enums.EventType.TRANSACTION_PAYMENT_COMPLETED)
                _paymentCompletedEventReceived.Set();
        }
        
        private void VerifonePayment_CommerceEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Commerce", e);
            
            if (e.Type == Lib.Enums.EventType.STATUS_ERROR && e.Status == "-20")
                _startSessionStatusEventReceived.Set();
        }
        
        #endregion
        
        #region "Helper Methods"
        
        private async System.Threading.Tasks.Task ExecutePaymentOperation(string operationName, Action operation)
        {
            try
            {
                LogMessage($"Executing {operationName}...");
                SetButtonsEnabled(false);
                
                // Run operation on background thread
                await System.Threading.Tasks.Task.Run(operation);
                
                LogMessage($"{operationName} completed successfully.");
            }
            catch (Exception ex)
            {
                LogError($"{operationName} failed: {ex.Message}");
                MessageBox.Show($"{operationName} failed: {ex.Message}", "Operation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }
        
        private void WaitForEvent(ManualResetEvent eventHandle, string actionName)
        {
            LogMessage($"{actionName}: Waiting for response...");
            eventHandle.WaitOne();
            eventHandle.Reset();
        }
        
        private void LogMessage(string message)
        {
            var invokeAttempted = false;
            
            if (this.InvokeRequired)
            {
                invokeAttempted = true;
                LogDebug($"LogMessage invoke required for: {message.Substring(0, Math.Min(50, message.Length))}...");
                
                try
                {
                    // Check if form is disposing before attempting to invoke
                    if (!this.IsDisposed && !this.Disposing && !_isDisposing)
                    {
                        this.Invoke((Action)(() => LogMessage(message)));
                        LogDebug("LogMessage invoke successful");
                    }
                    else
                    {
                        LogDebug($"LogMessage skipped - form disposing/disposed. Message: {message}");
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    LogDebug($"LogMessage ObjectDisposedException caught (expected during disposal): {ex.Message}");
                }
                catch (Exception ex)
                {
                    LogDebug($"LogMessage unexpected exception: {ex}");
                }
                return;
            }
            
            // Additional safety check for UI thread operations
            if (!this.IsDisposed && !this.Disposing && !_isDisposing && _txtLog != null && !_txtLog.IsDisposed)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                    _txtLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
                    _txtLog.SelectionStart = _txtLog.Text.Length;
                    _txtLog.ScrollToCaret();
                    
                    if (invokeAttempted)
                        LogDebug("LogMessage UI update successful after invoke");
                }
                catch (Exception ex)
                {
                    LogDebug($"LogMessage UI update failed: {ex}");
                }
            }
            else
            {
                LogDebug($"LogMessage UI update skipped - controls disposed. Message: {message}");
            }
        }
        
        private void LogError(string message)
        {
            LogMessage($"ERROR: {message}");
        }
        
        private void LogEvent(string eventType, PaymentEventArgs e)
        {
            LogMessage($"{eventType} Event - Status: {e.Status}, Type: {e.Type}, Message: {e.Message}");
        }
        
        private void SetButtonsEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetButtonsEnabled(enabled)));
                return;
            }
            
            // Only enable/disable buttons that should be affected during operations
            // Keep configuration and utility buttons always enabled
            foreach (Control control in _pnlOperations.Controls)
            {
                if (control is Button btn && btn != _btnValidateConfig && btn != _btnClearLog && btn != _btnGenerateInvoice)
                {
                    // Special handling for operation flow
                    if (!enabled || btn == _btnCommunicate) // Communication is always enabled when not running operation
                        btn.Enabled = enabled;
                }
            }
        }
        
        #endregion
        
        #region "Stability Monitoring Methods"
        
        private void LogDebug(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            lock (_logLock)
            {
                var threadInfo = $"T{Thread.CurrentThread.ManagedThreadId}{(Thread.CurrentThread.IsBackground ? "B" : "M")}";
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var formState = GetFormStateInfo();
                Debug.WriteLine($"[{timestamp}] [{threadInfo}] [{caller}:{line}] {formState} {message}");
            }
        }
        
        private void LogOperation(string operation, string status = "START", Exception ex = null)
        {
            var message = ex != null 
                ? $"OPERATION {operation} - {status}: {ex.Message}" 
                : $"OPERATION {operation} - {status}";
            LogDebug(message);
        }
        
        private void LogThreadState(string context)
        {
            var activeThreads = Process.GetCurrentProcess().Threads.Count;
            var workingSet = GC.GetTotalMemory(false) / 1024 / 1024;
            LogDebug($"THREAD_STATE {context} - Active: {activeThreads}, Memory: {workingSet}MB");
        }
        
        private void LogEventSubscription(string eventName, string action)
        {
            LogDebug($"EVENT_{action.ToUpper()} {eventName} [Instance: {_instanceId}]");
        }
        
        private string GetFormStateInfo()
        {
            if (_isDisposing) return "[DISPOSING]";
            if (this.IsDisposed) return "[DISPOSED]";
            if (this.Disposing) return "[DISPOSING_SYS]";
            if (!this.IsHandleCreated) return "[NO_HANDLE]";
            if (this.InvokeRequired) return "[CROSS_THREAD]";
            return "[NORMAL]";
        }
        
        private void LogFormLifecycle(string phase)
        {
            LogDebug($"FORM_LIFECYCLE {phase} [Instance: {_instanceId}, Uptime: {_formLifetime.ElapsedMilliseconds}ms]");
            LogThreadState(phase);
        }
        
        private void EnablePerformanceMonitoring()
        {
            _monitoringTimer = new System.Windows.Forms.Timer();
            _monitoringTimer.Interval = 10000; // 10 seconds
            _monitoringTimer.Tick += (s, e) =>
            {
                try
                {
                    var process = Process.GetCurrentProcess();
                    var workingSet = process.WorkingSet64 / 1024 / 1024;
                    var gcMemory = GC.GetTotalMemory(false) / 1024 / 1024;
                    var threadCount = process.Threads.Count;
                    
                    LogDebug($"MONITOR - Memory: {workingSet}MB, GC: {gcMemory}MB, Threads: {threadCount}");
                }
                catch (Exception ex)
                {
                    LogDebug($"Monitoring error: {ex.Message}");
                }
            };
            _monitoringTimer.Start();
            LogDebug("Performance monitoring enabled");
        }
        
        #endregion
        
        #region "Form Events"
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogFormLifecycle("FORM_CLOSING_START");
            _isDisposing = true;
            
            try
            {
                // Stop performance monitoring
                if (_monitoringTimer != null)
                {
                    _monitoringTimer.Stop();
                    _monitoringTimer.Dispose();
                    _monitoringTimer = null;
                    LogDebug("Performance monitoring stopped");
                }
                
                if (_verifonePayment != null)
                {
                    LogMessage("Cleaning up payment system...");
                    LogThreadState("BEFORE_UNSUBSCRIBE");
                    
                    // Unsubscribe from events BEFORE teardown to prevent race conditions
                    LogEventSubscription("StatusEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.StatusEventOccurred -= VerifonePayment_StatusEventOccurred;
                    
                    LogEventSubscription("TransactionEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.TransactionEventOccurred -= VerifonePayment_TransactionEventOccurred;
                    
                    LogEventSubscription("DeviceVitalsInformationEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.DeviceVitalsInformationEventOccurred -= VerifonePayment_DeviceVitalsInformationEventOccurred;
                    
                    LogEventSubscription("BasketEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.BasketEventOccurred -= VerifonePayment_BasketEventOccurred;
                    
                    LogEventSubscription("NotificationEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.NotificationEventOccurred -= VerifonePayment_NotificationEventOccurred;
                    
                    LogEventSubscription("PaymentCompletedEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.PaymentCompletedEventOccurred -= VerifonePayment_PaymentCompletedEventOccurred;
                    
                    LogEventSubscription("CommerceEventOccurred", "UNSUBSCRIBE");
                    _verifonePayment.CommerceEventOccurred -= VerifonePayment_CommerceEventOccurred;
                    
                    LogDebug("All events unsubscribed - calling TearDown");
                    LogThreadState("BEFORE_TEARDOWN");
                    
                    // Now safely call teardown
                    _verifonePayment.TearDown();
                    LogDebug("TearDown completed");
                    LogThreadState("AFTER_TEARDOWN");
                }
            }
            catch (Exception ex)
            {
                // Log but don't show UI during form closing to avoid disposal issues
                LogDebug($"Cleanup exception: {ex}");
                System.Diagnostics.Debug.WriteLine($"Cleanup failed: {ex.Message}");
            }
            
            LogFormLifecycle("FORM_CLOSING_END");
        }
        
        protected override void Dispose(bool disposing)
        {
            LogFormLifecycle($"DISPOSE_START - disposing: {disposing}");
            
            try
            {
                if (disposing)
                {
                    // Dispose designer components first
                    if (components != null)
                    {
                        components.Dispose();
                    }
                    
                    // Dispose our managed resources
                    if (_statusEventReceived != null)
                    {
                        _statusEventReceived.Dispose();
                        _statusEventReceived = null;
                    }
                    if (_loginEventReceived != null)
                    {
                        _loginEventReceived.Dispose();
                        _loginEventReceived = null;
                    }
                    if (_startSessionStatusEventReceived != null)
                    {
                        _startSessionStatusEventReceived.Dispose();
                        _startSessionStatusEventReceived = null;
                    }
                    if (_basketEventStatusEventReceived != null)
                    {
                        _basketEventStatusEventReceived.Dispose();
                        _basketEventStatusEventReceived = null;
                    }
                    if (_paymentCompletedEventReceived != null)
                    {
                        _paymentCompletedEventReceived.Dispose();
                        _paymentCompletedEventReceived = null;
                    }
                    if (_monitoringTimer != null)
                    {
                        _monitoringTimer.Dispose();
                        _monitoringTimer = null;
                    }
                    
                    LogDebug("Managed resources disposed");
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Dispose exception: {ex}");
            }
            finally
            {
                base.Dispose(disposing);
                LogFormLifecycle("DISPOSE_END");
                if (_formLifetime != null)
                {
                    _formLifetime.Stop();
                }
            }
        }
        
        #endregion
    }
}