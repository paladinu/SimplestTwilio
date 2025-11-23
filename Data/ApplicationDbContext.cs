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

        // Message configuration
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            entity.Property(e => e.Text).IsRequired().HasMaxLength(1600);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");

            // Relationship with History
            entity.HasMany(e => e.Histories)
                  .WithOne(e => e.Message)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // History configuration
        builder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryId);
            entity.Property(e => e.SentDate).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Completed");
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);

            // Indexes for performance
            entity.HasIndex(e => e.MessageId);
            entity.HasIndex(e => e.RecipientListId);
            entity.HasIndex(e => e.SentDate);

            // Relationship with RecipientList
            entity.HasOne(e => e.RecipientList)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientListId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
