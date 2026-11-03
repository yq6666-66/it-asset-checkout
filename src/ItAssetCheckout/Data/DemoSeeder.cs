using System.Data;
using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ItAssetCheckout.Data;

// Inserts starter assets, staff, and checkouts, plus the demo account when Demo:SeedUser is true.
public static class DemoSeeder
{
    // Creates the database if needed, seeds empty tables, and optionally seeds the demo user.
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await EnsureSchemaAsync(db);

        if (configuration.GetValue<bool>("Demo:SeedUser"))
        {
            await SeedDemoUserAsync(services);
        }

        await SeedAssetsAsync(db);
        await SeedStaffAsync(db);
        await SeedCheckoutsAsync(db);
    }

    // Creates the schema. A sample 1 file has no Staff or Checkouts table, so that file is recreated.
    private static async Task EnsureSchemaAsync(ApplicationDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await TableExistsAsync(db, "Staff") && await TableExistsAsync(db, "Checkouts"))
        {
            return;
        }

        await db.Database.CloseConnectionAsync();
        if (db.Database.IsSqlite())
        {
            DeleteSqliteFile(db);
        }
        else
        {
            await db.Database.EnsureDeletedAsync();
        }

        await db.Database.EnsureCreatedAsync();
    }

    // Closes pooled handles, then removes the SQLite file and its WAL sidecars.
    private static void DeleteSqliteFile(ApplicationDbContext db)
    {
        var connectionString = db.Database.GetConnectionString()
            ?? throw new InvalidOperationException("SQLite connection string is missing.");
        var dataSource = Path.GetFullPath(new SqliteConnectionStringBuilder(connectionString).DataSource);
        if (!string.Equals(Path.GetFileName(dataSource), "it-asset-checkout.db", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Refusing to delete a SQLite file that is not the local demo database.");
        }

        SqliteConnection.ClearAllPools();
        foreach (var path in new[] { dataSource, dataSource + "-wal", dataSource + "-shm" })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    // Looks for a table by name. SQLite and SQL Server use different catalogs.
    private static async Task<bool> TableExistsAsync(ApplicationDbContext db, string tableName)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@name";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);
        command.CommandText = db.Database.IsSqlite()
            ? "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = @name"
            : "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @name";

        var result = await command.ExecuteScalarAsync();
        return result is not null and not DBNull;
    }

    // Creates the demo account only when DEMO_ADMIN_PASSWORD is set and that username is missing.
    private static async Task SeedDemoUserAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DemoSeeder");
        var password = Environment.GetEnvironmentVariable("DEMO_ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("DEMO_ADMIN_PASSWORD is not set. The demo user was not created.");
            return;
        }

        var userName = Environment.GetEnvironmentVariable("DEMO_ADMIN_USER");
        if (string.IsNullOrWhiteSpace(userName))
        {
            userName = "admin";
        }

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var existing = await userManager.FindByNameAsync(userName);
        if (existing is not null)
        {
            return;
        }

        var user = new IdentityUser
        {
            UserName = userName,
            Email = "admin@localhost",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not create the demo user: {errors}");
        }
    }

    // Loads the starter catalogue once. Later runs leave existing rows alone.
    private static async Task SeedAssetsAsync(ApplicationDbContext db)
    {
        if (await db.Assets.AnyAsync())
        {
            return;
        }

        db.Assets.AddRange(
            new Asset
            {
                TagNo = "IT-10482",
                Name = "Dell Latitude 5440",
                Category = "Laptop",
                Status = AssetStatus.Available,
                PurchaseDate = new DateTime(2024, 3, 12),
                Value = 1280.00m
            },
            new Asset
            {
                TagNo = "IT-10490",
                Name = "LG 27UP850",
                Category = "Monitor",
                Status = AssetStatus.InUse,
                PurchaseDate = new DateTime(2023, 11, 2),
                Value = 640.50m
            },
            new Asset
            {
                TagNo = "IT-11002",
                Name = "Dell WD19S Dock",
                Category = "Peripheral",
                Status = AssetStatus.Available,
                PurchaseDate = new DateTime(2024, 6, 18),
                Value = 210.00m
            },
            new Asset
            {
                TagNo = "IT-12019",
                Name = "iPhone 15",
                Category = "Mobile",
                Status = AssetStatus.Maintenance,
                PurchaseDate = new DateTime(2024, 1, 9),
                Value = 899.00m
            },
            new Asset
            {
                TagNo = "IT-13044",
                Name = "Jabra Speak 750",
                Category = "Audio",
                Status = AssetStatus.Available,
                PurchaseDate = new DateTime(2022, 8, 30),
                Value = 275.00m
            },
            new Asset
            {
                TagNo = "IT-14007",
                Name = "OptiPlex Small Form",
                Category = "Desktop",
                Status = AssetStatus.Retired,
                PurchaseDate = new DateTime(2019, 5, 21),
                Value = 740.00m
            });

        await db.SaveChangesAsync();
    }

    // Loads the starter staff list once.
    private static async Task SeedStaffAsync(ApplicationDbContext db)
    {
        if (await db.Staff.AnyAsync())
        {
            return;
        }

        db.Staff.AddRange(
            new Staff
            {
                StaffCode = "S-1001",
                FullName = "Ada Okonkwo",
                Email = "ada.okonkwo@example.com",
                Department = "Finance"
            },
            new Staff
            {
                StaffCode = "S-1002",
                FullName = "Ben Cho",
                Email = "ben.cho@example.com",
                Department = "IT"
            },
            new Staff
            {
                StaffCode = "S-1003",
                FullName = "Carla Mendes",
                Email = "carla.mendes@example.com",
                Department = "Facilities"
            },
            new Staff
            {
                StaffCode = "S-1004",
                FullName = "Dev Patel",
                Email = "dev.patel@example.com",
                Department = "Design"
            },
            new Staff
            {
                StaffCode = "S-1005",
                FullName = "Elena Rossi",
                Email = "elena.rossi@example.com",
                Department = "Operations"
            },
            new Staff
            {
                StaffCode = "S-1006",
                FullName = "Farid Hassan",
                Email = "farid.hassan@example.com",
                Department = "Support"
            });

        await db.SaveChangesAsync();
    }

    // Loads starter loans once. One asset is left with a single open checkout.
    private static async Task SeedCheckoutsAsync(ApplicationDbContext db)
    {
        if (await db.Checkouts.AnyAsync())
        {
            return;
        }

        var assets = await db.Assets.ToDictionaryAsync(asset => asset.TagNo);
        var staff = await db.Staff.ToDictionaryAsync(person => person.StaffCode);
        string[] requiredAssets = ["IT-10482", "IT-10490", "IT-11002", "IT-12019", "IT-13044", "IT-14007"];
        string[] requiredStaff = ["S-1001", "S-1002", "S-1003", "S-1004", "S-1005", "S-1006"];
        if (requiredAssets.Any(tag => !assets.ContainsKey(tag)) || requiredStaff.Any(code => !staff.ContainsKey(code)))
        {
            return;
        }

        db.Checkouts.AddRange(
            new Checkout
            {
                AssetId = assets["IT-10490"].Id,
                StaffId = staff["S-1005"].Id,
                CheckoutAt = new DateTime(2026, 1, 8),
                DueAt = new DateTime(2026, 1, 22),
                ReturnAt = new DateTime(2026, 1, 20),
                Note = "Returned after the planning week."
            },
            new Checkout
            {
                AssetId = assets["IT-10490"].Id,
                StaffId = staff["S-1002"].Id,
                CheckoutAt = new DateTime(2026, 10, 20),
                DueAt = new DateTime(2026, 11, 20),
                ReturnAt = null,
                Note = "On the IT bench."
            },
            new Checkout
            {
                AssetId = assets["IT-10482"].Id,
                StaffId = staff["S-1001"].Id,
                CheckoutAt = new DateTime(2026, 2, 3),
                DueAt = new DateTime(2026, 2, 17),
                ReturnAt = new DateTime(2026, 2, 16),
                Note = "Finance month-end."
            },
            new Checkout
            {
                AssetId = assets["IT-11002"].Id,
                StaffId = staff["S-1003"].Id,
                CheckoutAt = new DateTime(2025, 11, 4),
                DueAt = new DateTime(2025, 11, 18),
                ReturnAt = new DateTime(2025, 11, 18),
                Note = "Dock for the site visit."
            },
            new Checkout
            {
                AssetId = assets["IT-13044"].Id,
                StaffId = staff["S-1004"].Id,
                CheckoutAt = new DateTime(2026, 4, 9),
                DueAt = new DateTime(2026, 4, 23),
                ReturnAt = new DateTime(2026, 4, 22),
                Note = "Design review room."
            },
            new Checkout
            {
                AssetId = assets["IT-14007"].Id,
                StaffId = staff["S-1006"].Id,
                CheckoutAt = new DateTime(2021, 6, 1),
                DueAt = new DateTime(2021, 6, 15),
                ReturnAt = new DateTime(2021, 6, 14),
                Note = "Returned before the desktop was retired."
            });

        await db.SaveChangesAsync();
    }
}
