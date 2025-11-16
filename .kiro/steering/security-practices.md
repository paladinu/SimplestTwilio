---
inclusion: always
---

# Security Practices

## Authentication

No authentication implemented - left as exercise for forkers.

## Credential Management

**Never hardcode credentials**

Development: Use User Secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "Twilio:AccountSid" "your-sid"
dotnet user-secrets set "Twilio:AuthToken" "your-token"
dotnet user-secrets set "Twilio:PhoneNumber" "your-number"
```

Production: Use environment variables or Azure Key Vault

**Connection Strings**
- Store in `appsettings.json` (not credentials)
- Never commit production credentials
- Use integrated security when possible

## Input Validation

- Data annotations for model validation
- Always validate server-side
- Check `ModelState.IsValid` before processing
- Validate phone number format before SMS
- Sanitize input to prevent injection

## SQL Injection Prevention

- EF Core uses parameterized queries automatically
- Never concatenate user input into SQL
- Use LINQ queries over raw SQL
- Validate any raw SQL parameters

## XSS Prevention

- Razor auto-encodes output
- Use `@Html.Raw()` only for trusted content
- Validate and sanitize rich text input

## CSRF Protection

- Use anti-forgery tokens on all forms
- Razor includes `@Html.AntiForgeryToken()` automatically
- Validate with `[ValidateAntiForgeryToken]` on POST actions

## HTTPS

- Enforce HTTPS in production
- Use HSTS headers
- Redirect HTTP to HTTPS

## Logging Best Practices

**DO log:**
- SMS sending operations (with recipient count, not numbers)
- API errors and exceptions
- Security events

**NEVER log:**
- Passwords or auth tokens
- Full phone numbers (mask them)
- Twilio credentials
- PII

```csharp
// Good
_logger.LogInformation("SMS sent to {RecipientCount} recipients", recipients.Count);
// Bad
_logger.LogInformation("Auth token: {Token}", authToken);
```

## Rate Limiting

- Implement per-user rate limiting
- Prevent SMS abuse
- Track API usage and costs
- Set reasonable bulk operation limits

## Data Privacy

- Store only necessary data
- Implement retention policies
- Provide export/deletion capabilities
- Comply with GDPR and privacy regulations

## Error Messages

- Generic messages for users
- Detailed logging server-side
- Don't expose system internals or DB structure
