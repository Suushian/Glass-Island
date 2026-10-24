# Implementation Roadmap

## Milestone 0 — repository baseline

- [x] Add source structure and reproducible build instructions.
- [x] Add a Windows Release build workflow.
- [x] Confirm Windows CI Release build and x64 publish.
- [ ] Record baseline startup time, working set, and idle CPU.

**Exit:** Release build succeeds; machine/OS are recorded; no unmeasured performance claims.

## Milestone 1 — stable shell

- [x] Borderless topmost WPF window and collapsed/expanded interaction.
- [x] Tray show/hide/exit and graceful unavailable states.
- [x] Eased transitions with no idle animation loop.
- [x] Reposition on display-settings changes and declare per-monitor DPI awareness.
- [x] Basic startup, visible shell, expand/collapse, Escape-to-collapse, and tray Exit manually accepted on Windows.
- [ ] Verify no off-screen placement across monitor changes and mixed-DPI configurations.
- [ ] Verify focus behavior and responsiveness on Windows.

**Exit:** no off-screen placement after monitor changes; no focus stealing during passive updates; shell remains responsive when media is unavailable.

## Milestone 2 — media controls

- [x] Add an event-driven GSMTC adapter for current-session metadata/playback state and supported previous/play-pause/next commands.
- [x] Add meaningful fallbacks for missing title/artist metadata.
- [x] Use album artist when the track artist is missing.
- [x] Disable an individual transport command when the active player declines it.
- [x] Manually validate media-session behavior and Telegram title/artist display on the user's Windows installation.
- [ ] Add automated regression coverage for media-session state and lifecycle.
- [ ] Re-run the media acceptance matrix after the media-only cleanup.

**Limitation:** GSMTC only provides metadata published by the media player. The fallback cannot infer a real track title that the player does not publish.

## Product scope decision — media only

The notification-listener proof of concept and system audio/volume controls have been removed from the active product scope. Do not implement notification access, request notification permission, add `userNotificationListener`, or add audio endpoint controls unless the product scope is explicitly changed later. The old notification test/package workflow is not part of the media-only deliverable.

**Current cleanup acceptance:**
- [x] UI contains only shell and media-session controls.
- [x] No notification or audio adapter/service remains in the app project.
- [x] Unused NAudio dependency is removed.
- [x] Product, UX, architecture, security, test, and build docs describe media-only scope.
- [x] Notification-specific POC and package-identity runbooks are removed.
- [ ] Windows Release build and self-contained x64 publish pass.

## Milestone 3 — quality and accessibility

- [ ] Add automated tests for media state mapping and fallback behavior.
- [ ] Validate keyboard operation, accessible names, text scaling, and high contrast.
- [ ] Validate multi-monitor and mixed-DPI placement.
- [ ] Measure startup, idle CPU, and working set using a repeatable protocol.

## Milestone 4 — packaging and release

- [ ] Choose installer and signing approach.
- [ ] Test clean install, upgrade, and uninstall.
- [ ] Document supported Windows versions and known media-player limitations.

## Priorities

P0: reliable shell and media controls.  
P1: regression coverage, accessibility, and display compatibility.  
P2: visual polish and optional future features only after explicit scope review.
