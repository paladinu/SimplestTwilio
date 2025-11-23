# SMS Messaging Design Document

## Overview

The SMS messaging feature enables users to compose message templates, select one or more recipient lists, and send bulk SMS messages through their Twilio account. The feature integrates with the existing list management system and provides comprehensive tracking of sent messages with delivery status. The design follows ASP.NET Core MVC patterns with a dedicated service layer for Twilio integration.

## Architecture

### High-Level Architecture

The feature follows a layered architecture pattern:

- **Presentation Layer**: Razor views for message management and sending interface
- **Controller Layer**: MessagesController and HistoryController handling HTTP requests
- **Service Layer**: TwilioService encapsulating all Twilio API interactions
- **Data Access Layer**: Entity Framework Core with ApplicationDbContext
- **Domain Layer**: Entity models (Message, History, RecipientList, Contact)

### Technology Stack

- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0 with SQLite
- Twilio C# SDK (Twilio NuGet package)
- Razor views with Bootstrap 5 for UI
- User Secrets for credential management (development)
- Async/await for all I/O operations

### Integration Points

- **List Management Feature**: Reads recipient lists and contacts for message targeting
- **Twilio API**: External SMS delivery service via official C# SDK
- **Configuration System**: Reads Twilio credentials from appsettings.json/user secrets

## Components and Interfaces

### 1. Data Models

#### Message Entity (Updated)

```csharp
public class Message
{
    public int MessageId { get; set; }
    
    [Required]
    [StringLength(1600, MinimumLength = 1)]
    [Display(Name = "Message Text")]
    public string Text { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
```

#### History Entity (Updated)

```csharp
public class History
{
    public int HistoryId { get; set; }
    
    [Required]
    public int MessageId { get; set; }
    
    [Required]
    public int RecipientListId { get; set; }
    
    public DateTime SentDate { get; set; }
    
    [Required]
    public int TotalRecipients { get; set; }
    
    [Required]
    public int SuccessfulSends { get; set; }
    
    [Required]
    public int FailedSends { get; set; }
    
    [StringLength(50)]
    public string Status { get; set; } = "Completed";
    
    [StringLength(500)]
    public string? ErrorMessage { get; set; }
    
    // Navigation properties
    public virtual Message? Message { get; set; }
    public virtual RecipientList? RecipientList { get; set; }
}
```

#### TwilioConfiguration

```csharp
public class TwilioConfiguration
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
```

### 2. Service Layer

#### ITwilioService Interface

```csharp
public interface ITwilioService
{
    Task<SendResult> SendSmsAsync(string to, string message);
    Task<BulkSendResult> SendBulkSmsAsync(List<string> recipients, string message);
    bool ValidateConfiguration();
    int CalculateSmsSegments(string message);
}
```

#### SendResult Model

```csharp
public class SendResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MessageSid { get; set; }
}
```

#### BulkSendResult Model

```csharp
public class BulkSendResult
{
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public List<SendFailure> Failures { get; set; } = new();
}

public class SendFailure
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
```

#### TwilioService Implementation

```csharp
public class TwilioService : ITwilioService
{
    private readonly TwilioConfiguration _config;
    private readonly ILogger<TwilioService> _logger;
    
    public TwilioService(IConfiguration configuration, ILogger<TwilioService> logger)
    {
        _config = configuration.GetSection("Twilio").Get<TwilioConfiguration>() 
                  ?? new TwilioConfiguration();
        _logger = logger;
        
        if (ValidateConfiguration())
        {
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }
    }
    
    public bool ValidateConfiguration()
    {
        return !string.IsNullOrWhiteSpace(_config.AccountSid) &&
               !string.IsNullOrWhiteSpace(_config.AuthToken) &&
               !string.IsNullOrWhiteSpace(_config.PhoneNumber);
    }
    
    public async Task<SendResult> SendSmsAsync(string to, string message)
    {
        // Implementation using Twilio SDK
    }
    
    public async Task<BulkSendResult> SendBulkSmsAsync(List<string> recipients, string message)
    {
        // Implementation with error handling and retry logic
    }
    
    public int CalculateSmsSegments(string message)
    {
        // GSM-7 encoding: 160 chars per segment
        // Unicode: 70 chars per segment
        // Concatenated messages have overhead
    }
}
```

### 3. Controller Layer

#### MessagesController (Updated)

**Actions:**
- `Index()` - Display all message templates
- `Create()` - Display create message form
- `Create(Message model)` - Process message creation
- `Edit(int id)` - Display edit message form
- `Edit(int id, Message model)` - Process message update
- `Delete(int id)` - Display delete confirmation
- `DeleteConfirmed(int id)` - Process message deletion
- `Send(int id)` - Display send interface with list selection
- `SendConfirm(SendMessageViewModel model)` - Display send preview
- `SendExecute(SendMessageViewModel model)` - Execute bulk send

**Dependencies:**
- ApplicationDbContext for data access
- ITwilioService for SMS sending
- ILogger for error logging

#### HistoryController (Updated)

**Actions:**
- `Index()` - Display all message history
- `Details(int id)` - Display detailed history entry with failures

### 4. View Models

#### SendMessageViewModel

```csharp
public class SendMessageViewModel
{
    public int MessageId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Please select at least one recipient list")]
    public List<int> SelectedListIds { get; set; } = new();
    
    public List<RecipientListOption> AvailableLists { get; set; } = new();
    
    // Calculated properties
    public int TotalRecipients { get; set; }
    public int SmsSegments { get; set; }
    public int TotalSmsCount { get; set; }
}

public class RecipientListOption
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public bool IsSelected { get; set; }
}
```

#### SendResultViewModel

```csharp
public class SendResultViewModel
{
    public string MessageText { get; set; } = string.Empty;
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public List<string> ListNames { get; set; } = new();
    public List<SendFailure> Failures { get; set; } = new();
}
```

#### MessageIndexViewModel

```csharp
public class MessageIndexViewModel
{
    public List<MessageSummary> Messages { get; set; } = new();
    public bool TwilioConfigured { get; set; }
}

public class MessageSummary
{
    public int MessageId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string TextPreview { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int TimesSent { get; set; }
}
```

#### HistoryIndexViewModel

```csharp
public class HistoryIndexViewModel
{
    public List<HistorySummary> Histories { get; set; } = new();
}

public class HistorySummary
{
    public int HistoryId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public string ListName { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

### 5. Views

#### Messages/Index.cshtml
- Display table of all message templates
- Show text preview (first 100 chars), created date, times sent
- Action buttons: Send, Edit, Delete, Create New
- Warning banner if Twilio not configured

#### Messages/Create.cshtml
- Form with textarea for message text
- Character counter showing remaining characters
- SMS segment calculator
- Submit and Cancel buttons

#### Messages/Edit.cshtml
- Form pre-populated with message text
- Character counter and segment calculator
- Submit and Cancel buttons

#### Messages/Delete.cshtml
- Confirmation page with message preview
- Warning about preserving history
- Confirm and Cancel buttons

#### Messages/Send.cshtml
- Display message text
- Checkboxes for selecting recipient lists
- Show contact count for each list
- Calculate and display total recipients
- Next button to preview

#### Messages/SendConfirm.cshtml
- Display message text
- Show selected lists and total recipients
- Calculate and display SMS segments
- Calculate and display total SMS count
- Confirm Send and Back buttons

#### Messages/SendResult.cshtml
- Display success/failure summary
- Show successful sends count
- Show failed sends with details (phone number, error)
- Back to Messages button

#### History/Index.cshtml
- Display table of all sent messages
- Show message preview, list names, sent date, status
- Show success/failure counts
- Link to view details
- Filter/sort options

#### History/Details.cshtml
- Display full message text
- Show recipient list details
- Display detailed failure information
- Back to History button

## Data Models

### Entity Relationships

```
Message 1 ──── * History
RecipientList 1 ──── * History
RecipientList 1 ──── * Contact
```

### Database Schema Updates

**Messages Table (Updated):**
- MessageId (PK, int, identity)
- Text (nvarchar(1600), required)
- CreatedDate (datetime2, default GETDATE())

**Histories Table (Updated):**
- HistoryId (PK, int, identity)
- MessageId (int, required, FK to Messages)
- RecipientListId (int, required, FK to RecipientLists)
- SentDate (datetime2, default GETDATE())
- TotalRecipients (int, required)
- SuccessfulSends (int, required)
- FailedSends (int, required)
- Status (nvarchar(50), required, default 'Completed')
- ErrorMessage (nvarchar(500), nullable)

### Indexes

- Histories.MessageId (for filtering by message)
- Histories.RecipientListId (for filtering by list)
- Histories.SentDate (for sorting by date)

## Error Handling

### Twilio API Errors

**Common Errors:**
- Invalid credentials (401 Unauthorized)
- Invalid phone number format (400 Bad Request)
- Insufficient account balance (402 Payment Required)
- Rate limiting (429 Too Many Requests)
- Network timeouts

**Handling Strategy:**
- Catch TwilioException specifically
- Log detailed error with stack trace
- Return user-friendly error messages
- Continue processing remaining recipients
- Track failures in BulkSendResult

### Validation Errors

- Validate message text length (1-1600 characters)
- Validate at least one list selected
- Validate Twilio configuration before sending
- Validate phone numbers in E.164 format
- Display field-level and summary errors

### Database Errors

- Wrap all database operations in try-catch
- Log errors with ILogger
- Display generic error messages to users
- Provide retry options where appropriate

### Configuration Errors

- Check Twilio configuration on service initialization
- Display warning banner if not configured
- Prevent sending if configuration invalid
- Provide clear instructions for configuration

## Testing Strategy

### Unit Testing

- Test TwilioService with mocked Twilio client
- Test SMS segment calculation logic
- Test phone number validation
- Test controller actions with mocked dependencies
- Test view model mapping logic

### Integration Testing

- Test full send workflow with Twilio test credentials
- Test database operations with in-memory database
- Test error handling with invalid credentials
- Test bulk send with mixed valid/invalid numbers

### Property-Based Testing

Property-based testing will be used to verify universal correctness properties across all inputs using a PBT library appropriate for C# (such as FsCheck or CsCheck).

**Configuration:**
- Each property-based test will run a minimum of 100 iterations
- Each test will be tagged with a comment referencing the design document property
- Tag format: `// Feature: sms-messaging, Property {number}: {property_text}`


## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Message text validation consistency

*For any* message text input (create or edit operation), the system should accept text between 1 and 1600 characters and reject text outside this range with a validation error

**Validates: Requirements 1.2, 3.2**

### Property 2: Message creation persistence

*For any* valid message text, when a message is created, querying the database should return a message with that exact text

**Validates: Requirements 1.1**

### Property 3: Message list completeness

*For any* set of created messages, navigating to the messages index page should display all messages with their text preview, creation date, and action buttons

**Validates: Requirements 2.1, 2.2, 2.3, 2.4**

### Property 4: Message update persistence

*For any* existing message and valid new text, when the message is updated, querying the database should return the message with the updated text

**Validates: Requirements 3.1**

### Property 5: Message deletion removes from database

*For any* message, when it is deleted, querying the database for that message should return no results

**Validates: Requirements 4.1**

### Property 6: History preservation after message deletion

*For any* message with associated history records, deleting the message should not delete the history records

**Validates: Requirements 4.5**

### Property 7: Recipient list display completeness

*For any* set of recipient lists, the send page should display all lists with their names and contact counts

**Validates: Requirements 5.1, 5.3**

### Property 8: List selection validation

*For any* send attempt with zero selected lists, the system should reject the operation with a validation error

**Validates: Requirements 5.4**

### Property 9: Twilio credential validation

*For any* credential set, the system should validate that Account SID, Auth Token, and Phone Number are all present and the phone number is in E.164 format

**Validates: Requirements 6.1, 6.2, 6.4**

### Property 10: Bulk send completeness

*For any* message and set of selected recipient lists, sending should deliver the message to all contacts across all selected lists

**Validates: Requirements 7.1**

### Property 11: Phone number validation before send

*For any* set of contacts in selected lists, all phone numbers should be validated as E.164 format before any messages are sent

**Validates: Requirements 7.4**

### Property 12: Send result summary accuracy

*For any* bulk send operation, the result summary should accurately report the total number of successful and failed deliveries

**Validates: Requirements 7.5**

### Property 13: History record creation

*For any* message sent to N recipient lists, exactly N history records should be created (one per list)

**Validates: Requirements 7.6**

### Property 14: History display completeness

*For any* set of history records, the history page should display all records with message text, list name, sent date, delivery status, and success/failure counts

**Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5**

### Property 15: History ordering

*For any* set of history records, they should be ordered by sent date in descending order (most recent first)

**Validates: Requirements 8.6**

### Property 16: Send failure identification

*For any* bulk send with failures, each failure should identify the specific contact (phone number and name if available) that caused the error

**Validates: Requirements 9.3**

### Property 17: Send resilience

*For any* bulk send operation where some recipients fail, the system should continue sending to all remaining recipients and report both successes and failures

**Validates: Requirements 9.6**

### Property 18: Recipient count calculation

*For any* set of selected recipient lists, the total recipient count should equal the sum of contact counts across all selected lists

**Validates: Requirements 10.1, 10.2**

### Property 19: SMS segment calculation

*For any* message text, the segment count should follow SMS segmentation rules: 160 characters per segment for GSM-7 encoding, 70 characters per segment for Unicode, with concatenation overhead for multi-segment messages

**Validates: Requirements 10.3, 10.4**

## Security Considerations

### Credential Management

- Store Twilio credentials in User Secrets during development
- Use environment variables or Azure Key Vault in production
- Never log or display credentials in UI or logs
- Validate credential format before storage
- Use IConfiguration to access credentials securely

### Input Validation

- Validate all input server-side with data annotations
- Sanitize message text to prevent injection attacks
- Validate phone numbers against E.164 format
- Limit message text length to prevent overflow
- Use anti-forgery tokens on all POST actions

### Data Privacy

- Mask phone numbers in logs (show only last 4 digits)
- Log recipient counts, not actual phone numbers
- Implement proper error messages that don't expose system internals
- Consider GDPR compliance for contact data storage
- Provide data export/deletion capabilities

### Rate Limiting

- Implement per-user rate limiting for sends
- Track API usage to prevent abuse
- Set reasonable limits on bulk operations
- Monitor Twilio usage and costs
- Implement cooldown periods between bulk sends

### Error Handling

- Never expose Twilio credentials in error messages
- Log detailed errors server-side only
- Display generic error messages to users
- Sanitize error messages from Twilio API
- Implement proper exception handling for all API calls

## Performance Considerations

### Bulk Sending

- Send messages asynchronously to avoid blocking UI
- Implement batching for very large recipient lists
- Add delays between sends to respect rate limits
- Use Task.WhenAll for parallel sends where appropriate
- Consider background job processing for large sends

### Database Queries

- Use AsNoTracking() for read-only queries
- Use Include() to eager load related entities
- Implement pagination for message and history lists
- Add indexes on foreign keys and date columns
- Optimize queries to avoid N+1 problems

### Caching

- Cache Twilio configuration on service initialization
- Cache recipient list counts where appropriate
- Invalidate caches on data changes
- Consider distributed caching for multi-instance deployments

## Implementation Notes

### Migration Strategy

1. Update Message entity with Text property and validation
2. Update History entity with new tracking fields
3. Create migration for schema changes
4. Apply migration to update database
5. Create TwilioService and interface
6. Register service in Program.cs
7. Update MessagesController with new actions
8. Update HistoryController with new actions
9. Create all required views
10. Configure Twilio credentials in user secrets

### Twilio Integration

- Use official Twilio C# SDK (Twilio NuGet package)
- Initialize TwilioClient once in service constructor
- Use async methods for all API calls
- Implement proper error handling for TwilioException
- Test with Twilio test credentials during development
- Use Twilio test phone numbers for integration testing

### SMS Segment Calculation

**GSM-7 Encoding (default):**
- Single segment: 160 characters
- Multi-segment: 153 characters per segment (7 char overhead)

**Unicode Encoding (when non-GSM chars present):**
- Single segment: 70 characters
- Multi-segment: 67 characters per segment (3 char overhead)

**Detection Logic:**
- Check if message contains only GSM-7 characters
- If any Unicode characters present, use Unicode calculation
- Account for concatenation overhead in multi-segment messages

### User Secrets Configuration

Development setup:
```bash
dotnet user-secrets init
dotnet user-secrets set "Twilio:AccountSid" "your-account-sid"
dotnet user-secrets set "Twilio:AuthToken" "your-auth-token"
dotnet user-secrets set "Twilio:PhoneNumber" "+1234567890"
```

### Future Enhancements

- Scheduled message sending
- Message templates with variable substitution
- SMS delivery status webhooks
- Message analytics and reporting
- A/B testing for message content
- Opt-out/unsubscribe management
- Message queuing for large batches
- Retry logic for failed sends
- Cost estimation before sending
- Message personalization per contact
