using System;
using System.Drawing;
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
        
        #endregion
        
        #region "Constructor"
        
        public MainForm()
        {
            InitializeComponent();
            InitializePaymentSystem();
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializePaymentSystem()
        {
            try
            {
                _verifonePayment = new VerifonePayment.Lib.VerifonePayment();
                
                // Subscribe to events
                _verifonePayment.StatusEventOccurred += VerifonePayment_StatusEventOccurred;
                _verifonePayment.TransactionEventOccurred += VerifonePayment_TransactionEventOccurred;
                _verifonePayment.DeviceVitalsInformationEventOccurred += VerifonePayment_DeviceVitalsInformationEventOccurred;
                _verifonePayment.BasketEventOccurred += VerifonePayment_BasketEventOccurred;
                _verifonePayment.NotificationEventOccurred += VerifonePayment_NotificationEventOccurred;
                _verifonePayment.PaymentCompletedEventOccurred += VerifonePayment_PaymentCompletedEventOccurred;
                _verifonePayment.CommerceEventOccurred += VerifonePayment_CommerceEventOccurred;
                
                // Display configuration
                LogMessage("=== Verifone Payment WinForms Test Application ===");
                LogMessage($"Configuration: {_verifonePayment.Configuration.GetConfigurationSummary()}");
                LogMessage("Ready to test payment operations.");
                
                // Enable initial buttons
                _btnCommunicate.Enabled = true;
                _btnValidateConfig.Enabled = true;
            }
            catch (Exception ex)
            {
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
            LogEvent("Status", e);
            _statusEventReceived.Set();
        }
        
        private void VerifonePayment_TransactionEventOccurred(object sender, PaymentEventArgs e)
        {
            LogEvent("Transaction", e);
            
            if (e.Type == Lib.Enums.EventType.SESSION_STARTED && e.Status == "0")
            {
                _startSessionStatusEventReceived.Set();
            }
            if (e.Type == Lib.Enums.EventType.SESSION_ENDED && e.Status == "0")
            {
                _statusEventReceived.Set();
            }
            if (e.Type == Lib.Enums.EventType.LOGIN_COMPLETED)
            {
                _loginEventReceived.Set();
                
                if (e.Status == "-20")
                    _statusEventReceived.Set();
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
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => LogMessage(message)));
                return;
            }
            
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            _txtLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            _txtLog.SelectionStart = _txtLog.Text.Length;
            _txtLog.ScrollToCaret();
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
        
        #region "Form Events"
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_verifonePayment != null)
                {
                    LogMessage("Cleaning up payment system...");
                    _verifonePayment.TearDown();
                }
            }
            catch (Exception ex)
            {
                LogError($"Cleanup failed: {ex.Message}");
            }
        }
        
        #endregion
    }
}