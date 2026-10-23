# Phase 1 Report — Stable Shell

## Status

**Implementation added; Windows runtime validation is still required.** The repository now contains a first WPF application shell. This report deliberately separates code committed from behavior not yet verified on a Windows machine.

## Delivered

- .NET 8 Windows WPF application project and solution.
- Borderless, topmost floating surface with a compact collapsed state.
- Expanded state with short eased transitions; no idle animation loop.
- Escape-to-collapse interaction.
- System tray menu for show, hide, and explicit exit.
- Alt+F4 hides to the tray; explicit tray Exit disposes the icon and closes the app.
- Primary-display top-center placement, DPI conversion for the initial placement, and repositioning after Windows display-settings changes.
- Per-monitor DPI awareness declaration in the application manifest.
- Placeholder feature states clearly label media/audio as Phase 2 and notifications as Phase 3; no unsupported functionality is simulated.
- Windows GitHub Actions build workflow for Release configuration.

## Files

- `GlassIsland.sln`
- `src/GlassIsland.App/GlassIsland.App.csproj`
- `src/GlassIsland.App/App.xaml`
- `src/GlassIsland.App/App.xaml.cs`
- `src/GlassIsland.App/MainWindow.xaml`
- `src/GlassIsland.App/MainWindow.xaml.cs`
- `src/GlassIsland.App/app.manifest`
- `.github/workflows/windows-build.yml`
- `.gitignore`

## Acceptance review

| Check | Current result |
|---|---|
| WPF source and solution added | Done |
| Collapsed/expanded interaction implemented | Done in source; runtime test pending |
| Eased expand/collapse transitions | Done in source; visual review pending |
| Tray show/hide/exit lifecycle | Done in source; runtime test pending |
| Explicit unavailable states for unimplemented features | Done in UI source |
| Display-change repositioning | Implemented for primary display; Windows/multi-monitor verification pending |
| DPI behavior across mixed-scale monitors | Declared/partially handled; requires Windows validation |
| Release build | Not run in this environment (no .NET SDK/Windows runtime available) |
| Visual inspection on Windows | Not run |
| CPU/memory/startup measurements | Not run |

## Known limitations

1. This phase is a shell prototype, not the complete MVP.
2. The tray uses the standard Windows application icon until product branding is added.
3. Placement currently targets the primary display; a user-selectable monitor setting is not implemented.
4. Mixed-DPI multi-monitor behavior and taskbar/work-area edge cases must be verified on Windows.
5. Media, audio, and notifications are not connected yet and are intentionally shown as not implemented.
6. The notification API decision remains gated on the Phase 3 proof of concept.

## Validation commands (Windows)

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release --no-restore
```

Run the app from Visual Studio or:

```powershell
dotnet run --project .\src\GlassIsland.App\GlassIsland.App.csproj
```

## Next step

Run the Windows CI build and fix any XAML/compiler issues it finds. Then verify the shell manually on Windows 10/11, including hide/show/exit, keyboard interaction, display changes, and DPI scaling. After the shell is stable, proceed to Phase 2 (GSMTC media sessions and Core Audio volume control).
