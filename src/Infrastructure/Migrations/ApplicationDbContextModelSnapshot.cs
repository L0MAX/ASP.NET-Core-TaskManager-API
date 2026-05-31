using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManager.Infrastructure.Data;

#nullable disable

namespace TaskManager.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("TaskManager.Domain.Entities.TaskItem", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasMaxLength(2000)
                    .HasColumnType("nvarchar(2000)");

                b.Property<DateTime?>("DueDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Priority")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnType("nvarchar(32)");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnType("nvarchar(32)");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("UserId", "Status");

                b.ToTable("Tasks", (string)null);
            });

        modelBuilder.Entity("TaskManager.Domain.Entities.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnType("nvarchar(512)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Users", (string)null);
            });

        modelBuilder.Entity("TaskManager.Domain.Entities.TaskItem", b =>
            {
                b.HasOne("TaskManager.Domain.Entities.User", "User")
                    .WithMany("Tasks")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

        modelBuilder.Entity("TaskManager.Domain.Entities.User", b =>
            {
                b.Navigation("Tasks");
            });
#pragma warning restore 612, 618
    }
}
