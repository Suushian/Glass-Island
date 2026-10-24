# Requirements and Acceptance Criteria

## Functional requirements

### FR-001 Floating shell
Display a borderless floating surface near the top-center of the selected display.
- No normal taskbar button.
- Placement recovers after display changes.
- Passive updates do not activate or steal focus.

### FR-002 Expand/collapse
Pointer and keyboard can expand/collapse the island. Focus is predictable; animation can be reduced/disabled; supported DPI/text scaling does not clip controls.

### FR-003 Media
Show metadata and playback state when a supported session provides them.
- Missing metadata does not crash the app.
- Session disappearance clears stale state.
- Unsupported commands are disabled or report a clear result.
- Subscriptions are released on shutdown.

### FR-004 Audio
Read/change default output volume and mute.
- Validate values and reflect external changes.
- Handle endpoint changes and disconnection.
- Avoid steady-state polling unless a measured platform constraint requires it.

### FR-005 Notifications
On first launch, request Windows notification access through the documented, permission-compatible API and, after permission is granted, display supported existing and incoming notifications from any app that Windows exposes.
- Explain the purpose of access in friendly English before the Windows permission prompt.
- Handle Allowed, Denied, Unspecified, and later-revoked access states.
- Show all supported notifications in a scrollable list, with source app, timestamp, title, and body when exposed by Windows.
- Do not log or persist notification content.
- Show a clear unavailable state if package identity, capability, API, or permission is missing.
- Never represent unsupported access as implemented.

### FR-006 Startup
Allow users to enable/disable launch at sign-in.
- Default is documented.
- UI reflects actual OS registration.
- Disable removes only app-owned registration.
- No elevation for ordinary per-user startup.

### FR-007 Tray and exit
System tray supports hide/show and exit. Exit disposes subscriptions/native resources; relaunch restores valid state.

## Non-functional requirements
- **NFR-001 Efficiency:** no continuous polling/animation while idle without justification.
- **NFR-002 Resilience:** a failed adapter must not terminate the shell.
- **NFR-003 Privacy:** no telemetry or notification-content logging by default.
- **NFR-004 Accessibility:** keyboard navigation, accessible names, visible focus, scalable text.
- **NFR-005 Compatibility:** document supported Windows builds and API limitations.
- **NFR-006 Evidence:** performance claims require a repeatable measurement procedure and build.

## Out of scope
Cloud sync, account login, replacing Windows system controls, unrestricted notification history, elevated services/drivers.
