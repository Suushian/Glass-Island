# Repository Structure and Engineering Conventions

## Current layout

```text
GlassIsland/
├── GlassIsland.sln
├── README.md
├── docs/
│   ├── PHASE-1-REPORT.md
│   └── ...
├── src/
│   └── GlassIsland.App/
│       ├── App.xaml
│       ├── App.xaml.cs
│       ├── MainWindow.xaml
│       ├── MainWindow.xaml.cs
│       ├── app.manifest
│       └── GlassIsland.App.csproj
└── .github/workflows/windows-build.yml
```

This is the current first-stage implementation. Media/audio adapters, notification proof-of-concept, automated tests, and packaging projects are not present yet.

## Planned responsibilities

- App: WPF startup, windows, view models, resources, settings UI.
- Core: platform-neutral models, interfaces, feature state, validation.
- Windows: GSMTC, Core Audio, window placement, startup, and any validated notification adapter.
- Tests: unit and Windows-only integration tests.
- Packaging: installer metadata and release scripts.

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
