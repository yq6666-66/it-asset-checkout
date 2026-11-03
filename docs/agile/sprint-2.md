IT Asset Checkout is a custom equipment-loan scenario, not a course sample.

# Sprint 2

## 目的

Record the Sprint 2 window and the retest goal from tip `docs/agile/sprint-2.md` at `f7d5b172`, and fill Review, Retro, and standup only with facts that tip already records. Do not invent meeting dialogue, a product-owner decision, a school name, or a mailbox.

## 方法

Keep the tip window **2026-11-04 through 2026-11-05 morning**. The demonstration freeze sits at the end of the window. The goal points at `docs/testing/ieee-829-test-plan.md` on the frozen build `f7d5b172`. Review and Retro cite the Pass evidence already in that build, and any change request stays for the board owner / PO to backfill after the demonstration（演示后由板主 / PO 回填）. Standup rows for 2026-11-04 and 2026-11-05 restate that window, the retest, and the recorded evidence paths. Demo credentials use `DEMO_ADMIN_PASSWORD` (value `[已遮蔽]`); no plaintext password. Board: `https://trello.com/b/beKd2EVy/it-asset-checkout`.

## Window

2026-11-04 through 2026-11-05 morning. The tip file at `f7d5b172` was a skeleton. The demonstration freeze sits at the end of this window.

## Goal

Repeat the checks in `docs/testing/ieee-829-test-plan.md` on the frozen build and fill any gap the review finds.

## Review

The tip does not contain review minutes. The bullets record the Pass evidence already stored on `f7d5b172` (run date 2026-11-03). A product-owner decision is not on the tip（演示后由板主 / PO 回填）.

- What was demonstrated: The frozen build's recorded pack, not a new live review. Selenium SEL-01 through SEL-04 Pass (`docs/testing/selenium-run.md`: 4 passed, 0 failed, 0 skipped). Screenshots: `docs/testing/selenium/screenshots/sel-01-login.png`, `docs/testing/selenium/screenshots/sel-02-assets.png`, `docs/testing/selenium/screenshots/sel-03-checkouts.png`, `docs/testing/selenium/screenshots/sel-04-staff.png`. Black-box BB-01 through BB-06 Pass (`docs/testing/black-box.md`). White-box WB-01 through WB-10 Pass (`docs/testing/white-box.md`). JMeter on 2026-11-03: 12,183 samples, 0 errors, total p95 239 ms, inside the plan targets of p95 under 1 second and error rate under 1 percent (`docs/performance/results-summary.md`). Evidence files: `docs/performance/jmeter-dashboard.png`, `docs/performance/results.jtl`, `docs/performance/statistics.json`. Sprint 2 repeats `docs/testing/ieee-829-test-plan.md` on this same build.
- What the reviewer asked to change: Nothing is recorded. IEEE 829 section 14 still has Reviewer Open (`docs/testing/ieee-829-test-plan.md`). The checklist in `docs/agile/trello.md` is still unchecked. The board URL is `https://trello.com/b/beKd2EVy/it-asset-checkout`.
- Accepted or sent back: Not recorded. The build stays the draft at `f7d5b172` until the board owner / PO backfills this line after the demonstration.

## Retro

No retrospective minutes are on the tip. The three lines below use only that record.

- What to keep: The window 2026-11-04 through 2026-11-05 morning, one frozen build `f7d5b172`, env-only demo sign-in (`DEMO_ADMIN_PASSWORD`, value `[已遮蔽]`), and the Pass evidence paths listed in the Review.
- What to change next sprint: Only a gap this review records. The tip records none. Slide polish and the upload pack stay in Sprint 3, after the demonstration (`docs/agile/sprint-3.md`).
- One concrete action: Before the 2026-11-05 morning freeze, repeat `docs/testing/ieee-829-test-plan.md` on `f7d5b172` and append any failed id under "What the reviewer asked to change". Do not write a product-owner decision the tip does not have.

## Daily standup

Four short notes for each day, taken from the tip record. No attendance list is stored on `f7d5b172`.

| Date | 参与 | 沟通 | 贡献 | 协作 |
|---|---|---|---|---|
| 2026-11-04 | Sprint 2 window opens. The tip names no attendees. | Retest plan is `docs/testing/ieee-829-test-plan.md` on frozen build `f7d5b172`. | Carry forward the 2026-11-03 Pass pack: Selenium screenshots under `docs/testing/selenium/screenshots/`, and JMeter `docs/performance/results.jtl`. | Board `https://trello.com/b/beKd2EVy/it-asset-checkout`. Demo password stays `DEMO_ADMIN_PASSWORD` (`[已遮蔽]`). |
| 2026-11-05 | Morning demonstration freeze. The tip names no attendees. | Review and Retro stay open for the board owner / PO. IEEE 829 Reviewer is still Open. | Write a gap here only if the retest finds one. None is recorded on the tip. | Same build `f7d5b172`. This note does not add a release tag. |

## 验收要点

Pass: window 2026-11-04～05 morning; Goal cites the IEEE 829 retest and review gaps; Review cites tip SHA `f7d5b172`, the Pass rows, and the demo evidence paths above; Retro has one concrete action; standup 2026-11-04 and 2026-11-05 are filled from those tip facts; password is `[已遮蔽]`; no school name, mailbox, plaintext password, or invented product-owner decision. Fail: a different window or SHA, a plaintext password, a school name, or a review verdict the tip does not record.
