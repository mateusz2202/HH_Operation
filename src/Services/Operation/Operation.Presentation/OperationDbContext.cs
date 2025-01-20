using Microsoft.EntityFrameworkCore;
using Operation.Application.Contracts.Services;
using Operation.Domain.Contracts;
using Operation.Domain.Entities;

namespace Operation.Persistence;

public partial class OperationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public OperationDbContext(DbContextOptions<OperationDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public virtual DbSet<Domain.Entities.Operation> Operations { get; set; }

    public virtual DbSet<OperationPlanned> OperationPlanneds { get; set; }

    public virtual DbSet<OperationStarted> OperationStarteds { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.UserId ?? string.Empty;
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserService?.UserId ?? string.Empty;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserService?.UserId ?? string.Empty;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
           .SelectMany(t => t.GetProperties())
           .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
               .SelectMany(t => t.GetProperties())
               .Where(p => p.Name is "ModifiedBy" or "CreatedBy"))
        {
            property.SetColumnType("nvarchar(128)");
        }

        modelBuilder.Entity<Domain.Entities.Operation>(entity =>
        {
            entity.ToTable("Operation", "HH");

            entity.Property(e => e.Code)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OperationPlanned>(entity =>
        {
            entity.ToTable("OperationPlanned", "HH");

            entity.HasOne(d => d.Operation).WithMany(p => p.OperationPlanneds)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationPlanned_Operation");
        });

        modelBuilder.Entity<OperationStarted>(entity =>
        {
            entity.ToTable("OperationStarted", "HH");

            entity.HasOne(d => d.OperationPlanned).WithMany(p => p.OperationStarteds)
                .HasForeignKey(d => d.OperationPlannedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OperationStarted_Operation");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        string connectionString = "Server=DESKTOP-RQDKA5R\\HERMES;Database=SS_OperationDB;User ID=sa;Password=997997;TrustServerCertificate=True;Trusted_Connection=False;";
        optionsBuilder.UseSqlServer(connectionString);

    }

}