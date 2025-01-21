using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;
using Product.Application.Interfaces.Services;
using Product.Domain.Contracts;

namespace Product.Persistence;

public class ProductDBContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ProductDBContext(DbContextOptions<ProductDBContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }
    public DbSet<Domain.Entities.Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
        .SelectMany(t => t.GetProperties())
        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
        {
            property.SetColumnType("nvarchar(128)");
        }

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        string connectionString = "Server=DESKTOP-RQDKA5R\\HERMES;Database=SS_ProductDB;User ID=sa;Password=997997;TrustServerCertificate=True;Trusted_Connection=False;";
        optionsBuilder.UseSqlServer(connectionString);

    }

}
