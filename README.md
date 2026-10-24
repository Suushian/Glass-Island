# GlassIsland

**A lightweight Windows Dynamic Island-style media controller**

GlassIsland is a compact floating Windows desktop companion for the active supported media session. It displays track metadata and playback state and provides previous, play/pause, and next controls when the media player supports them.

## Current scope

GlassIsland is intentionally media-only:
- Floating shell with collapsed and expanded states, tray show/hide/exit, and display-position recovery.
- Active-session metadata and playback state through Windows Global System Media Transport Controls (GSMTC).
- Previous, play/pause, and next commands when exposed by the active player.

System volume controls and notification previews/listening are not part of the product. The app does not request notification access, declare the `userNotificationListener` capability, or require a package-identity test workflow.

Phase 1 shell and Phase 2 media-session behavior were previously manually validated by the user on Windows, including Telegram track title and artist display. The media-only cleanup is in progress on a separate branch and must pass Windows CI before it is considered validated.

## Run locally

Prerequisites: Windows 10/11 and the .NET 8 SDK.

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

The application requires no notification permission and does not control system output volume. If no supported media session is active, media controls remain disabled and a clear fallback state is shown.

## Documentation

- [Product scope](docs/PRODUCT.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Architecture decisions](docs/ADR.md)
- [UX and visual design](docs/UX-DESIGN.md)
- [Requirements and acceptance criteria](docs/REQUIREMENTS.md)
- [Implementation roadmap](docs/ROADMAP.md)
- [Phase 1 report](docs/PHASE-1-REPORT.md)
- [Testing and quality plan](docs/TESTING.md)
- [Security and privacy](docs/SECURITY-PRIVACY.md)
- [Build and release plan](docs/BUILD-RELEASE.md)
- [Risks and open decisions](docs/RISKS.md)
- [Repository structure and conventions](docs/REPOSITORY-STRUCTURE.md)
- [Glossary](docs/GLOSSARY.md)

## Product principles

1. Keep the overlay lightweight and unobtrusive.
2. Prefer event-driven media updates over continuous polling.
3. Degrade gracefully when a player or transport command is unavailable.
4. Keep media metadata local and out of logs.
5. Make no performance or compatibility claims without repeatable evidence.

## Stack

C# / WPF on .NET 8, Windows GSMTC projections, and Windows Forms for the system-tray icon. No third-party audio-control dependency is required.

## Validation status

- Phase 1 shell and Phase 2 media-session behavior were manually accepted on the user's Windows installation.
- The media-only cleanup requires a fresh Windows Release build and self-contained x64 publish in CI.
- Remaining release work includes automated regression coverage, mixed-DPI/multi-monitor validation, performance measurements, installer/signing, and release packaging.
