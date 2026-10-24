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

## Phase 2 Windows integration scenarios
| Scenario | Expected behavior |
|---|---|
| No current media session | Shell remains usable; media controls are disabled |
| Session appears, changes, or disappears | Metadata/playback state refreshes; stale session data is not retained |
| Empty or placeholder title/artist | Empty values and common placeholders such as “Unknown artist” are treated as missing; album artist is used when available; a missing artist with a valid title gets an artist-specific fallback |
| Telegram session with no track metadata | Show “رسانهٔ فعال از Telegram” and “اطلاعات آهنگ ارائه نشده”; do not claim the actual track title was recovered |
| Player does not support a transport command | If the command is declined, disable only that command for the current snapshot; keep the shell and other controls usable |
| External volume or mute change | Slider and mute label update from endpoint notifications |
| User changes volume or mute | Default multimedia output endpoint reflects the requested value |
| Default output device changes or disappears | Adapter rebinds or displays an explicit unavailable state |
| App exits during callbacks | Media/audio event subscriptions and endpoint resources are released |

The user manually tested the Phase 2 Windows x64 build and reported that the application and media/audio behavior work correctly, including Telegram track title and artist display. The latest Windows CI workflow validates compilation and publishing, not runtime integration behavior. Automated unit and integration tests are still not implemented, so repeat this scenario matrix for regression checks and before production release.

The GSMTC API only exposes metadata published by the media player. A fallback can make missing metadata understandable, but it cannot recover a real song title that the player does not publish.

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
