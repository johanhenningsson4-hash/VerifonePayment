# Verifone Payment Integration Library

A .NET Framework library for integrating with Verifone payment devices, providing a managed wrapper around the Verifone SDK with comprehensive configuration management and event-driven architecture.

## Features

- **Configuration Management**: Flexible configuration via app.config or programmatic setup
- **Event-Driven Architecture**: Comprehensive event handling for payment operations
- **Logging Support**: Configurable logging with automatic file management
- **Device Management**: Complete device lifecycle management (initialization, login, sessions, teardown)
- **Payment Processing**: Support for various payment types and transaction management
- **Merchandise Management**: Basket operations for adding/removing items

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

// Basic workflow
try
{
    payment.CommunicateWithPaymentSDK();
    payment.LoginWithCredentials();
    payment.StartSession(Guid.NewGuid().ToString());
    
    // Add merchandise and process payment
    payment.AddMerchandise();
    payment.PaymentTransaction(1000); // $10.00
    
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
- `PaymentTransaction(amount, paymentType, scale)` - Process a payment transaction

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
- `CommerceEventOccurred` - Commerce-related events

## Sample Applications

### Console Test Application

The included `VerifonePayment.Test` project provides a comprehensive example with an interactive menu for testing all library features.

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

## Dependencies

- **.NET Framework 4.7.2+** - Target framework
- **System.Configuration** - Configuration management (automatically referenced)
- **VerifoneSdk** - Verifone payment device SDK (external - included in Dlls folder)

## Recent Updates

### Version 1.0.1
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

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or issues, please open an issue on GitHub or contact the maintainer.
