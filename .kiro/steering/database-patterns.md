---
inclusion: always
---

# Database Patterns

## DbContext Setup

Single `ApplicationDbContext` inheriting from `IdentityDbContext`:

```csharp
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<Message> Messages { get; set; }
    public DbSet<RecipientList> RecipientLists { get; set; }
    public DbSet<History> Histories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configure relationships and constraints
    }
}
```

Register in `Program.cs`: `builder.Services.AddDbContext<ApplicationDbContext>()`

## Entity Design

**Primary Keys**
- Use `int` type
- Name as `[EntityName]Id` (e.g., `MessageId`)
- Let EF Core handle identity generation

**Navigation Properties**
- Use `List<T>` for collections
- Configure relationships in `OnModelCreating`

**Data Annotations**
- `[Required]` for mandatory fields
- `[StringLength]` for text limits
- `[Display]` for UI labels

## Query Patterns

Always use async methods with proper loading:

```csharp
// Read-only query
var messages = await _context.Messages
    .AsNoTracking()
    .OrderByDescending(m => m.MessageId)
    .ToListAsync();

// With related data
var history = await _context.Histories
    .Include(h => h.Message)
    .Include(h => h.RecipientList)
    .FirstOrDefaultAsync(h => h.HistoryId == id);
```

**Performance Rules**
- Use `AsNoTracking()` for read-only queries
- Use `Include()` for eager loading to avoid N+1 queries
- Paginate large result sets
- Index frequently queried columns

## Migrations

```bash
dotnet ef migrations add DescriptiveName
dotnet ef database update
```

**Best Practices**
- Descriptive migration names
- Review generated code before applying
- Test on dev database first
- Keep in source control
- Never modify applied migrations

## Connection Strings

- Store in `appsettings.json`
- SQLite: `Data Source=SimplestTwilio.db`
- Database file created automatically on first run
- Add `*.db` to `.gitignore` to avoid committing database
- Use different database files per environment
