# Implementation Roadmap

## Milestone 0 — repository baseline
- Add source structure and reproducible build instructions.
- Build on a clean Windows development machine.
- Record baseline startup time, working set, and idle CPU.
**Exit:** Release build succeeds; machine/OS are recorded; no unmeasured performance claims.

## Milestone 1 — stable shell
- Borderless topmost window, collapsed/expanded interaction, monitor/DPI-aware placement.
- Tray show/hide/exit, settings model, feature availability states.
**Exit:** no off-screen placement after monitor changes; no focus stealing; shell remains responsive when integrations are unavailable.

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
