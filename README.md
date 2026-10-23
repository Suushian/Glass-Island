# GlassIsland

**A lightweight Windows Dynamic Island-style desktop companion**

GlassIsland is a Windows desktop utility with a compact floating surface near the top-center of the display. The MVP target is a minimal, smoothly animated shell with controls for the active supported media session, system output volume, and Windows notifications where documented APIs and permissions allow them.

> **Current status:** Phase 1 stable-shell implementation is complete and has passed a Windows CI build plus user-reported manual smoke tests on Windows. Startup is wired through `StartupUri="MainWindow.xaml"`. Media controls and system audio remain Phase 2; notification support remains Phase 3. See the [Phase 1 report](docs/PHASE-1-REPORT.md).

## Run locally

Prerequisites: Windows 10/11 and the .NET 8 SDK.

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

The app provides a floating shell, expand/collapse interaction, tray show/hide/exit, and visible placeholders for features planned in later phases.

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

C# / WPF on .NET 8 for the initial shell; GSMTC for supported media sessions; Windows Core Audio for endpoint volume; notification implementation gated on a technical proof-of-concept.

## Validation status

- **Windows CI:** Release build and self-contained Windows x64 Debug publish succeeded in GitHub Actions (run [40016330117](https://github.com/Suushian/Glass-Island/actions/runs/40016330117)).
- **Manual smoke test:** user confirmed the app starts and is visible, expand/collapse works, Escape collapses the surface, and tray Exit closes it.
- **Not yet verified:** mixed-DPI multi-monitor edge cases, performance measurements, full compatibility matrix, automated test coverage, installer/signing, and integrations assigned to later phases.
