# Product Requirements — GlassIsland

## Summary

GlassIsland is a lightweight Windows desktop companion inspired by Dynamic Island interactions. Its only integration is the active supported media session: the floating surface shows track metadata and playback state and exposes transport controls.

## Problem

Checking a track or pausing playback often requires switching applications. GlassIsland makes the current media session glanceable and controllable without becoming a full dashboard.

## Target users

- Windows users who listen to media while working.
- People who want a compact, unobtrusive desktop overlay.
- Users who value keyboard/mouse access and low background resource use.

## Goals

- Display current-session metadata and playback state.
- Offer previous, play/pause, and next when supported by the player.
- Remain unobtrusive while idle and avoid continuous polling.
- Keep media metadata local and avoid telemetry by default.
- Continue to function when no supported media session is available.

## Explicit non-goals

- Windows notification previews, notification history, or notification-listener access.
- System volume or mute controls.
- Cloud accounts/synchronization, analytics, advertisements, plugins, macOS, and mobile.
- Guaranteed metadata or transport support for every media player.

## Functional scope

- **P0 — Shell:** borderless floating window, collapsed/expanded states, tray show/hide/exit, display/DPI recovery, graceful failure.
- **P1 — Media:** supported session metadata and playback state; transport commands when available.
- **P2 — Polish:** accessibility, performance, mixed-DPI behavior, and release packaging.

## Success measures (proposed, not measured)

- No unnecessary recurring work while idle.
- Media-session changes update the UI without stale-session data.
- Unsupported players/commands do not crash the shell.
- CPU, memory, startup, and compatibility claims are backed by repeatable measurements.
