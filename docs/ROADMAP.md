# Implementation Roadmap

## Milestone 0 — repository baseline
- [x] Add source structure and reproducible build instructions.
- [x] Add a Windows Release build workflow.
- [x] Confirm successful Windows CI Release build and x64 Debug publish.
- [ ] Record baseline startup time, working set, and idle CPU.
**Exit:** Release build succeeds; machine/OS are recorded; no unmeasured performance claims.

**Status:** CI build and publish succeeded on Windows GitHub Actions. Performance baselines remain unmeasured.

## Milestone 1 — stable shell
- [x] Borderless topmost WPF window and collapsed/expanded interaction.
- [x] Tray show/hide/exit and explicit unavailable states for later integrations.
- [x] Eased transitions with no idle animation loop.
- [x] Reposition on display-settings changes and declare per-monitor DPI awareness.
- [x] Wire application startup to `MainWindow.xaml` and verify the published executable launches.
- [x] Verify basic startup, visible overlay, expand/collapse, Escape-to-collapse, and tray Exit on Windows (user-reported manual acceptance).
- [ ] Verify no off-screen placement across monitor changes and mixed-DPI configurations.
- [ ] Verify focus behavior and responsiveness on Windows.
**Exit:** no off-screen placement after monitor changes; no focus stealing during passive updates; shell remains responsive when integrations are unavailable.

**Status:** Phase 1 basic acceptance is complete: Windows CI passed and user-reported manual smoke tests passed. Advanced multi-monitor/mixed-DPI and focus behavior remain explicit follow-up validation items. Current placement defaults to the primary display.

## Milestone 2 — media and audio
- [x] Add an event-driven GSMTC adapter for current-session metadata/playback state and supported previous/play-pause/next commands.
- [x] Add default multimedia output volume/mute read/write and event-driven volume/default-endpoint notifications.
- [x] Manually validate media-session behavior, metadata fallbacks, unsupported transport commands, volume/mute, endpoint changes, and shutdown on Windows (user-reported acceptance).
- [x] Confirm Telegram track title and artist display correctly on the user's Windows installation.
**Exit:** Phase 2 implementation, Windows CI build/publish, and user-reported manual acceptance are complete. Automated regression tests and extended multi-monitor/performance testing remain follow-up quality work.

### Phase 2 metadata follow-up
- [x] Replace empty/common placeholder title and artist values with meaningful fallbacks.
- [x] Use album artist when the track artist is missing.
- [x] Identify common media sources in the fallback title where GSMTC exposes a recognizable app ID.
- [x] Disable an individual transport command when the active player declines it.
- [x] Confirm Telegram metadata behavior on the user's Windows installation.
- [ ] Add automated tests once the test project is introduced.

**Limitation:** GSMTC only provides metadata published by the media player. The fallback improves the UI state but cannot infer the real track title when a player does not publish it.

## Milestone 3 — notification proof-of-concept (Phase 3)
- [x] Review Microsoft's documented notification-listener API, permission flow, capability declaration, and supported notification types.
- [x] Record the packaging/capability dependency and privacy constraints before implementing notification UX.
- [x] Add the first listener adapter and an English, scrollable notification list UI; this is code-level work only, not proof that Windows access works.
- [ ] Establish package identity and declare `userNotificationListener` in a test package; validate on a real Windows installation.
- [ ] Validate the first-run access request on the UI thread and verify Allowed, Denied, and Unspecified/revoked states on a packaged Windows installation.
- [ ] Verify notification enumeration/change events, source app metadata, and graceful behavior when no access or no notifications are available on a packaged Windows installation.
- [ ] Confirm privacy safeguards: no notification text in logs, no unnecessary persistence, and a clear English explanation before the Windows permission prompt.
- [ ] Add automated coverage for state mapping and lifecycle; document Windows integration results.
**Exit:** a real Windows test demonstrates the supported access path and permission behavior; limitations and package requirements are documented; no unsupported access techniques and no notification content in logs.

**Phase 3 status:** The first listener adapter and scrollable notification list are implemented, and in-app copy is English-only with a friendly conversational voice. This is not yet a runtime proof: the current CI artifact is a self-contained executable, while the documented listener requires the `userNotificationListener` package capability and explicit user permission. Notification behavior remains unverified until the packaged runtime is tested on Windows.

## Milestone 4 — startup and settings
Opt-in per-user launch, reliable cleanup, start collapsed, reduced-motion/privacy settings, safe persistence defaults.

## Milestone 5 — visual polish and performance
Explore supported native backdrops; test themes/high contrast, animations, multi-monitor/DPI; profile rendering.

## Milestone 6 — packaging and release
Choose installer/MSIX, signing/update policy, clean install/upgrade/uninstall tests, support matrix and known issues.

## Priorities
P0: reliable shell, lifecycle, media/audio foundations, graceful failure.  
P1: validated notifications, startup, tray, settings.  
P2: advanced themes, per-monitor customization, optional integrations.

No calendar estimates until Windows support, notification scope, and packaging requirements are confirmed.
