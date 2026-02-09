# Verifone Payment Windows Forms Test Application

This is a comprehensive Windows Forms test application for the Verifone Payment Integration Library. It provides a user-friendly graphical interface to test all payment operations and monitor events in real-time.

## Features

### **Graphical User Interface**
- **Organized Layout**: Clean, professional interface with logical grouping of operations
- **Real-time Event Logging**: Live display of all payment events with timestamps
- **Interactive Controls**: Numbered buttons guide you through the payment workflow
- **Configuration Management**: Built-in configuration validation and invoice generation

### **Payment Operations**
1. **Communicate with SDK** - Initialize communication with the payment device
2. **Login with Credentials** - Authenticate using configured credentials  
3. **Start Session** - Begin a new payment session with invoice ID
4. **Add Merchandise** - Add items to the payment basket
5. **Process Payment** - Execute payment transaction with configurable amount
6. **Remove Merchandise** - Remove items from basket (optional)
7. **End Session** - Complete and close the payment session
8. **Tear Down** - Clean disconnect from the payment device

### **Advanced Features**
- **Async Operations**: All payment operations run asynchronously to prevent UI freezing
- **Event Monitoring**: Real-time display of all payment events with detailed information
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Configuration Tools**: Validate settings and generate new invoice IDs
- **Operation Flow Control**: Buttons are enabled/disabled based on current state

## Getting Started

### **Prerequisites**
- Windows operating system
- .NET Framework 4.8 or later
- Verifone payment device (or simulator)
- Network connectivity to the device

### **Configuration**
Edit the `App.config` file to match your Verifone device settings:

```xml
<!-- Device Configuration -->
<add key="DeviceIpAddress" value="192.168.0.36" />
<add key="DeviceConnectionType" value="tcpip" />

<!-- Authentication -->
<add key="DefaultUsername" value="username" />
<add key="DefaultPassword" value="password" />
<add key="DefaultShiftNumber" value="shift" />
```

### **Running the Application**

1. **Build the Solution**:
   ```bash
   dotnet build VerifonePayment.sln
   ```

2. **Run the Windows Forms Test**:
   ```bash
   cd VerifonePayment.WinFormsTest\bin\Debug
   VerifonePayment.WinFormsTest.exe
   ```

3. **Follow the Workflow**:
   - Click "1. Communicate with SDK" to initialize
   - Click "2. Login with Credentials" to authenticate
   - Click "3. Start Session" to begin a payment session
   - Add merchandise and process payments as needed
   - End session and tear down when complete

## User Interface Guide

### **Payment Operations Panel** (Left Side)
- **Sequential Workflow**: Buttons are numbered to guide you through the process
- **State Management**: Buttons are automatically enabled/disabled based on current state  
- **Visual Feedback**: Button states indicate what operations are currently available

### **Configuration Panel** (Top Right)
- **Invoice ID**: Generate or manually enter invoice IDs for sessions
- **Payment Amount**: Set the transaction amount (in dollars)
- **Validate Config**: Check if current configuration is valid

### **Event Log Panel** (Bottom)
- **Real-time Logging**: All events are displayed as they occur
- **Timestamps**: Each event includes precise timing information
- **Event Details**: Status codes, event types, and descriptive messages
- **Clear Log**: Reset the log display when needed

## Event Types Monitored

The application monitors and displays these event categories:
- **Status Events** - General status updates and confirmations
- **Transaction Events** - Payment transaction progress and results
- **Device Vitals** - Hardware health and diagnostic information
- **Basket Events** - Merchandise addition/removal operations
- **Notification Events** - System notifications and alerts
- **Payment Completed** - Transaction completion confirmations
- **Commerce Events** - Business logic and workflow events

## Tips for Testing

### **Basic Testing Workflow**
1. Start with "Validate Configuration" to ensure settings are correct
2. Follow the numbered buttons in sequence for standard operations
3. Monitor the event log for detailed feedback on each operation
4. Use different payment amounts to test various scenarios

### **Advanced Testing**
- Test error conditions by using invalid configuration settings
- Try different payment amounts including edge cases (very small/large amounts)
- Test the merchandise basket operations in different sequences
- Verify proper cleanup by always running "Tear Down" before closing

### **Troubleshooting**
- Check the event log for detailed error messages
- Verify network connectivity to the payment device
- Ensure the Verifone SDK is properly installed
- Validate configuration settings before starting operations

## Comparison with Console Test

| Feature | Console Test | Windows Forms Test |
|---------|-------------|-------------------|
| **Interface** | Text-based menu | Graphical interface |
| **Event Display** | Console output | Real-time log panel |
| **User Experience** | Command-driven | Click-driven |
| **State Management** | Manual tracking | Automatic button states |
| **Error Handling** | Text messages | Message boxes + log |
| **Workflow Guidance** | Menu numbers | Visual button sequence |

## Integration Notes

This Windows Forms test application uses the same `VerifonePayment.Lib` library as the console version, ensuring consistent behavior and testing of the same functionality. The GUI wrapper adds:

- Enhanced user experience
- Better error visualization  
- Real-time event monitoring
- Simplified operation sequencing
- Professional appearance for demonstrations

## Development Notes

### **Architecture**
- **MVVM Pattern**: Clean separation between UI and business logic
- **Async/Await**: Non-blocking operations for better responsiveness
- **Event-Driven**: Reactive UI updates based on payment events
- **Table Layout Panels**: Responsive, scalable UI layout

### **Error Handling**
- **Exception Catching**: All operations wrapped in try-catch blocks
- **User Feedback**: Both message boxes and log entries for errors
- **Recovery**: Graceful handling of connection failures and timeouts

### **Threading**
- **UI Thread Safety**: All UI updates properly marshaled to main thread
- **Background Operations**: Payment operations run on background threads
- **Event Synchronization**: Manual reset events for operation completion

This Windows Forms test application provides a comprehensive, professional tool for testing and demonstrating the Verifone Payment Integration Library capabilities.