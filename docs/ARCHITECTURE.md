# Architecture

## Proposed stack
- C# and WPF on a Windows-specific .NET target; .NET 8 is the initial documented baseline and should be reassessed before release.
- MVVM-style separation for presentation and application state.
- Windows Global System Media Transport Controls (GSMTC) for supported media sessions.
- Windows Core Audio APIs for endpoint volume and mute.
- Notification implementation remains undecided until a proof-of-concept validates supported APIs and permissions.
- Per-user, opt-in startup registration.
- Unit tests for platform-neutral logic and Windows integration tests for OS adapters.

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
This is a target layout, not a claim that these projects already exist. The current implementation still keeps the WPF app and its Windows adapters in one project; the media and audio adapters live under `src/GlassIsland.App/Services/`.

## Phase 2 implementation status
- `MediaSessionService` uses the Windows Global System Media Transport Controls API and listens for current-session, session-list, media-property, and playback-state changes.
- `AudioService` uses NAudio Core Audio APIs to read/set the default multimedia render endpoint volume and mute state, listens for volume notifications, and rebinds when the default render endpoint changes.
- Both services publish snapshots and represent unavailable states instead of taking down the shell. UI callbacks marshal state updates onto the WPF Dispatcher.
- The Phase 2 code is awaiting a successful Windows CI build and manual runtime validation; implementation status must not be confused with acceptance.

## Boundaries
- App: WPF startup, windows, view models, resources, settings.
- Core: immutable state models, interfaces, coordination, validation.
- Windows: GSMTC, Core Audio, window placement, startup, and any validated notification adapter.
- Tests: unit tests and Windows-only integration coverage.
- Packaging: installer metadata and release scripts.

## Contracts (illustrative)
```csharp
public interface IMediaService
{
    event EventHandler<MediaSnapshot>? StateChanged;
    Task InitializeAsync(CancellationToken cancellationToken);
    Task PlayPauseAsync(CancellationToken cancellationToken);
    Task NextAsync(CancellationToken cancellationToken);
    Task PreviousAsync(CancellationToken cancellationToken);
}

public interface IAudioService
{
    event EventHandler<AudioSnapshot>? StateChanged;
    Task InitializeAsync(CancellationToken cancellationToken);
    Task SetVolumeAsync(float normalizedVolume, CancellationToken cancellationToken);
    Task SetMutedAsync(bool isMuted, CancellationToken cancellationToken);
}
```
These are design examples, not existing implementation. Define snapshot models and error contracts before coding. Keep COM objects and native handles out of the UI layer.

## State flow
1. Start the shell in a usable collapsed state.
2. Initialize each integration independently.
3. Publish initial snapshots and availability states.
4. Marshal immutable state changes onto the WPF Dispatcher.
5. Update only affected UI elements.
6. Mark failed integrations unavailable without terminating the shell.
7. Unsubscribe events and dispose native resources on shutdown.

## Resource and threading policy
Prefer OS events over polling. Keep callbacks short; marshal state to the UI thread. Avoid blocking UI calls, idle animations, and repeated layout work. Bound album-art memory and debounce event bursts without making controls feel delayed. Pass CancellationToken to asynchronous initialization and clean up deterministically.

## Windowing
Use a borderless topmost window without a normal taskbar button. Position against the selected monitor/work area, account for DPI, and recalculate after display topology changes. Passive events must not steal focus. Define a full-screen policy and ensure resolution changes cannot strand the window off-screen.

## Availability and errors
Represent states such as Available, Unavailable, PermissionRequired, Unsupported, and TemporarilyFailed. Messages should be actionable and non-repetitive.

## Known uncertainties
WPF transparency is not equivalent to native Acrylic/Mica. GSMTC metadata varies by player. Core Audio COM lifetime needs careful handling. Notification access is the highest-risk capability. Deployment mode affects package size and runtime prerequisites.
