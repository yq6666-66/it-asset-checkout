# Changelog

All notable changes to this project are recorded here.

## Unreleased

### Added

- Windows one-click launchers under `scripts/windows/` (`Start-IT-Asset-Checkout`, `Open-Slide7-Evidence`, `Open-Test-Evidence`, each as `.ps1` and `.bat`). The slide-7 script checks the site, runs Selenium and JMeter, stops hosts that lock build outputs, runs `dotnet clean` / `dotnet build`, then opens the evidence files. Paths resolve from the script folder. The demo password stays in the gitignored `.env`.
- English demonstration PowerPoint at `presentation/IT-Asset-Checkout-demo-deck.pptx` (video-s5), with embedded main live-path and step-8 narration clips on slides 5 and 7.
- `presentation/README.md` describing how to open the deck, the ten-slide talk order, embedded media checksums, and optional local `dotnet run` mirroring.

### Changed

- Root `README.md`: Screenshots section added with the signed-in home, Assets list, Checkouts list, and the recorded JMeter dashboard.
- Root `README.md`: Windows section points at `scripts/windows/`, and the folder layout lists that directory.
- Root `README.md`: Folder layout and demonstration sections now point at the deck in `presentation/`; placeholder “Reserved for a later deck” wording removed. “Still later” no longer claims the deck is missing.
