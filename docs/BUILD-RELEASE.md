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

The WPF app targets `net8.0-windows`. A Windows GitHub Actions workflow builds the solution in Release configuration. The local build and runtime still need confirmation on Windows; a CI workflow definition alone is not evidence of a successful build.

There is not yet a test project, so `dotnet test` does not provide meaningful coverage at this stage.

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
- [ ] Media/audio/notification scenarios tested.
- [ ] Startup opt-in and cleanup verified.
- [ ] Install, upgrade, repair, uninstall tested.
- [ ] Signing and publisher identity decided.
- [ ] Privacy/security checklist reviewed.
- [ ] Performance report generated using the documented protocol.
- [ ] Known limitations and rollback/update policy documented.

## Versioning

Use Semantic Versioning where practical and preserve the source revision associated with every binary release.
