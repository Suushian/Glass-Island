# GlassIsland

**A lightweight Windows Dynamic Island-style desktop companion**

GlassIsland is a proposed Windows desktop utility with a compact floating surface near the top-center of the display. The intended product brings supported media controls, system volume, and carefully scoped notification previews into one unobtrusive UI.

> **Current repository status:** documentation baseline only. The uploaded project package describes a proposed architecture and roadmap; the application source, solution, test projects, native integrations, installer, and measured performance results are not present in this repository yet. Feature descriptions below are plans, not claims of implemented behavior.

## Documentation
- [Product requirements](docs/PRODUCT.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Architecture decisions](docs/ADR.md)
- [UX and visual design](docs/UX-DESIGN.md)
- [Requirements and acceptance criteria](docs/REQUIREMENTS.md)
- [Implementation roadmap](docs/ROADMAP.md)
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

## Proposed stack
C# / WPF, Windows-specific .NET target, GSMTC for supported media sessions, Core Audio for output volume, and opt-in per-user startup. Notification support is explicitly gated on a technical proof-of-concept.

## Build status
Build commands and proposed project paths are documented in [Build and Release](docs/BUILD-RELEASE.md), but cannot run until the solution and source projects are added. No build or test success is claimed for this repository state.
