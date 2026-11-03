using ItAssetCheckout.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Sqlite is the local/dev default. Set DatabaseProvider to SqlServer for the production/demo target.
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    else if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(ResolveSqliteConnectionString(connectionString, builder.Environment.ContentRootPath));
    }
    else
    {
        throw new InvalidOperationException(
            $"Unsupported DatabaseProvider '{databaseProvider}'. Use Sqlite or SqlServer.");
    }
});

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await DemoSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Keeps a relative SQLite file next to the project content root, independent of the process working directory.
static string ResolveSqliteConnectionString(string connectionString, string contentRoot)
{
    var sqlite = new SqliteConnectionStringBuilder(connectionString);
    if (!string.Equals(sqlite.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
        && !Path.IsPathRooted(sqlite.DataSource))
    {
        sqlite.DataSource = Path.Combine(contentRoot, sqlite.DataSource);
    }

    return sqlite.ToString();
}
