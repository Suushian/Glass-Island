# Testing and Quality Plan

## Test layers

- **Unit:** media snapshot normalization, fallback selection, availability transitions, command-support mapping.
- **Adapter:** GSMTC session lifecycle, missing metadata, unsupported commands, stale-session protection, and event cleanup.
- **UI:** placement, expand/collapse, accessible names, disabled states, focus behavior, and multi-monitor/DPI recovery.
- **Manual integration:** run on declared Windows builds and record OS build, display scale, media player, and result.

## Media scenario matrix

| Scenario | Expected behavior |
|---|---|
| No media session | Shell works; media controls are disabled |
| Session starts or changes | Metadata/playback state updates from events |
| Session disappears | Stale session data is cleared |
| Missing or placeholder title/artist | Safe fallback label; album artist used if available |
| Telegram session without track metadata | Source-aware fallback; do not claim actual track metadata was recovered |
| Player declines a transport command | Disable only that command; keep other controls usable |
| Player metadata changes while another session becomes active | Old session result does not overwrite current UI |
| App exits during callbacks | Media event subscriptions are released |
| GSMTC initialization fails | Shell remains usable with an unavailable state |

The user manually tested the earlier Phase 2 Windows x64 build and reported that media behavior works, including Telegram title and artist display. The media-only cleanup still needs a fresh CI build/publish and a runtime smoke test. Automated unit/integration tests are not yet implemented.

GSMTC only exposes metadata published by the media player. A fallback cannot recover a real track title that the player does not publish.

## Performance protocol

Record Windows build, hardware, display scaling, power mode, and configuration. Warm up, measure idle for a fixed interval (e.g. five minutes), repeat at least three times, and report median/range. Record cold start to first frame, working set, idle/peak CPU, background threads/timers, and media-update CPU. Store raw results in a versioned report.

## Build validation

On Windows with the .NET SDK:

```powershell
dotnet restore .\GlassIsland.sln
dotnet build .\GlassIsland.sln -c Release
dotnet test .\GlassIsland.sln -c Release
```

The test command becomes meaningful after test projects are added.

## Release blockers

Build failures; crash on unavailable media sessions; high-severity privacy/security issues; unclean startup removal if implemented; keyboard/DPI usability failures; missing evidence for published performance claims.
