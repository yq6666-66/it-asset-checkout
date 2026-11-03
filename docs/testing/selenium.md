IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Selenium

## 目的

Drive the real login and the three list pages in Chrome, and keep a screenshot of each passing case. A failure saves a second screenshot under `failures/`.

## 方法

1. Start the site: `dotnet run --project src/ItAssetCheckout` so it listens on `http://127.0.0.1:5080`.
2. From the repository root:

```bash
export RUN_SELENIUM=1
export SELENIUM_SHOT_DIR="$PWD/docs/testing/selenium/screenshots"
dotnet test tests/ItAssetCheckout.Selenium
```

`RUN_SELENIUM` is required. A normal `dotnet test` skips these four cases so the unit suite can run without Chrome.

The project is `tests/ItAssetCheckout.Selenium`. It uses Selenium WebDriver against the local Chrome binary. Each case signs in with `DEMO_ADMIN_USER` (default `admin`) and `DEMO_ADMIN_PASSWORD`. 演示账号见 `.env.example`. Demo environment only. If `RUN_SELENIUM=1` and the password variable is empty, the suite fails with that requirement instead of signing in.

On failure the thrown message includes `Screenshot:` and the path under `docs/testing/selenium/screenshots/failures/`.

## 用例表

| ID | Page | Assertion text | Pass screenshot |
|---|---|---|---|
| SEL-01 | After login | `Assert.Contains("admin", account)` and `Assert.Contains("Logout", account)` on `.account` | `screenshots/sel-01-login.png` |
| SEL-02 | `/Assets` | `Assert.Contains("IT-10482", body)` and `Assert.Contains("Dell Latitude 5440", body)` | `screenshots/sel-02-assets.png` |
| SEL-03 | `/Checkouts` | `Assert.Contains("IT-10490", body)`, `Assert.Contains("Open", body)`, and `Assert.Contains("Ben Cho", body)` | `screenshots/sel-03-checkouts.png` |
| SEL-04 | `/Staff` | `Assert.Contains("S-1001", body)` and `Assert.Contains("Ada Okonkwo", body)` | `screenshots/sel-04-staff.png` |

## 判断

On 2026-11-03, with the site listening on port 5080, the command above reported 4 passed, 0 failed, and 0 skipped. The four pass screenshots are in `screenshots/`. Details are in `selenium-run.md`. Fail means a later run reports a failed method; the message includes `Screenshot:` and a file under `screenshots/failures/`.
