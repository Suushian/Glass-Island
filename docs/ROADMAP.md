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
- Enumerate supported media sessions and show metadata/state.
- Transport commands where supported.
- Read/set volume and mute; subscribe to endpoint changes.
**Exit:** success and unavailable cases tested; event subscriptions are cleaned up; no idle polling loop.

## Milestone 3 — notification proof-of-concept
- Validate documented APIs, permissions, supported OS versions, and feasible scenarios.
- Record API decision/limitations before building UX.
- Add privacy controls and integration tests only for supported behavior.
**Exit:** no unsupported access techniques; graceful unavailable state; no notification content in logs.

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
