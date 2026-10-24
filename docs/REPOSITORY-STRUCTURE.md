# Repository Structure and Engineering Conventions

## Current layout

```text
GlassIsland/
├── GlassIsland.sln
├── README.md
├── docs/
│   ├── PRODUCT.md
│   ├── ARCHITECTURE.md
│   ├── ROADMAP.md
│   └── ...
├── src/
│   └── GlassIsland.App/
│       ├── App.xaml
│       ├── App.xaml.cs
│       ├── MainWindow.xaml
│       ├── MainWindow.xaml.cs
│       ├── Services/
│       │   └── MediaSessionService.cs
│       ├── app.manifest
│       └── GlassIsland.App.csproj
└── .github/workflows/windows-build.yml
```

The application currently consists of one WPF project. Its only OS integration is the Windows GSMTC media-session adapter. There is no notification service, audio service, test project, or packaging project in the current scope.

## Planned responsibilities

- App: WPF startup, windows, resources, shell/tray lifecycle.
- Media adapter: GSMTC session discovery, metadata/playback snapshots, supported transport commands.
- Tests: unit tests and Windows-only integration tests.
- Packaging: installer metadata and release scripts, when release requirements are decided.

## Conventions

- Enable nullable reference types.
- Prefer async APIs for operations that can block; pass CancellationToken where applicable.
- Publish immutable snapshots from adapters.
- Avoid async void except UI event handlers.
- Dispose event subscriptions and native resources deterministically.
- Keep COM/native types out of presentation code where practical.
- Keep user-facing strings in English and concise.
- Redact personal data in logs.

## Git hygiene

Commit source, docs, tests, and reproducible build configuration. Do not commit bin/obj, user-specific IDE files, logs, generated installers unless explicitly required, secrets, tokens, or personal media metadata. Record architectural decisions in ADRs.
