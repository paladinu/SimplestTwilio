# List Management Design Document

## Overview

The list management feature enables users to create, view, edit, and delete recipient lists containing contacts with phone numbers and optional names. This feature builds upon the existing SimplestTwilio MVC architecture and uses Entity Framework Core for data persistence.

## Architecture

### High-Level Architecture

The feature follows the standard ASP.NET Core MVC pattern with the following layers:

- **Presentation Layer**: Razor views for user interface
- **Controller Layer**: ListsController handling HTTP requests and responses
- **Data Access Layer**: Entity Framework Core with ApplicationDbContext
- **Domain Layer**: Entity models (RecipientList, Contact)

### Technology Stack

- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0 with SQL Server
- Razor views with Bootstrap for UI
- Data annotations for validation

## Components and Interfaces

### 1. Data Models

#### RecipientList Entity

```csharp
public class RecipientList
{
    public int RecipientListId { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    [Display(Name = "List Name")]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    
    [NotMapped]
    public int ContactCount => Contacts?.Count ?? 0;
}
```

#### Contact Entity

```csharp
public class Contact
{
    public int ContactId { get; set; }
    
    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [StringLength(100)]
    [Display(Name = "Contact Name")]
    public string? Name { get; set; }
    
    [Required]
    public int RecipientListId { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    // Navigation property
    public virtual RecipientList RecipientList { get; set; } = null!;
}
```

### 2. Database Context

#### ApplicationDbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<RecipientList> RecipientLists { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<History> Histories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // RecipientList configuration
        builder.Entity<RecipientList>(entity =>
        {
            entity.HasKey(e => e.RecipientListId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            
            // Relationship with Contact
            entity.HasMany(e => e.Contacts)
                  .WithOne(e => e.RecipientList)
                  .HasForeignKey(e => e.RecipientListId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Contact configuration
        builder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
        });
    }
}
```

### 3. Controller

#### ListsController

The controller will handle all CRUD operations for recipient lists and contacts:

**Actions:**
- `Index()` - Display all lists for the current user
- `Details(int id)` - Display a specific list with all contacts
- `Create()` - Display create list form
- `Create(RecipientList model)` - Process list creation
- `Edit(int id)` - Display edit list form
- `Edit(int id, RecipientList model)` - Process list update
- `Delete(int id)` - Display delete confirmation
- `DeleteConfirmed(int id)` - Process list deletion
- `AddContact(int listId)` - Display add contact form
- `AddContact(Contact model)` - Process contact addition
- `RemoveContact(int id)` - Process contact removal

**Key Responsibilities:**
- Use async/await for all database operations
- Return appropriate HTTP status codes
- Handle errors gracefully with user-friendly messages
- Validate input data before processing

### 4. View Models

#### RecipientListsViewModel

```csharp
public class RecipientListsViewModel
{
    public List<RecipientListSummary> Lists { get; set; } = new();
}

public class RecipientListSummary
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

#### RecipientListDetailsViewModel

```csharp
public class RecipientListDetailsViewModel
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ContactSummary> Contacts { get; set; } = new();
}

public class ContactSummary
{
    public int ContactId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
}
```

### 5. Views

#### Index.cshtml
- Display table/cards of all recipient lists
- Show list name, contact count, created date
- Provide action buttons: View Details, Edit, Delete, Create New

#### Details.cshtml
- Display list name and metadata
- Show table of all contacts with phone numbers and names
- Provide actions: Add Contact, Remove Contact, Edit List, Back to Lists

#### Create.cshtml
- Form with list name input
- Validation messages
- Submit and Cancel buttons

#### Edit.cshtml
- Form pre-populated with current list name
- Validation messages
- Submit and Cancel buttons

#### Delete.cshtml
- Confirmation page showing list details
- Warning about deleting all contacts
- Confirm and Cancel buttons

#### AddContact.cshtml (Partial or Modal)
- Form with phone number (required) and name (optional)
- Validation messages
- Submit and Cancel buttons

## Data Models

### Entity Relationships

```
RecipientList 1 ──── * Contact
```

### Database Schema

**RecipientLists Table:**
- RecipientListId (PK, int, identity)
- Name (nvarchar(100), required)
- CreatedDate (datetime2, default GETDATE())

**Contacts Table:**
- ContactId (PK, int, identity)
- PhoneNumber (nvarchar(20), required)
- Name (nvarchar(100), nullable)
- RecipientListId (int, required, FK to RecipientLists)
- CreatedDate (datetime2, default GETDATE())

### Indexes

- Contacts.RecipientListId (for filtering by list)

## Error Handling

### Validation Errors

- **Server-side**: Validate ModelState and return validation errors
- Display field-level errors using validation tag helpers
- Display summary errors at the top of forms

### Not Found Errors

- Return 404 Not Found if list doesn't exist
- Return 404 Not Found if contact doesn't exist
- Display user-friendly error page

### Database Errors

- Wrap database operations in try-catch blocks
- Log errors using ILogger
- Display generic error messages to users
- Provide retry options where appropriate

### Phone Number Validation

- Use [Phone] data annotation for basic validation
- Consider additional validation for E.164 format
- Provide clear error messages for invalid formats
- Example valid formats: +1234567890, +44 20 7946 0958

## Testing Strategy

### Unit Testing (Optional)

- Test controller actions with mocked DbContext
- Test validation logic on models
- Test phone number validation

### Integration Testing (Optional)

- Test full CRUD workflows
- Test cascade deletion of contacts when list is deleted
- Test database constraints and relationships

### Manual Testing

- Test all CRUD operations through UI
- Test validation messages display correctly
- Test edge cases (empty lists, special characters in names)
- Test phone number validation with various formats
- Test cascade deletion behavior

## Security Considerations

### Data Validation

- Validate all input server-side
- Sanitize phone numbers before storage
- Limit string lengths to prevent overflow
- Use parameterized queries (EF Core handles this)
- Use anti-forgery tokens on all POST actions

### Data Integrity

- Use database constraints for required fields
- Implement cascade deletion for related entities
- Audit trail via CreatedDate fields

## Implementation Notes

### Migration Strategy

1. Create ApplicationDbContext if it doesn't exist
2. Create initial migration with RecipientList and Contact entities
3. Update existing RecipientList model (currently has Emails property)
4. Apply migration to database
5. Update Program.cs to register DbContext

### Phone Number Handling

- Store phone numbers as strings
- Validate format using [Phone] attribute
- Consider normalizing to E.164 format for consistency
- Display formatted numbers in UI

### Performance Considerations

- Use AsNoTracking() for read-only queries (Index, Details)
- Use Include() to eager load Contacts with RecipientList
- Implement pagination if lists grow large
- Add indexes on foreign keys

### Future Enhancements

- Bulk import contacts from CSV
- Export contacts to CSV
- Search/filter contacts within a list
- Duplicate detection for phone numbers
- Contact groups or tags
- Integration with message sending workflow
