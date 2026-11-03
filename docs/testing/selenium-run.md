IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Selenium run on 2026-11-03

Command:

```bash
export RUN_SELENIUM=1
export SELENIUM_SHOT_DIR="$PWD/docs/testing/selenium/screenshots"
dotnet test tests/ItAssetCheckout.Selenium
```

The site was already listening on `http://127.0.0.1:5080`.

Result: **4 passed, 0 failed, 0 skipped.**

| ID | Result | Screenshot |
|---|---|---|
| SEL-01 | Pass | `screenshots/sel-01-login.png` |
| SEL-02 | Pass | `screenshots/sel-02-assets.png` |
| SEL-03 | Pass | `screenshots/sel-03-checkouts.png` |
| SEL-04 | Pass | `screenshots/sel-04-staff.png` |

No failure screenshots were kept. A failed case would write `screenshots/failures/<id>.png` and include that path in the test output.

The first attempt failed SEL-02 and SEL-03 because the next navigation started before the sign-in cookie was visible. The login helper now waits until `.username` is on the page. The rerun above is the recorded result.
