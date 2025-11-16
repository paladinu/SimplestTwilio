---
inclusion: manual
---

# Testing Guidelines

## Testing Philosophy

- Focus on core functional logic
- Write minimal, focused tests
- Test business logic, not framework code
- Mock external dependencies (Twilio API, database)

## Test Project Structure

- Create separate test project: `SimplestTwilio.Tests`
- Use xUnit as testing framework
- Use Moq for mocking dependencies
- Organize tests by feature/controller

## Unit Testing

### What to Test

- Business logic in services
- Controller action logic
- Model validation
- Data transformations
- Error handling

### What NOT to Test

- Framework code (EF Core, ASP.NET Core)
- Third-party libraries (Twilio SDK)
- Simple property getters/setters
- Auto-generated code

### Example Unit Test

```csharp
public class MessageServiceTests
{
    [Fact]
    public async Task SendBulkSms_ValidRecipients_ReturnsSuccess()
    {
        // Arrange
        var mockTwilioService = new Mock<ITwilioService>();
        var service = new MessageService(mockTwilioService.Object);
        var recipients = new List<string> { "+1234567890", "+0987654321" };
        
        // Act
        var result = await service.SendBulkSmsAsync(recipients, "Test message");
        
        // Assert
        Assert.True(result.Success);
    }
}
```

## Integration Testing

### Database Testing

- Use in-memory database for tests
- Create fresh context for each test
- Clean up test data after tests

### Example Integration Test

```csharp
public class MessagesControllerTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task Create_ValidMessage_SavesToDatabase()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var controller = new MessagesController(context);
        var message = new Message { Text = "Test" };
        
        // Act
        await controller.Create(message);
        
        // Assert
        Assert.Equal(1, await context.Messages.CountAsync());
    }
}
```

## Mocking External Services

### Twilio Service Mocking

```csharp
var mockTwilioService = new Mock<ITwilioService>();
mockTwilioService
    .Setup(s => s.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
    .ReturnsAsync(true);
```

## Test Naming Convention

- Use descriptive test names
- Format: `MethodName_Scenario_ExpectedResult`
- Examples:
  - `SendSms_InvalidPhoneNumber_ReturnsFalse`
  - `CreateRecipientList_ValidData_SavesSuccessfully`

## Test Coverage

- Aim for high coverage of business logic
- Don't chase 100% coverage
- Focus on critical paths and edge cases
- Test error handling scenarios

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~MessageServiceTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Continuous Integration

- Run tests on every commit
- Fail builds on test failures
- Monitor test execution time
- Keep tests fast and reliable
