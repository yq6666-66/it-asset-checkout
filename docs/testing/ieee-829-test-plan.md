IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# IEEE 829 test plan

Identifier: ITAC-TP-2026-11-03

## 1. Introduction

This plan covers the demo build of IT Asset Checkout: cookie sign-in, and the Assets, Staff, and Checkouts registers. 演示账号见 `.env.example`. Demo environment only.

## 2. Test items

- Web host `src/ItAssetCheckout` on `http://127.0.0.1:5080`
- SQLite file created on first run, with six seeded rows in each register
- xUnit project `tests/ItAssetCheckout.Tests`
- Selenium project `tests/ItAssetCheckout.Selenium`
- JMeter plan `docs/performance/it-asset-checkout.jmx`

## 3. Features to be tested

- Sign-in and sign-out, and the username in the header
- List, search, and sort for Assets, Staff, and Checkouts
- Create, edit, and delete, including the block on deleting a row that still has checkouts
- Validation: required fields, email format, duplicate codes, date boundaries, one open checkout per asset
- The Test page lists UT-01 through UT-05

## 4. Features not tested here

- SQL Server as a live server (the provider switch is documented; this plan runs on SQLite)
- A presentation deck
- A live Trello board

## 5. Approach

- Unit tests call the validators and `CheckoutRules` directly (`docs/testing/unit-tests.md`).
- White-box posts hit the create actions (`docs/testing/white-box.md`).
- Black-box reads the seeded lists (`docs/testing/black-box.md`).
- Selenium drives Chrome for login and one list on each register (`docs/testing/selenium.md`).
- JMeter sends 20 threads for 60 seconds through login and the Assets list (`docs/performance/`).
- Static analysis is the Security Code Scan analyzer (`docs/security/`).

## 6. Item pass or fail criteria

- A unit case passes when its xUnit assertions hold.
- A black-box or white-box case passes when the page contains the text in its case row.
- A Selenium case passes when its assertions hold and a pass screenshot is saved.
- The JMeter run is recorded with its measured p95 and error rate. The target is p95 under 1 second and error rate under 1 percent.
- A feature fails if a seeded relation is missing or a rejected input is saved.

## 7. Suspension and resumption

Stop the browser and load runs if the site does not answer on port 5080. Resume after `dotnet run --project src/ItAssetCheckout` is listening again. Unit tests do not need the site.

## 8. Test deliverables

- This plan
- The four write-ups in `docs/testing/`
- `dotnet test` output
- Selenium screenshots
- JMeter plan, raw log, and summary
- Static-analysis and OWASP notes

## 9. Testing tasks

1. `dotnet test tests/ItAssetCheckout.Tests`
2. Start the site, then run the white-box and black-box requests
3. `RUN_SELENIUM=1 dotnet test tests/ItAssetCheckout.Selenium`
4. Run the JMeter plan for 60 seconds
5. Build with the Security Code Scan analyzer and record the warnings

## 10. Environment

- .NET 8 SDK
- Local SQLite
- Chrome for Selenium
- Apache JMeter 5.6.3 on Java 21
- Demo environment only for the known password

## 11. Responsibilities

The person running the demo executes the commands in this plan and replaces the Trello placeholder with a real board screenshot. This document does not assign a named reviewer.

## 12. Schedule

The cases in this plan were executed on 2026-11-03 against the draft build. A later pass can repeat them before the demonstration.

## 13. Risks

- The known demo password must not be reused outside a demo environment.
- JMeter numbers are for one local process, not a deployed server.
- An old SQLite file from before Staff and Checkouts existed is recreated on startup.

## 14. Approvals

| Role | Name | Date | Note |
|---|---|---|---|
| Author of this draft |  | 2026-11-03 | Draft for the demonstration pack |
| Reviewer |  |  | Open |
