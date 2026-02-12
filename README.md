# Verifone Payment Integration Library

A .NET Framework library for integrating with Verifone payment devices, providing a managed wrapper around the Verifone SDK with comprehensive configuration management, event-driven architecture, complete refund processing, and advanced receipt handling capabilities.

## Features

- **Configuration Management**: Flexible configuration via app.config or programmatic setup
- **Event-Driven Architecture**: Comprehensive event handling for payment operations
- **Payment Processing**: Support for various payment types and transaction management
- **Refund Processing**: Complete linked and unlinked refund functionality with partial refund support
- **Receipt Handling**: Advanced receipt processing, validation, formatting, and archiving capabilities
- **Reconciliation & Reporting**: Complete end-of-day operations, transaction querying, and Store & Forward (SAF) support
- **Merchandise Management**: Basket operations for adding/removing items
- **Device Management**: Complete device lifecycle management (initialization, login, sessions, teardown)
- **Logging Support**: Configurable logging with automatic file management
- **Workflow Validation**: State management ensuring proper operation sequence

## Quick Start

### 1. Installation

1. Clone or download this repository
2. Build the solution in Visual Studio
3. Reference `VerifonePayment.Lib.dll` in your project
4. Ensure the Verifone SDK is properly installed and accessible

### 2. Basic Configuration

Add the following to your application's `app.config` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <!-- Verifone Device Configuration -->
        <add key="DeviceIpAddress" value="192.168.1.100" />
        <add key="DeviceConnectionType" value="tcpip" />
        
        <!-- Authentication Configuration -->
        <add key="DefaultUsername" value="your_username" />
        <add key="DefaultPassword" value="your_password" />
        <add key="DefaultShiftNumber" value="your_shift" />
        
        <!-- Logging Configuration -->
        <add key="LogFilePath" value="" />
        <add key="DeleteLogFile" value="false" />
        
        <!-- Timeout Configuration (in seconds) -->
        <add key="ConnectionTimeoutSeconds" value="30" />
        <add key="TransactionTimeoutSeconds" value="60" />
    </appSettings>
</configuration>
```

### 3. Basic Usage

```csharp
using VerifonePayment.Lib;

// Initialize with configuration from app.config
var payment = new VerifonePayment();

// Or initialize with custom IP address
var payment = new VerifonePayment("192.168.1.100");

// Subscribe to events
payment.StatusEventOccurred += (sender, e) => {
    Console.WriteLine($"Status: {e.Status} - {e.Message}");
};

payment.PaymentCompletedEventOccurred += (sender, e) => {
    Console.WriteLine($"Payment completed: {e.Status}");
};

payment.RefundCompletedEventOccurred += (sender, e) => {
    Console.WriteLine($"Refund completed: {e.Status}");
};

payment.PrintEventOccurred += (sender, e) => {
    Console.WriteLine($"Print event: {e.Status}");
};

// Basic workflow
try
{
    payment.CommunicateWithPaymentSDK();
    payment.LoginWithCredentials();
    
    string invoiceId = Guid.NewGuid().ToString();
    payment.StartSession(invoiceId);
    
    // Add merchandise and process payment
    payment.AddMerchandise();
    string paymentId = Guid.NewGuid().ToString();
    payment.PaymentTransaction(1000, paymentId, "EUR"); // $10.00
    
    // Extract and handle receipts (after payment completion)
    // Note: Receipt extraction would typically happen in payment completion event
    // var receiptWrapper = payment.ExtractReceipt(paymentObject);
    // if (receiptWrapper != null && receiptWrapper.IsValid())
    // {
    //     // Save receipt in multiple formats
    //     payment.SaveReceipt(receiptWrapper, "receipt.txt", "txt");
    //     payment.SaveReceipt(receiptWrapper, "receipt.html", "html");
    //     
    //     // Archive receipt with timestamp
    //     payment.ArchiveReceipt(receiptWrapper, @"C:\Receipts", paymentId);
    //     
    //     // Validate receipt content
    //     var validation = payment.ValidateReceipt(receiptWrapper);
    //     if (!validation.IsValid) Console.WriteLine(validation.GetDetailedReport());
    // }
    
    // Process refunds (after successful payment)
    // Full refund (linked to original payment)
    payment.ProcessLinkedRefund(paymentId);
    
    // Or partial refund
    payment.ProcessLinkedRefund(paymentId, 500, "EUR"); // Refund $5.00
    
    // Or unlinked refund (standalone)
    payment.ProcessUnlinkedRefund(250, "EUR", "REFUND-001");
    
    // Clean up
    payment.EndSession();
    payment.TearDown();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Configuration Reference

### Device Settings

| Setting | Description | Default | Required |
|---------|-------------|---------|----------|
| `DeviceIpAddress` | IP address of the Verifone device | 127.0.0.1 | Yes |
| `DeviceConnectionType` | Connection type (typically 'tcpip') | tcpip | Yes |

### Authentication Settings

| Setting | Description | Default | Required |
|---------|-------------|---------|----------|
| `DefaultUsername` | Default username for device login | username | Yes |
| `DefaultPassword` | Default password for device login | password | Yes |
| `DefaultShiftNumber` | Default shift number for operations | shift | Yes |

### Logging Settings

| Setting | Description | Default | Required |
|---------|-------------|---------|----------|
| `LogFilePath` | Custom log file path (empty = temp directory) | (empty) | No |
| `DeleteLogFile` | Delete existing log file on startup | false | No |

### Timeout Settings

| Setting | Description | Default | Required |
|---------|-------------|---------|----------|
| `ConnectionTimeoutSeconds` | Device connection timeout | 30 | No |
| `TransactionTimeoutSeconds` | Transaction operation timeout | 60 | No |

## API Reference

### Constructor Options

```csharp
// Use configuration from app.config
var payment = new VerifonePayment();

// Override IP address from configuration
var payment = new VerifonePayment("192.168.1.100");
```

### Core Methods

#### Device Management
- `CommunicateWithPaymentSDK()` - Initialize communication with the payment device
- `LoginWithCredentials()` - Login using configured credentials
- `LoginWithCredentials(username, password, shiftNumber)` - Login with custom credentials
- `TearDown()` - Clean up and disconnect from device

#### Session Management
- `StartSession(invoiceId)` - Start a new payment session
- `EndSession()` - End the current payment session

#### Transaction Operations
- `AddMerchandise()` - Add merchandise to the current basket
- `RemoveMerchandise()` - Remove last merchandise item from basket
- `PaymentTransaction(amount, invoiceId, currency, paymentType, scale)` - Process a payment transaction

#### Refund Operations
- `ProcessLinkedRefund(originalPaymentId, refundAmount, currency, scale)` - Process a linked refund with reference to original payment
- `ProcessUnlinkedRefund(refundAmount, currency, invoiceId, scale)` - Process an unlinked refund (standalone)
- `ProcessRefund(refundPayment)` - Advanced refund processing with Payment object

**Refund Features:**
- **Linked Refunds**: Reference original payment for fraud protection and host support
- **Unlinked Refunds**: Standalone refunds without payment reference
- **Full Refunds**: Pass `null` for refundAmount in linked refunds
- **Partial Refunds**: Specify exact amount to refund (must not exceed original)
- **Multi-Currency Support**: EUR, USD, GBP, and other standard currencies
- **Automatic Invoice Generation**: Unique refund IDs with timestamp

#### Receipt Handling
- `ExtractReceipt(payment)` - Extract and wrap receipt from Payment object
- `SaveReceipt(receipt, filePath, format)` - Save receipt in multiple formats (txt, html, metadata)
- `ArchiveReceipt(receipt, directory, transactionId)` - Archive receipt with timestamp
- `ValidateReceipt(receipt)` - Comprehensive receipt validation with detailed reporting
- `PrintReceipt(receipt, copies)` - Print receipt if printing is supported
- `IsPrintingSupported()` - Check if printing capability is available

**Receipt Features:**
- **Multiple Formats**: Plain text, HTML, and metadata export
- **Content Validation**: Comprehensive validation with issues and warnings
- **Flexible Extraction**: Reflection-based receipt extraction for SDK compatibility
- **Archive Management**: Timestamped receipt storage with unique filenames
- **Print Support**: Automatic printing capability detection
- **Configuration Access**: Logo, QR code, cashier name, and customization options

#### Reconciliation & Reporting
- `IsReportingCapable(capability)` - Check if reporting capability is supported
- `ClosePeriod()` - Close current period (end of day)
- `ClosePeriodAndReconcile(acquirers)` - Close period and reconcile in single operation
- `QueryTransactions(parameters)` - Query transactions with filtering and pagination
- `QuerySAFTransactions(startTime, endTime)` - Query Store and Forward transactions
- `GetSupportedCapabilities()` - Get all supported reporting capabilities

**Reporting Features:**
- **Unlinked Refunds**: Standalone refunds without payment reference
- **Full Refunds**: Pass `null` for refundAmount in linked refunds
- **Partial Refunds**: Specify exact amount to refund (must not exceed original)
- **Multi-Currency Support**: EUR, USD, GBP, and other standard currencies
- **Automatic Invoice Generation**: Unique refund IDs with timestamp

#### Configuration & Validation
- `ValidateConfiguration()` - Check if current configuration is valid
- `GetConfigurationSummary()` - Get a summary of current settings

### Events

All events use the `PaymentEventArgs` class which contains:
- `Status` - Current status/result
- `Type` - Event type (from EventType enum)
- `Message` - Descriptive message

Available events:
- `StatusEventOccurred` - General status updates
- `TransactionEventOccurred` - Transaction-related events
- `DeviceVitalsInformationEventOccurred` - Device health information
- `BasketEventOccurred` - Basket/merchandise events
- `NotificationEventOccurred` - General notifications
- `PaymentCompletedEventOccurred` - Payment completion events
- `RefundCompletedEventOccurred` - Refund completion events
- `ReconciliationEventOccurred` - Reconciliation and end-of-day events
- `TransactionQueryEventOccurred` - Transaction query result events
- `PrintEventOccurred` - Receipt printing events
- `ReceiptDeliveryMethodEventOccurred` - Receipt delivery method events
- `CommerceEventOccurred` - Commerce-related events

## Sample Applications

### Console Test Application

The included `VerifonePayment.Test` project provides a comprehensive example with an interactive menu for testing all library features including:
- Complete payment workflow (SDK initialization ? Login ? Session ? Payment ? Teardown)
- **Refund Processing**: Both linked and unlinked refunds with full/partial options
- **Receipt Handling**: Receipt extraction, validation, saving, and archiving
- **Reconciliation & Reporting**: End-of-day operations, transaction queries, and capability checking
- **Workflow Validation**: State management ensuring proper operation sequence
- **Real-time Event Monitoring**: Live display of all payment, refund, receipt, and reconciliation events
- **Interactive Options**: Choose formats, amounts, and processing methods

### Windows Forms Test Application

The `VerifonePayment.WinFormsTest` project offers a modern GUI interface for testing payment operations:

- **Professional Interface**: Clean, organized layout with numbered operation buttons
- **Real-time Event Monitoring**: Live display of all payment events with timestamps
- **Interactive Controls**: GUI controls for configuration and payment amounts
- **Visual Workflow**: Button states guide users through the payment process
- **Error Handling**: User-friendly error messages and comprehensive logging

**Key Features:**
- Async operations prevent UI freezing
- Automatic button state management based on operation flow
- Built-in configuration validation
- Invoice ID generation
- Clear event log with detailed information

To run the Windows Forms test:
```bash
cd VerifonePayment.WinFormsTest\bin\Debug
VerifonePayment.WinFormsTest.exe
```

See [WinForms Test README](VerifonePayment.WinFormsTest/README.md) for detailed usage instructions.

## Receipt Processing Guide

### Receipt Extraction and Validation

Extract and validate receipts from completed payment transactions:

```csharp
// Extract receipt from Payment object (typically in payment completion event)
var receiptWrapper = payment.ExtractReceipt(paymentObject);

if (receiptWrapper != null)
{
    // Validate receipt content and configuration
    var validation = payment.ValidateReceipt(receiptWrapper);
    
    if (validation.IsValid)
    {
        Console.WriteLine("? Receipt is valid");
    }
    else
    {
        Console.WriteLine(validation.GetDetailedReport());
    }
}
```

### Receipt Saving and Archiving

Save receipts in multiple formats or archive for long-term storage:

```csharp
// Save in different formats
payment.SaveReceipt(receiptWrapper, "receipt.txt", "txt");        // Plain text
payment.SaveReceipt(receiptWrapper, "receipt.html", "html");      // HTML format
payment.SaveReceipt(receiptWrapper, "receipt_full.txt", "metadata"); // With metadata

// Archive with timestamp and transaction ID
string archivePath = payment.ArchiveReceipt(receiptWrapper, @"C:\Receipts", transactionId);
Console.WriteLine($"Receipt archived to: {archivePath}");
```

### Receipt Printing and Content Access

Print receipts and access receipt content and configuration:

```csharp
// Check printing capability and print
if (payment.IsPrintingSupported())
{
    payment.PrintReceipt(receiptWrapper, copies: 2);
}

// Access receipt content and properties
Console.WriteLine($"Receipt Type: {receiptWrapper.ReceiptType}");
Console.WriteLine($"Has QR Code: {receiptWrapper.IsQrCodeIncluded}");
Console.WriteLine($"Cashier: {receiptWrapper.CashierName}");

// Get preferred content format
string content = receiptWrapper.GetPreferredContent(); // HTML if available, else plain text
```

## Refund Processing Guide

### Linked Refunds (Recommended)

Linked refunds reference the original payment transaction and are preferred for fraud protection and universal host support:

```csharp
// Full refund - refund the entire original amount
payment.ProcessLinkedRefund(originalPaymentId);

// Partial refund - specify exact amount to refund
payment.ProcessLinkedRefund(originalPaymentId, 50.00m, "EUR");

// With custom currency
payment.ProcessLinkedRefund(originalPaymentId, 25.50m, "USD");
```

### Unlinked Refunds

Unlinked refunds are standalone transactions without reference to an original payment:

```csharp
// Basic unlinked refund
payment.ProcessUnlinkedRefund(75.00m, "EUR");

// With custom invoice ID
payment.ProcessUnlinkedRefund(100.00m, "EUR", "CUSTOM-REFUND-001");
```

### Refund Workflow Requirements

1. **SDK Initialization**: Must call `CommunicateWithPaymentSDK()` first
2. **Authentication**: Must call `LoginWithCredentials()` 
3. **Active Session**: Must call `StartSession()` before processing refunds
4. **Linked Refunds**: Require a completed payment transaction to reference
5. **Event Handling**: Subscribe to `RefundCompletedEventOccurred` for completion tracking

### Refund Event Handling

```csharp
payment.RefundCompletedEventOccurred += (sender, e) => {
    if (e.Status == "0") // Success
    {
        Console.WriteLine($"Refund completed successfully: {e.Message}");
    }
    else if (e.Status == "-11") // Cancelled
    {
        Console.WriteLine("Refund was cancelled by user or system");
    }
    else // Failed
    {
        Console.WriteLine($"Refund failed: {e.Message}");
    }
};
```

## Dependencies

- **.NET Framework 4.7.2+** - Target framework
- **System.Configuration** - Configuration management (automatically referenced)
- **VerifoneSdk** - Verifone payment device SDK (external - included in Dlls folder)

## Recent Updates

### Version 1.2.0 - Advanced Receipt Handling & Comprehensive Reporting
- **Receipt Processing**: Complete receipt extraction, validation, and formatting capabilities
- **Receipt Storage**: Multi-format saving (plain text, HTML, metadata) and timestamped archiving
- **Receipt Validation**: Comprehensive validation with detailed issue/warning reporting
- **Print Integration**: Automatic printing capability detection and receipt printing
- **Reconciliation & Reporting**: End-of-day operations, transaction querying, and Store & Forward (SAF) support
- **Transaction Queries**: Advanced filtering, pagination, and offline transaction support
- **Capability Discovery**: Complete reporting capability enumeration and checking
- **Enhanced Console App**: Receipt handling, reconciliation, and reporting menu options
- **Comprehensive Testing**: 18 new receipt tests + 19 reconciliation tests (123 total tests)
- **Complete Documentation**: Receipt processing guides and API reference

### Version 1.1.0 - Comprehensive Refund Processing
- **?? Linked Refunds**: Complete refund functionality with reference to original payments
- **?? Unlinked Refunds**: Standalone refund processing without payment reference
- **?? Partial Refunds**: Support for partial refund amounts with validation
- **?? Multi-Currency Refunds**: EUR, USD, GBP and other standard currency support
- **?? Refund Events**: New `RefundCompletedEventOccurred` event for refund tracking
- **?? Workflow Validation**: Enhanced state management for payment/refund operations
- **?? Interactive Refund UI**: Console and WinForms test applications updated with refund options
- **?? Unit Tests**: 16 new comprehensive refund tests (86 total tests)
- **?? Documentation**: Complete refund implementation based on Verifone SDK specifications

### Version 1.0.3 - Stability & Monitoring Enhancements
- **?? Critical Fix**: Resolved ObjectDisposedException during form disposal
- **?? Enhanced Monitoring**: Comprehensive debug logging and performance tracking
- **?? Thread Safety**: Improved cross-thread operation handling
- **?? Diagnostics**: Real-time memory, thread, and resource monitoring
- **? Stability**: Enhanced form lifecycle management and cleanup

### Version 1.0.2
- **Fixed Configuration Management**: Added proper reference to `System.Configuration` assembly for .NET Framework projects
- **Enhanced Error Handling**: Improved configuration validation and error messages
- **Project Structure**: Cleaned up project file references and dependencies

## Build Instructions

1. **Prerequisites**:
   - Visual Studio 2017 or later
   - .NET Framework 4.7.2 SDK or later

2. **Build Steps**:
   ```bash
   # Clone the repository
   git clone <repository-url>
   
   # Open solution in Visual Studio
   # Or build from command line:
   dotnet build VerifonePayment.sln
   ```

3. **Running Tests**:
```bash
# Console Test Application
cd VerifonePayment.Test
dotnet run
   
# Windows Forms Test Application
cd VerifonePayment.WinFormsTest\bin\Debug
VerifonePayment.WinFormsTest.exe
```

## Changelog

### [1.2.0] - 2026-02-10
- **Receipt Processing**: Comprehensive receipt extraction, validation, and formatting capabilities
- **ReceiptWrapper Model**: Complete wrapper around Verifone SDK Receipt class with C# 7.3 compatibility
- **Receipt Storage**: Multi-format saving (plain text, HTML, metadata) and timestamped archiving
- **Receipt Validation**: Advanced validation with detailed issue and warning reporting
- **Print Integration**: Automatic printing capability detection and receipt printing support
- **Reconciliation & Reporting**: Complete end-of-day operations and transaction querying
- **Store & Forward (SAF)**: Offline transaction querying and management
- **Transaction Queries**: Advanced filtering, pagination, and time-based searches
- **Capability Discovery**: Complete reporting capability enumeration and checking
- **Enhanced Console App**: Receipt handling and reconciliation menu options with interactive workflows
- **Print & Receipt Events**: PrintEventOccurred and ReceiptDeliveryMethodEventOccurred event handling
- **Comprehensive Testing**: 18 receipt tests + 19 reconciliation tests (123 total tests)
- **Complete API Documentation**: Receipt processing guides and comprehensive API reference

### [1.1.0] - 2026-02-10
- **?? Comprehensive Refund Processing**: Complete implementation of Verifone refund functionality
- **?? Linked Refunds**: `ProcessLinkedRefund()` - refunds with reference to original payment
- **?? Unlinked Refunds**: `ProcessUnlinkedRefund()` - standalone refund processing
- **?? Partial Refund Support**: Specify exact refund amounts with validation
- **?? Multi-Currency Support**: EUR, USD, GBP, and other standard currencies
- **?? Refund Events**: New `RefundCompletedEventOccurred` event for tracking
- **?? Enhanced Test Applications**: Interactive refund options in both console and WinForms
- **?? Comprehensive Testing**: 16 new refund unit tests (total: 86 tests)
- **?? State Management**: Workflow validation for proper payment/refund sequence
- **?? Automatic Invoice Generation**: Unique refund IDs with timestamps
- **?? Documentation**: Complete refund API documentation and examples

### [1.0.3] - 2026-02-09
- **?? Critical Fix**: Resolved race condition causing `ObjectDisposedException` during application shutdown
- **?? Enhanced Monitoring**: Added comprehensive debug logging with thread-aware diagnostics
- **??? Thread Safety**: Improved cross-thread operation handling in event handlers
- **?? Performance Monitoring**: Real-time memory usage, thread count, and resource tracking
- **? Form Lifecycle**: Enhanced disposal process with proper event unsubscription
- **?? Stability**: Added global exception handling and detailed error context
- **?? Debug Output**: Structured logging with caller information and form state tracking
- **?? Resource Management**: Automatic cleanup of ManualResetEvents and monitoring timers

**Key Technical Improvements:**
- Event unsubscription before SDK teardown prevents disposal exceptions
- Enhanced LogMessage with ObjectDisposedException handling
- Performance monitoring timer with 10-second interval metrics
- Form state tracking (Normal, Disposing, Disposed, Cross-thread)
- Thread-aware logging with background/main thread identification
- Global exception handlers in Program.cs for unhandled exceptions

### [1.0.2] - 2026-02-09
- **New**: Added comprehensive Windows Forms test application (`VerifonePayment.WinFormsTest`)
- **Enhanced**: Professional GUI interface with real-time event monitoring
- **Improved**: Visual workflow guidance with numbered operation buttons
- **Added**: Async operation support for non-blocking UI experience
- **Enhanced**: Interactive configuration management and validation tools

### [1.0.1] - 2026-02-09
- **Fixed**: Added missing System.Configuration assembly reference to project file
- **Fixed**: Corrected malformed XML in project file that was causing build issues
- **Improved**: Enhanced project documentation and configuration examples
- **Updated**: Cleaned up project dependencies and references

### [1.0.0] - Initial Release
- Initial implementation of Verifone Payment Integration Library
- Complete configuration management system
- Event-driven architecture for payment operations
- Comprehensive device lifecycle management
- Sample console application with interactive testing

## Troubleshooting

### Common Issues

1. **"Invalid payment configuration" error**
   - Ensure all required settings are present in app.config
   - Verify IP address format is correct
   - Check that authentication credentials are not empty

2. **Device connection failures**
   - Verify device IP address is reachable
   - Check network connectivity
   - Ensure Verifone SDK is properly installed
   - Verify device is powered on and ready

3. **Login failures**
   - Check username/password credentials
   - Verify shift number is valid for your device
   - Ensure device is in the correct state for login

### Debug Information

Enable detailed logging by setting a custom log file path:

```xml
<add key="LogFilePath" value="C:\Logs\verifone.log" />
```

**Enhanced Debug Output (v1.0.3+)**:
The WinForms application now includes comprehensive monitoring in the Visual Studio Debug Output window:

```
[14:32:15.123] [T1M] [InitializePaymentSystem:45] [NORMAL] OPERATION InitializePaymentSystem - START
[14:32:15.134] [T1M] [LogEventSubscription:67] [NORMAL] EVENT_SUBSCRIBE StatusEventOccurred [Instance: 1]
[14:32:15.145] [T3B] [VerifonePayment_StatusEventOccurred:234] [CROSS_THREAD] StatusEvent received: 0, STATUS_TEARDOWN
[14:32:15.156] [T1M] [LogFormLifecycle:89] [DISPOSING] FORM_LIFECYCLE FORM_CLOSING_START [Instance: 1, Uptime: 12456ms]
```

**Debug Output Features:**
- ?? **Performance Metrics**: Memory usage, thread count, GC statistics
- ?? **Thread Tracking**: Main vs background thread identification  
- ?? **Event Flow**: Complete event subscription/unsubscription lifecycle
- ?? **Form State**: Real-time form disposal and lifecycle tracking
- ? **Cross-thread Safety**: Detailed invoke operation monitoring

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or issues, please open an issue on GitHub or contact the maintainer.
