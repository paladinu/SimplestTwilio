---
inclusion: always
---

# Coding Standards

## Naming Conventions

- **PascalCase**: Classes, methods, properties, constants, interfaces (prefix `I`)
- **camelCase with `_` prefix**: Private fields (`_twilioClient`)
- **camelCase**: Local variables, parameters

## Code Organization

- One class per file, filename matches class name
- File-scoped namespaces: `namespace SimplestTwilio;`
- Leverage implicit usings (.NET 8 default)
- Explicit access modifiers always
- Nullable reference types enabled

## Controller Patterns

- Keep controllers thin - delegate to services
- Use async/await for I/O (DB, API calls)
- Return appropriate HTTP status codes
- Use strongly-typed view models
- Follow RESTful conventions

```csharp
public class ExampleController : Controller
{
    private readonly IExampleService _service;
    
    public ExampleController(IExampleService service)
    {
        _service = service;
    }
    
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetDataAsync();
        return View(data);
    }
}
```

## Model Design

- Data annotations for validation
- Separate domain models from view models
- Navigation properties for related entities
- Keep focused on data representation

## Service Layer

- Create interfaces for business logic
- Register in `Program.cs`: `builder.Services.AddScoped<IService, Service>()`
- Constructor injection for dependencies
- Single responsibility per service

## Error Handling

- Try-catch for expected exceptions
- Log errors with appropriate level
- User-friendly messages in UI
- ModelState for validation errors
- Graceful Twilio API error handling

## Async Patterns

- Async/await for all I/O operations
- Suffix async methods with "Async"
- Avoid async void (except event handlers)

## Documentation

- XML comments for public APIs
- Comment complex business logic only
- Avoid obvious comments
- Keep comments current with code
