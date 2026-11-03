IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Sprint 2

## Window

2026-11-04 through 2026-11-05 morning. The demonstration freeze sits at the end of this window.

Tip commit `f7d5b172`. The file `docs/agile/sprint-2.md` at that commit was a skeleton.

Board: `https://trello.com/b/beKd2EVy/it-asset-checkout`. Demo sign-in uses `DEMO_ADMIN_PASSWORD` (value `[已遮蔽]`).

## Goal

Repeat the checks in `docs/testing/ieee-829-test-plan.md` on the frozen build (tip commit `f7d5b172`) and fill any gap the review finds.

## Review

The runs below are recorded on tip commit `f7d5b172` and dated 2026-11-03. They are not a sprint review decision.

- What was demonstrated: Selenium SEL-01 through SEL-04 Pass (`docs/testing/selenium-run.md`: 4 passed, 0 failed, 0 skipped). Screenshots: `docs/testing/selenium/screenshots/sel-01-login.png`, `docs/testing/selenium/screenshots/sel-02-assets.png`, `docs/testing/selenium/screenshots/sel-03-checkouts.png`, `docs/testing/selenium/screenshots/sel-04-staff.png`. Black-box BB-01 through BB-06 Pass (`docs/testing/black-box.md`). White-box WB-01 through WB-10 Pass (`docs/testing/white-box.md`). JMeter CLI summary: 12,183 samples, 0 errors (`docs/performance/results-summary.md`). For the total row, the report's 95th percentile (`pct2ResTime`) is 239 ms. Evidence files: `docs/performance/jmeter-dashboard.png`, `docs/performance/results.jtl`, `docs/performance/statistics.json`.
- What the reviewer asked to change: Not recorded.
- Accepted or sent back: Not recorded.

Backfill after demonstration:

## Retro

- What to keep: The window 2026-11-04 through 2026-11-05 morning, tip commit `f7d5b172`, and the Pass evidence listed in the Review.
- What to change next sprint: Not recorded. `docs/agile/sprint-3.md` states: "Slide polish and the upload pack are this sprint, not the current draft."
- One concrete action: Before the 2026-11-05 morning freeze, repeat the checks in `docs/testing/ieee-829-test-plan.md` on tip commit `f7d5b172`.

## Daily standup

| Date | 参与 | 沟通 | 贡献 | 协作 |
|---|---|---|---|---|
| 2026-11-04 | 未记录 | Sprint 2 window opens. Frozen build is tip commit `f7d5b172`. | Retest target is `docs/testing/ieee-829-test-plan.md`. The 2026-11-03 Pass pack is on that commit: `docs/testing/selenium-run.md`, `docs/testing/black-box.md`, `docs/performance/results-summary.md`. | 未记录 |
| 2026-11-05 | 未记录 | Demonstration freeze is this morning. Review decision: Not recorded. | Repeat `docs/testing/ieee-829-test-plan.md` on tip commit `f7d5b172` before the freeze. | 未记录 |
