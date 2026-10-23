# GlassIsland

**A lightweight Windows Dynamic Island-style desktop companion**

GlassIsland is a Windows desktop utility with a compact floating surface near the top-center of the display. The MVP target is a minimal, smoothly animated shell with controls for the active supported media session, system output volume, and Windows notifications where documented APIs and permissions allow them.

> **Current status:** Phase 1 shell source has been added. It has not yet been built or visually verified on Windows in this environment. Media controls, system audio integration, and notification support are not implemented yet; see the [Phase 1 report](docs/PHASE-1-REPORT.md).

## Run locally

Prerequisites: Windows 10/11 and the .NET 8 SDK.

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

The app currently provides a floating shell, expand/collapse interaction, tray show/hide/exit, and visible placeholders for features planned in later phases.

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

## Validation
A Windows GitHub Actions workflow builds the solution in Release configuration. Local build success, runtime behavior, multi-monitor/DPI behavior, and performance remain unverified until the workflow and Windows manual checks complete.
