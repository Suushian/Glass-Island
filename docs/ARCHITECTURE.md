# Architecture

## Stack

- C# and WPF on `.NET 8`, targeting `net8.0-windows10.0.19041.0`.
- Windows Global System Media Transport Controls (GSMTC) for supported media sessions.
- Windows Forms `NotifyIcon` for the system tray.
- No audio-control or notification-listener integration.

The current implementation keeps the WPF shell and media adapter in `src/GlassIsland.App/`. This is intentionally simpler than the future target layout; no additional projects are required for the current scope.

## Implemented media adapter

`MediaSessionService`:
- Requests the GSMTC session manager.
- Tracks the current session and listens for session-list, current-session, media-property, and playback-state changes.
- Publishes media snapshots and safe fallback states when metadata or the session is unavailable.
- Exposes previous, play/pause, and next commands and disables commands the active player declines.
- Unsubscribes event handlers during disposal.

The UI marshals updates to the WPF Dispatcher and keeps the shell usable if GSMTC initialization or a media session is unavailable.

## State flow

1. Start the shell in a collapsed state.
2. Initialize the media-session adapter.
3. Publish the initial media snapshot or an unavailable state.
4. Marshal state changes onto the WPF Dispatcher.
5. Update the metadata and transport controls.
6. Unsubscribe from session events on application exit.

## Resource and threading policy

Prefer OS events over polling. Keep callbacks short; marshal UI state to the Dispatcher. Avoid blocking UI calls, idle animations, and repeated layout work. Dispose subscriptions and native resources deterministically.

## Windowing

Use a borderless topmost window without a normal taskbar button. Position against the primary monitor's work area and recalculate after display topology changes. Passive media updates must not steal focus. Mixed-DPI and multi-monitor behavior remain validation items.

## Availability and limitations

- GSMTC exposes only the metadata and transport capabilities published by a media player.
- Some players do not provide track title/artist or decline transport commands.
- Fallback labels improve clarity but cannot reconstruct metadata that the player never publishes.
- No notification capability, notification permission flow, system-volume adapter, or package identity is required by the current product scope.
