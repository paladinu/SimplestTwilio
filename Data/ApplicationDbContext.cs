using Microsoft.EntityFrameworkCore;
using SimplestTwilio.Models;

namespace SimplestTwilio.Data;

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
        });
    }
}
