# VerifonePayment Test Documentation

## Test Organization

### Test Categories

#### 1. **Unit Tests** (`TestCategory = "Unit"`)
- **Purpose:** Test individual components in isolation
- **Dependencies:** None (mocked)
- **Execution:** Fast (< 1s per test)
- **Examples:**
  - Payment validation logic
  - Configuration parsing
  - Amount calculations
  - Error handling

#### 2. **Integration Tests** (`TestCategory = "Integration"`)
- **Purpose:** Test component interactions
- **Dependencies:** Verifone SDK (mocked or simulated)
- **Execution:** Medium (1-10s per test)
- **Examples:**
  - SDK initialization
  - Transaction workflow
  - Event handling
  - Session management

#### 3. **Hardware Tests** (`TestCategory = "Hardware"`)
- **Purpose:** Test with actual Verifone hardware
- **Dependencies:** Physical device or simulator
- **Execution:** Slow (10+ seconds per test)
- **Examples:**
  - Real payment processing
  - Device communication
  - Network connectivity
  - Error recovery

## Test Project Structure

```
VerifonePayment.Tests/
??? Unit/
?   ??? PaymentTests.cs
?   ??? ConfigurationTests.cs
?   ??? ValidationTests.cs
?   ??? EnumTests.cs
??? Integration/
?   ??? SdkIntegrationTests.cs
?   ??? TransactionFlowTests.cs
?   ??? EventHandlingTests.cs
?   ??? SessionManagementTests.cs
??? Hardware/
?   ??? DeviceCommunicationTests.cs
?   ??? RealPaymentTests.cs
?   ??? NetworkTests.cs
??? TestData/
?   ??? Configurations/
?   ??? PaymentSamples/
?   ??? MockResponses/
??? Helpers/
?   ??? TestBase.cs
?   ??? MockSdkFactory.cs
?   ??? TestConfiguration.cs
??? app.config
```

## Usage Examples

### Basic Testing
```powershell
# Run all unit tests
.\scripts\test.ps1

# Run integration tests with simulator
.\scripts\test.ps1 -TestCategory Integration -VerifoneSimulator

# Run all tests with coverage
.\scripts\test.ps1 -TestCategory All -GenerateCoverage
```

### Advanced Scenarios
```powershell
# Debug mode with detailed output
.\scripts\test.ps1 -Configuration Debug -TestCategory Unit

# Hardware tests (requires physical device)
.\scripts\test.ps1 -TestCategory Hardware

# Custom test filter
.\scripts\test.ps1 -TestFilter "FullyQualifiedName~PaymentTransaction"

# Parallel execution for faster feedback
.\scripts\test.ps1 -Parallel -TestCategory Unit
```

### CI/CD Integration
```powershell
# Standard CI pipeline
.\scripts\test.ps1 -Configuration Release -TestCategory Unit

# Pre-deployment validation
.\scripts\test.ps1 -TestCategory Integration -GenerateCoverage

# Performance testing
.\scripts\test.ps1 -TestCategory All -TestTimeout 600
```

## Test Attributes and Categories

### MSTest Attributes
```csharp
[TestClass]
public class PaymentTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Fast")]
    public void PaymentTransaction_ValidAmount_Success() { }

    [TestMethod] 
    [TestCategory("Integration")]
    [TestCategory("Slow")]
    public void PaymentTransaction_WithSdk_ProcessesCorrectly() { }

    [TestMethod]
    [TestCategory("Hardware")]
    [TestCategory("Slow")]
    [Ignore("Requires physical device")]
    public void PaymentTransaction_RealDevice_Success() { }
}
```

### Test Data Management
```csharp
[TestMethod]
[DataRow(100, PaymentType.CREDIT)]
[DataRow(250, PaymentType.DEBIT)]
[DataRow(1000, PaymentType.CREDIT)]
public void PaymentTransaction_DifferentAmounts_Success(long amount, PaymentType type) { }
```

## Mock SDK Configuration

### Using Test Doubles
```csharp
public class MockVerifonePaymentSdk : IPaymentSdk
{
    public bool SimulateFailure { get; set; }
    public TimeSpan ResponseDelay { get; set; }
    
    public PaymentResult StartPayment(Payment payment)
    {
        if (SimulateFailure)
            throw new PaymentException("Simulated failure");
            
        Thread.Sleep(ResponseDelay);
        return new PaymentResult { Success = true };
    }
}
```

## Coverage Targets

| Component | Target Coverage | Priority |
|-----------|----------------|----------|
| **Core Payment Logic** | 95%+ | Critical |
| **Configuration** | 90%+ | High |
| **Event Handling** | 85%+ | High |
| **Error Handling** | 80%+ | Medium |
| **UI Components** | 70%+ | Medium |

## Best Practices

### 1. **Test Naming Convention**
```
MethodName_Scenario_ExpectedResult
PaymentTransaction_NullAmount_ThrowsException
PaymentTransaction_ValidCredit_ReturnsSuccess
```

### 2. **Test Organization**
- One test class per production class
- Group related tests with `[TestCategory]`
- Use descriptive test names
- Keep tests independent

### 3. **Mock Usage**
- Mock external dependencies (Verifone SDK)
- Use real objects for value types
- Verify interactions, not just state
- Keep mocks simple and focused

### 4. **Performance Considerations**
- Unit tests: < 1 second each
- Integration tests: < 10 seconds each
- Hardware tests: < 60 seconds each
- Parallel execution where safe

### 5. **CI/CD Integration**
- Fast feedback loop with unit tests
- Integration tests in pre-deployment
- Hardware tests in staging environment
- Coverage gates for critical components

## Troubleshooting

### Common Issues

1. **Verifone SDK Not Found**
   - Ensure SDK is in bin directory
   - Check package references
   - Use mock mode for unit tests

2. **Test Timeouts**
   - Increase timeout with `-TestTimeout`
   - Check for infinite loops
   - Verify async/await patterns

3. **Hardware Test Failures**
   - Verify device connectivity
   - Check simulator configuration
   - Use appropriate test categories

4. **Coverage Issues**
   - Install Visual Studio Enterprise
   - Use OpenCover as alternative
   - Check file inclusion/exclusion rules

### Debug Mode
```powershell
# Enable detailed logging
$env:VERIFONE_LOG_LEVEL = "Debug"
.\scripts\test.ps1 -Configuration Debug
```

For more information, see the full test documentation in `/docs/testing/`.