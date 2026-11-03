IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# OWASP Top 10 (2021)

演示账号见 `.env.example`. Demo environment only. The account is created only when `Demo:SeedUser` is true and `DEMO_ADMIN_PASSWORD` is set in the environment. That flag is true in `appsettings.Development.json` and false in `appsettings.json`. The password is not written in appsettings.

The static-analysis column points at `security-code-scan.md`. Where the scanner was silent, this table says so instead of inventing a finding.

| OWASP | How this app behaves | Scanner cross-reference |
|---|---|---|
| A01 Broken Access Control | Assets, Staff, and Checkouts require a signed-in cookie. Home, Login, and Test stay public. | No SCS finding. |
| A02 Cryptographic Failures | Identity stores a password hash. The development site uses HTTP on localhost. Production configuration is expected to use HTTPS. The demo password comes from the environment, not from appsettings. | No SCS finding. |
| A03 Injection | List search uses parameterized EF Core queries. | No SCS finding. |
| A04 Insecure Design | One asset can have only one open checkout. That rule is checked in code and by a filtered unique index. | No SCS finding. |
| A05 Security Misconfiguration | The known account is not seeded when `Demo:SeedUser` is false. The SQL Server example password in development settings is a placeholder. | No SCS finding. |
| A06 Vulnerable and Outdated Components | Packages are pinned in the project files for .NET 8. Re-run `dotnet list package --vulnerable` before a deployment. | No SCS finding in this build. |
| A07 Identification and Authentication Failures | Failed sign-ins use one message. Lockout starts after five failures. The cookie is HttpOnly. | No SCS finding. |
| A08 Software and Data Integrity Failures | POST actions use the antiforgery token. | No SCS finding. |
| A09 Security Logging and Monitoring Failures | The host writes standard ASP.NET logs. There is no separate audit trail of who changed a row. | No SCS finding. A later pass could log create, update, and delete with the signed-in name. |
| A10 Server-Side Request Forgery | The app does not fetch a URL supplied by the user. | No SCS finding. |

## SCS0018

The only scanner hit is SCS0018 on `DemoSeeder.DeleteSqliteFile`, described in `security-code-scan.md`. It is a local file delete during startup, not a request to another server, so it is not an A10 issue. The guard that limits deletion to `it-asset-checkout.db` is the hardening for that hit.
