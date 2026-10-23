# Repository Structure and Engineering Conventions

## Target layout
```text
GlassIsland/
├── GlassIsland.sln
├── README.md
├── docs/
├── src/
│   ├── GlassIsland.App/
│   ├── GlassIsland.Core/
│   └── GlassIsland.Windows/
├── tests/
│   ├── GlassIsland.Core.Tests/
│   └── GlassIsland.Windows.Tests/
└── packaging/
```
This is a target layout. The current repository baseline contains documentation only; do not assume these projects exist until committed.

## Responsibilities
- App: WPF startup, windows, view models, resources, settings UI.
- Core: platform-neutral models, interfaces, feature state, validation.
- Windows: GSMTC, Core Audio, window placement, startup, and validated notification adapter.
- Tests: unit and Windows-only integration tests.
- Packaging: installer metadata, assets, release scripts.

## Conventions
- Enable nullable reference types.
- Prefer async APIs for operations that can block; pass CancellationToken.
- Publish immutable snapshots from adapters.
- Avoid async void except UI event handlers.
- Dispose event subscriptions and native resources deterministically.
- Keep COM/native types out of view models.
- Localize user-facing strings.
- Redact personal data in logs.

## Git hygiene
Commit source, docs, tests, and reproducible build configuration. Do not commit bin/obj, user-specific IDE files, logs, generated installers unless explicitly required, secrets, tokens, or personal notification/media data. Record architectural decisions in ADRs.
