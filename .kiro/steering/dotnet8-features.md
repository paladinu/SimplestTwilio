---
inclusion: always
---

# .NET 8 Features

## Project Configuration

- Target framework: `net8.0`
- Nullable reference types enabled by default
- Implicit usings enabled

**Auto-included namespaces:**
`System`, `System.Collections.Generic`, `System.Linq`, `System.Threading.Tasks`, `Microsoft.AspNetCore.*`, `Microsoft.Extensions.*`

## Minimal Hosting Model

No `Startup.cs` - all configuration in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```

**Service Lifetimes:**
- Scoped (per request): `builder.Services.AddScoped<IService, Service>()`
- Singleton (app lifetime): `builder.Services.AddSingleton<IService, Service>()`
- Transient (per injection): `builder.Services.AddTransient<IService, Service>()`

## Key .NET 8 Features

**Keyed Services** - Multiple implementations with keys:
```csharp
builder.Services.AddKeyedScoped<INotificationService, SmsNotificationService>("sms");
// Inject: [FromKeyedServices("sms")] INotificationService service
```

**Primary Constructors** - Simplified syntax:
```csharp
public class MessagesController(
    ApplicationDbContext context,
    ITwilioService twilioService) : Controller
{
    // Fields auto-created, access directly
}
```

**Collection Expressions**:
```csharp
List<string> numbers = ["+1234567890", "+0987654321"];
```

**File-Scoped Namespaces**:
```csharp
namespace SimplestTwilio;

public class MyClass { }
```

**Pattern Matching**:
```csharp
if (result is { Success: true, Data: not null }) { }
```

**Required Members**:
```csharp
public class RecipientList
{
    public required string Name { get; set; }
}
```

## EF Core 8

- Improved query performance and change tracking
- Complex types for value objects
- Primitive collections support
- JSON column improvements

## Docker

- Runtime: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Build: `mcr.microsoft.com/dotnet/sdk:8.0`
- Ports: 8080, 8081 (not 80, 443)

## Breaking Changes from .NET Core 3.1

- `Startup.cs` removed - use `Program.cs`
- Nullable reference types enabled by default (use `?` for nullable, `!` for null-forgiving)
- Updated logging categories
