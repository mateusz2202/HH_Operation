using Document.Application.Interfaces.Services;
using Document.Domain.Contracts;
using Document.Domain.Entities;
using Document.Domain.ExtendedAttributes;
using Microsoft.EntityFrameworkCore;


namespace Document.Persistence;

public class DocumentDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public DocumentDbContext(DbContextOptions<DocumentDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Domain.Entities.Document> Documents { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserService.UserId;
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
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

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        string connectionString = "Server=DESKTOP-RQDKA5R\\HERMES;Database=SS_DocumentDB;User ID=sa;Password=997997;TrustServerCertificate=True;Trusted_Connection=False;";
        optionsBuilder.UseSqlServer(connectionString);

    }
}
