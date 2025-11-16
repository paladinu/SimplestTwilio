---
inclusion: always
---

# Twilio Integration

All SMS functionality uses the official Twilio C# SDK (`Twilio` NuGet package).

## Configuration

Store credentials securely - never hardcode:

```csharp
// appsettings.json structure
{
  "Twilio": {
    "AccountSid": "stored-in-user-secrets",
    "AuthToken": "stored-in-user-secrets",
    "PhoneNumber": "stored-in-user-secrets"
  }
}
```

- Dev: User Secrets
- Prod: Environment variables or Key Vault

## Service Pattern

Create `ITwilioService` interface with async methods:

```csharp
public interface ITwilioService
{
    Task<bool> SendSmsAsync(string to, string message);
    Task<List<MessageResult>> SendBulkSmsAsync(List<string> recipients, string message);
}
```

- Proper error handling
- Log all API interactions
- Use async/await for all calls

## Error Handling

**Common errors:**
- Invalid phone number format
- Insufficient account balance
- Invalid credentials
- Rate limiting
- Network timeouts

**Strategy:**
- Catch `TwilioException` specifically
- Log details for debugging
- Return user-friendly messages
- Track failures in history
- Retry logic for transient failures

## Phone Number Format

- Validate before sending
- Use E.164 format: `+[country code][number]`
- Clear validation messages
- Support international numbers

## Rate Limiting

- Respect Twilio's limits
- Batch bulk sends
- Add delays if needed
- Monitor usage and costs

## Best Practices

- Validate all recipient numbers
- SMS limit: 160 characters per segment
- Track message status and delivery
- Proper logging for audit trails
- Handle opt-out/unsubscribe
- Consider costs in bulk operations
- Use test credentials for development
- Mock service for unit tests
