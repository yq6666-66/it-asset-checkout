# Presentation deck

This folder holds the English demonstration PowerPoint for the IT Asset Checkout delivery. The deck is the talk track for a live walkthrough. Written Agile, testing, security, and performance evidence stays under `docs/`; open those folders when a slide points at a plan, write-up, or board note.

The file to open is `IT-Asset-Checkout-demo-deck.pptx` (SHA-256 `0150847b331d576e1cd4e3ada675c58dacfc4404a802ef8ba4e5d129342125b2`). Prefer Microsoft PowerPoint desktop for the embedded MP4 clips on slides 5 and 7; other viewers may open the slides but skip video playback. Slides 5 and 7 contain embedded MP4 clips; play them from the slides. Do not speak or paste a plaintext demo password. `DEMO_ADMIN_PASSWORD` is only used to create the demo account on startup; sign in on the Login page with that password (see the repository root README and `.env.example`).

## How this deck was assembled

Before any slide was Englishized or given an embedded clip, the delivery locked the business scenario as custom IT Asset Checkout (ASP.NET Core 8, cookie Identity, SQLite locally, SQL Server for demo or production), kept Agile / testing / security / performance write-ups under `docs/`, and kept demo credentials out of the repository by configuring `DEMO_ADMIN_PASSWORD` locally from `.env.example`. The deck outline was shaped as background and goals, live demo, testing and security evidence, Agile process, then FAQ (about ten slides). Supporting evidence and FAQ notes were frozen before the embedded-video line started.

The embedded-video and English body gates, in order:

1. **ppt-video-s1:** embed the main live-path English-narrated clip on slide 5.
2. **ppt-video-s2:** embed the step-8 / Selenium path English-narrated clip on slide 7.
3. **ppt-video-s3:** Chinese body polish on other slides. That pass is **superseded** and is not the demonstration deck after the requirement that slide bodies stay English.
4. **ppt-video-s4:** Englishize slides 1–2 (sample pass).
5. **ppt-video-s5:** Englishize slides 3, 4, 6, 8, 9, and 10. Slides 5 and 7 (including embedded media) stay hard-kept from the prior embedded state. **s5 is the current demonstration deck** copied into this folder as `IT-Asset-Checkout-demo-deck.pptx`.

Narration must match what is on screen, stay on the marking guide and learning outcomes, and avoid filler. Only the English-dubbed clips are embedded.

## Slide map (current deck)

| Slide | Content | Language |
| --- | --- | --- |
| 1–2 | Cover; background and goals | English (from s4) |
| 3–4 | Architecture and features; demo path notes | English (from s5) |
| 5 | Main live-path demo video (embedded) | Slide shell hard-kept; clip is English narration |
| 6 | Testing evidence overview | English (from s5) |
| 7 | Step-8 / Selenium path video (embedded) | Slide shell hard-kept; clip is English narration |
| 8 | Security and static analysis | English (from s5) |
| 9 | Agile / Trello | English body (from s5); the Trello screenshot is image media and may still show Chinese card titles — accepted exception; slide body text is English and the board image is not rewritten in this gate |
| 10 | FAQ | English (from s5) |

## Embedded clips

| Slide | Media inside the pptx | SHA-256 |
| --- | --- | --- |
| 5 | Main live-path English dub (`livepath-en-r2`) | `5fc4c19b51033685f29f153e427dcae04c8155430141949a7b7573e553be1a61` |
| 7 | Step-8 path English dub (`step8-en-r2`) | `7c9769c959567ae37f9aa9203268118a423af3ef723c0d953b798336eead1313` |

## Files in this folder

| File | Role | SHA-256 |
| --- | --- | --- |
| `IT-Asset-Checkout-demo-deck.pptx` | Current English demonstration deck (video-s5) | `0150847b331d576e1cd4e3ada675c58dacfc4404a802ef8ba4e5d129342125b2` |
| `README.md` | This process note | — |

## Local run (optional mirror)

From the repository root, after the .NET 8 SDK is installed:

```bash
dotnet restore
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/ItAssetCheckout
```

Open http://localhost:5080. Setting `ASPNETCORE_ENVIRONMENT=Development` keeps `Demo:SeedUser` true so the demo account can be created. Set `DEMO_ADMIN_PASSWORD` (and optionally `DEMO_ADMIN_USER`) via `.env` as described in the root README. Never put a plaintext demo password in this folder or in the deck.

## Suggested talk order

1. Open `IT-Asset-Checkout-demo-deck.pptx` and walk slides 1–4 for context.
2. On slide 5, play the embedded main-path clip; optionally mirror the same path with the Local run steps above.
3. Use slides 6 and 8 with `docs/testing/` and `docs/security/` when you need the written evidence.
4. On slide 7, play the embedded step-8 clip.
5. Close with Agile / Trello (slide 9, see also `docs/agile/`) and FAQ (slide 10).
