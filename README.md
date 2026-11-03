IT Asset Checkout is a custom web application for recording equipment and the people who borrow it. This is a custom IT Asset Checkout scenario, not a course sample. Staff hold the people. A Checkout links one asset to one staff member, and an asset can have only one open checkout (a row whose return date is empty). This sample includes cookie sign-in plus Assets, Staff, and Checkouts.

Local and dev use SQLite so the app starts without a database server. The production and demo target is SQL Server. The provider is switchable: set `DatabaseProvider` to `SqlServer` and point `ConnectionStrings:DefaultConnection` at SQL Server. `appsettings.Development.json` includes a placeholder SQL Server connection string under `ConnectionStrings:SqlServerExample`. That password is not a real secret.

## Run

Install the .NET 8 SDK, then from the repository root:

```bash
dotnet restore
dotnet run --project src/ItAssetCheckout
```

Open http://localhost:5080.

On first run the app creates `src/ItAssetCheckout/it-asset-checkout.db` and, when each table is empty, inserts six assets, six staff members, and six checkouts. One of those checkouts is open. If a database file from the earlier sample has no Staff or Checkouts table, startup recreates that file and seeds it again. Delete the file yourself when you want a clean seed.

## Demo sign-in

Demo environment only. Do not use this account in production.

演示账号见 `.env.example`. Copy that file to `.env` (gitignored), set `DEMO_ADMIN_PASSWORD` locally, then export it before you run:

```bash
set -a
source .env
set +a
dotnet run --project src/ItAssetCheckout
```

The username defaults to `admin` when `DEMO_ADMIN_USER` is empty. The account is created only when `Demo:SeedUser` is `true` and `DEMO_ADMIN_PASSWORD` is set. `SeedUser` is `true` in `appsettings.Development.json` and `false` in `appsettings.json`. The password is not stored in appsettings.

Assets, Staff, and Checkouts require a signed-in user. Home, Login, and the Test page stay public. The header shows the username and Logout after sign-in.

Each list has a search box and a sort:

- Assets: tag number, name, category, or status
- Staff: staff code, name, email, or department
- Checkouts: asset, staff member, note, the word `open`, or a date written `yyyy-MM-dd`. Sort by checkout date, asset, staff, due date, or return date

Create and edit check required fields, email format, duplicate tag numbers and staff codes, a due date on or after the checkout date, and a single open checkout per asset.

## Tests

```bash
dotnet test
```

That command runs the five unit cases. The Selenium cases stay skipped unless the site is already running and `RUN_SELENIUM=1` is set. The commands and the case tables are in `docs/testing/`. The Test page lists UT-01 through UT-05.

## Switch to SQL Server

1. Set `DatabaseProvider` to `SqlServer`.
2. Set `ConnectionStrings:DefaultConnection` to your SQL Server string. Start from the example in `appsettings.Development.json` and replace the placeholder password locally. Do not commit a real password.
3. Run the app against an empty database. The schema is created on startup with `EnsureCreated`. If that database was created before Staff and Checkouts existed, startup drops and recreates it.

## Folder layout

```
src/ItAssetCheckout/
  Controllers/    Home, Account, Assets, Staff, Checkouts, and the Test page
  Data/           EF Core context and first-run seed
  Models/         Asset, Staff, Checkout, list filters, and the login form
  Views/          Razor pages for home, login, and the three registers
  wwwroot/        Primary colour styles and the logo placeholder
  Program.cs      Host, cookie Identity, and the database provider switch
tests/ItAssetCheckout.Tests/       xUnit cases UT-01 through UT-05
tests/ItAssetCheckout.Selenium/    Browser cases, skipped unless RUN_SELENIUM=1
docs/testing/                      IEEE 829 plan and the four test write-ups
docs/security/                     Static analysis notes and the OWASP map
docs/performance/                  JMeter plan and the recorded run
docs/agile/                        Stories, MoSCoW, sprint notes, burndown, Trello checklist
presentation/                      Reserved for a later deck
```

## Still later

The demonstration deck, a live Trello invite, and the upload pack are not in this draft. `docs/agile/trello.md` lists the human steps for the board.
