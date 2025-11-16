using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimplestTwilio.Data;

#nullable disable

namespace SimplestTwilio.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SimplestTwilio.Models.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContactId"));

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("RecipientListId")
                        .HasColumnType("int");

                    b.HasKey("ContactId");

                    b.HasIndex("RecipientListId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("SimplestTwilio.Models.RecipientList", b =>
                {
                    b.Property<int>("RecipientListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipientListId"));

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RecipientListId");

                    b.ToTable("RecipientLists");
                });

            modelBuilder.Entity("SimplestTwilio.Models.Contact", b =>
                {
                    b.HasOne("SimplestTwilio.Models.RecipientList", "RecipientList")
                        .WithMany("Contacts")
                        .HasForeignKey("RecipientListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RecipientList");
                });

            modelBuilder.Entity("SimplestTwilio.Models.RecipientList", b =>
                {
                    b.Navigation("Contacts");
                });
#pragma warning restore 612, 618
        }
    }
}
