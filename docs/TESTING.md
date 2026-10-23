# Testing and Quality Plan

## Test layers
- **Unit:** state coordination, availability transitions, volume validation/clamping, settings defaults/migrations, preview redaction/expiry.
- **Adapter:** media session lifecycle and missing metadata; audio endpoint changes/removal; unsupported APIs/errors; event cleanup.
- **UI:** placement, expand/collapse, accessible names, disabled states, focus behavior, multi-monitor/DPI recovery.
- **Manual integration:** run against every declared Windows build and record OS build, display scale, endpoint, media player, and result.

## Scenario matrix
| Scenario | Expected behavior |
|---|---|
| No media session | Shell works; controls neutral/disabled |
| Session starts or changes | State updates from events |
| Missing metadata | Safe fallback label |
| Player closes | State clears without crash |
| Audio device changes/disappears | Rebind or show unavailable state |
| Notification access unavailable | Explicit state; no repeated modal errors |
| App starts offline | Local core remains usable |
| Monitor disconnected / DPI changes | Window remains accessible and layout correct |
| Startup disabled | No stale app-owned registration |
| Shutdown | Native resources and subscriptions released |

## Performance protocol
Record Windows build, hardware, display scaling, power mode, and configuration. Warm up, measure idle for a fixed interval (e.g. five minutes), repeat at least three times, and report median/range. Record cold start to first frame, working set, idle/peak CPU, background threads/timers, media-update CPU, and backdrop/album-art impact. Compare backdrop on/off. Store raw results in a versioned report.

## Build validation
On Windows with the .NET SDK:
```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet test .\GlassIsland.sln -c Release
```
The test command becomes meaningful after test projects are added.

## Release blockers
Correctness-affecting build failures; crash on unavailable integrations; high-severity privacy/security issues; unclean startup removal; undocumented notification access; keyboard/DPI usability failures; missing evidence for published performance claims.
