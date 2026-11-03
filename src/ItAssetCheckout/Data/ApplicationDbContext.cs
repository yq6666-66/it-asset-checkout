using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItAssetCheckout.Data;

// EF Core context for Identity accounts, assets, staff, and checkouts.
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    // Accepts the provider options chosen in Program.cs.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Staff> Staff => Set<Staff>();

    public DbSet<Checkout> Checkouts => Set<Checkout>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Asset>(entity =>
        {
            entity.HasIndex(asset => asset.TagNo).IsUnique();
            entity.Property(asset => asset.TagNo).HasMaxLength(32).IsRequired();
            entity.Property(asset => asset.Name).HasMaxLength(100).IsRequired();
            entity.Property(asset => asset.Category).HasMaxLength(50).IsRequired();
            entity.Property(asset => asset.Status).HasMaxLength(30).IsRequired();
            entity.Property(asset => asset.Value).HasPrecision(12, 2);
        });

        builder.Entity<Staff>(entity =>
        {
            entity.HasIndex(staff => staff.StaffCode).IsUnique();
            entity.Property(staff => staff.StaffCode).HasMaxLength(32).IsRequired();
            entity.Property(staff => staff.FullName).HasMaxLength(100).IsRequired();
            entity.Property(staff => staff.Email).HasMaxLength(200).IsRequired();
            entity.Property(staff => staff.Department).HasMaxLength(80).IsRequired();
        });

        var provider = Database.ProviderName ?? string.Empty;
        var openCheckoutFilter = provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase)
            ? "\"ReturnAt\" IS NULL"
            : "[ReturnAt] IS NULL";

        builder.Entity<Checkout>(entity =>
        {
            entity.Property(checkout => checkout.Note).HasMaxLength(500);
            entity.HasIndex(checkout => checkout.AssetId)
                .IsUnique()
                .HasDatabaseName("IX_Checkouts_OneOpenPerAsset")
                .HasFilter(openCheckoutFilter);
            entity.HasOne(checkout => checkout.Asset)
                .WithMany()
                .HasForeignKey(checkout => checkout.AssetId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(checkout => checkout.Staff)
                .WithMany()
                .HasForeignKey(checkout => checkout.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
