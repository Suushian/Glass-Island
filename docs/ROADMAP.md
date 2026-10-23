# Implementation Roadmap

## Milestone 0 — repository baseline
- [x] Add source structure and reproducible build instructions.
- [x] Add a Windows Release build workflow.
- [ ] Build on a clean Windows development machine.
- [ ] Record baseline startup time, working set, and idle CPU.
**Exit:** Release build succeeds; machine/OS are recorded; no unmeasured performance claims.

**Status:** source structure and CI workflow are committed on the Phase 1 branch; Windows build verification remains pending.

## Milestone 1 — stable shell
- [x] Borderless topmost WPF window and collapsed/expanded interaction.
- [x] Tray show/hide/exit and explicit unavailable states for later integrations.
- [x] Eased transitions with no idle animation loop.
- [x] Reposition on display-settings changes and declare per-monitor DPI awareness.
- [ ] Verify no off-screen placement across monitor changes and mixed-DPI configurations.
- [ ] Verify focus behavior and responsiveness on Windows.
**Exit:** no off-screen placement after monitor changes; no focus stealing during passive updates; shell remains responsive when integrations are unavailable.

**Status:** first implementation committed; this milestone is not considered fully verified until Windows build and manual acceptance checks pass. Current placement defaults to the primary display.

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
