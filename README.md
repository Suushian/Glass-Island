# GlassIsland

**A lightweight Windows Dynamic Island-style desktop companion**

GlassIsland is a Windows desktop utility with a compact floating surface near the top-center of the display. The MVP target is a minimal, smoothly animated shell with controls for the active supported media session, system output volume, and Windows notifications where documented APIs and permissions allow them.

> **Current status:** Phase 1 and Phase 2 are merged into `main`. Phase 2 provides Windows media-session metadata and transport controls, plus default output volume/mute controls. The latest Windows CI build and self-contained x64 Debug publish passed in [run 40092533594](https://github.com/Suushian/Glass-Island/actions/runs/40092533594), and the user confirmed the Windows build works, including correct Telegram track title and artist display. Phase 3 is underway: the first Windows notification-listener adapter and scrollable notification list are in place, and in-app copy is English-only with a friendly, conversational tone. The target is to show supported notifications from all apps after Windows permission is granted. End-to-end access remains unverified until package identity, capability declaration, and real Windows permission/runtime tests pass. See the [roadmap](docs/ROADMAP.md), [Phase 3 notification POC](docs/PHASE-3-NOTIFICATIONS-POC.md), and [package-identity test runbook](docs/PHASE-3-PACKAGE-IDENTITY-TEST.md).

## Run locally

Prerequisites: Windows 10/11 and the .NET 8 SDK.

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

The app provides the floating shell and tray lifecycle, event-driven Windows media-session controls, and default output volume/mute controls. When a media session or audio endpoint is unavailable, the corresponding controls remain disabled.

## Documentation
- [Product requirements](docs/PRODUCT.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Architecture decisions](docs/ADR.md)
- [UX and visual design](docs/UX-DESIGN.md)
- [Requirements and acceptance criteria](docs/REQUIREMENTS.md)
- [Implementation roadmap](docs/ROADMAP.md)
- [Phase 1 report](docs/PHASE-1-REPORT.md)
- [Testing and quality plan](docs/TESTING.md)
- [Security and privacy](docs/SECURITY-PRIVACY.md)
- [Build and release plan](docs/BUILD-RELEASE.md)
- [Phase 3 notification listener POC](docs/PHASE-3-NOTIFICATIONS-POC.md)
- [Phase 3 package-identity test runbook](docs/PHASE-3-PACKAGE-IDENTITY-TEST.md)
- [Risks and open decisions](docs/RISKS.md)
- [Repository structure and conventions](docs/REPOSITORY-STRUCTURE.md)
- [Glossary](docs/GLOSSARY.md)

## Product principles
1. **Lightweight by design:** prefer event-driven integrations over continuous polling.
2. **Native where practical:** use Windows APIs where supported and justified.
3. **Unobtrusive:** collapsed UI remains small and passive.
4. **Graceful degradation:** one unavailable integration must not break the shell.
5. **Privacy-respecting:** no telemetry by default; notification access must be documented and permission-compatible.
6. **Evidence-based:** performance and compatibility claims require repeatable tests.

## Planned stack

C# / WPF on .NET 8; Windows SDK projections for GSMTC media sessions; NAudio Core Audio endpoint volume and device notifications. Notification implementation remains gated on a technical proof-of-concept.

## Validation status

- **Phase 1 CI:** Release build and self-contained Windows x64 Debug publish succeeded in [run 40017882776](https://github.com/Suushian/Glass-Island/actions/runs/40017882776).
- **Phase 1 manual smoke test:** user confirmed startup, visible shell, expand/collapse, Escape-to-collapse, and tray Exit.
- **Phase 2 CI:** Release build and self-contained Windows x64 Debug publish passed in [run 40092533594](https://github.com/Suushian/Glass-Island/actions/runs/40092533594).
- **Phase 2 manual validation:** user confirmed the Windows build and media/audio behavior work correctly, including Telegram track title and artist metadata.
- **Remaining before a production release:** automated regression tests, mixed-DPI/multi-monitor validation, performance measurements, installer/signing, and notification integration.
