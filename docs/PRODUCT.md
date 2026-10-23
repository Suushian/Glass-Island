# Product Requirements — GlassIsland

## Summary
GlassIsland is a lightweight Windows desktop companion inspired by Dynamic Island interactions. A compact floating surface near the top-center of a display presents media state, system audio controls, and—only where supported—notification previews.

## Problem
Checking a track, pausing playback, adjusting volume, or noticing an alert often requires opening a separate system panel or switching applications. GlassIsland aims to make these actions glanceable without becoming a full dashboard.

## Target users
- Windows users who listen to media while working.
- People who want a compact, unobtrusive desktop overlay.
- Users who value keyboard/mouse access and low background resource use.

## Goals
- Combine media, volume, and supported notifications in one compact overlay.
- Remain unobtrusive while idle and avoid continuous polling.
- Keep integrations independent so one failure does not stop the shell.
- Keep data local and avoid telemetry by default.
- Make startup behavior opt-in and reversible.

## Non-goals
Cloud accounts/synchronization, replacing Windows system controls, unrestricted notification history, guaranteed support for every media player, advertisements, analytics, plugins, macOS, and mobile.

## Functional scope
- **P0 — Shell:** borderless floating window, collapsed/expanded states, tray show/hide/exit, display/DPI recovery, graceful failures.
- **P1 — Media:** supported session metadata and playback state; transport commands when available.
- **P1 — Audio:** default output volume/mute state, changes, and endpoint recovery.
- **P1 — Notifications:** first validate documented APIs and permissions; never imply unsupported access is implemented.
- **P2 — Personalization:** opt-in startup, reduced motion, privacy controls, display placement if feasible.

## Success measures (proposed, not measured)
- No unnecessary recurring work while idle.
- Core features survive unavailable integrations.
- Startup can be disabled without stale registration.
- Accessibility and supported OS scenarios pass a documented test matrix.
- CPU, memory, and startup claims are backed by repeatable measurements.
