# Build, Run, and Release

## Prerequisites

- Windows 10/11 development machine.
- .NET 8 SDK.
- Git and Visual Studio 2022 with the .NET desktop workload, or a compatible editor.

Check the environment:

```powershell
dotnet --info
dotnet --list-sdks
```

## Build and run

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Debug
dotnet build .\GlassIsland.sln -c Release
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

The WPF app targets `net8.0-windows10.0.19041.0`. It uses Windows SDK projections for GSMTC media sessions. The product scope is media-only: no notification listener, package-identity registration, or system audio/volume controls. GitHub Actions builds Release and publishes a self-contained Windows x64 Debug executable. Build success does not establish compatibility with every Windows build or display configuration.

There is not yet a test project, so `dotnet test` does not provide meaningful coverage at this stage.

## Published Debug artifact

The workflow artifact is named `GlassIsland-Debug-win-x64` and is retained by GitHub Actions for 14 days. Download the ZIP from the successful workflow run, extract it into a separate directory, and launch `GlassIsland.exe`. The artifact is self-contained and includes the .NET runtime. It is a CI test build, not a signed installer or production release.

## Publish options

Choose only after runtime and distribution requirements are known:

```powershell
dotnet publish .\src\GlassIsland.App\GlassIsland.App.csproj -c Release -r win-x64 --self-contained false
dotnet publish .\src\GlassIsland.App\GlassIsland.App.csproj -c Release -r win-x64 --self-contained true
```

Self-contained builds include the runtime and usually increase artifact size; they do not guarantee lower memory use or faster startup.

## Release checklist

- [ ] Clean Release build on a clean machine.
- [ ] Supported Windows versions documented.
- [ ] Media-session scenarios tested across the declared support matrix.
- [ ] Startup opt-in and cleanup verified if that feature is implemented.
- [ ] Install, upgrade, repair, uninstall tested.
- [ ] Signing and publisher identity decided.
- [ ] Privacy/security checklist reviewed.
- [ ] Performance report generated using the documented protocol.
- [ ] Known limitations and rollback/update policy documented.

## Versioning

Use Semantic Versioning where practical and preserve the source revision associated with every binary release.
