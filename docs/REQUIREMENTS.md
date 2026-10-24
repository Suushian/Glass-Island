# Requirements and Acceptance Criteria

## Functional requirements

### FR-001 Floating shell

Display a borderless floating surface near the top-center of the primary display.
- No normal taskbar button.
- Placement recovers after display changes.
- Passive updates do not activate or steal focus.

### FR-002 Expand/collapse

Pointer and keyboard can expand/collapse the island. Focus is predictable; animation can be reduced/disabled; supported DPI/text scaling does not clip controls.

### FR-003 Media session

Show metadata and playback state when a supported Windows media session provides them.
- Missing metadata does not crash the app and uses a clear fallback.
- Session disappearance clears stale state.
- Previous, play/pause, and next are enabled only when the current player reports support.
- A command declined by a player disables that command without breaking other controls.
- Event subscriptions are released on shutdown.

### FR-004 Tray and exit

System tray supports show, hide, and explicit exit. Exit disposes subscriptions and native resources; relaunch restores valid shell state.

## Non-functional requirements

- **NFR-001 Efficiency:** no continuous polling/animation while idle without justification.
- **NFR-002 Resilience:** unavailable GSMTC or media sessions do not terminate the shell.
- **NFR-003 Privacy:** no telemetry by default; media metadata is not logged or persisted by the app.
- **NFR-004 Accessibility:** keyboard navigation, accessible names, visible focus, scalable text.
- **NFR-005 Compatibility:** document supported Windows builds and GSMTC/player limitations.
- **NFR-006 Evidence:** performance claims require a repeatable measurement procedure.

## Explicitly out of scope

Notification reading/previews/history, notification permission prompts, package identity for notification access, system output volume/mute, cloud sync, account login, replacing Windows system controls, elevated services/drivers, and guaranteed support for every media player.
