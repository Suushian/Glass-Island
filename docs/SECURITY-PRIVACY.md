# Security and Privacy

## Data minimization

The initial product is local-only. It requires no account, network service, analytics, or remote storage. Media metadata and playback state are displayed in memory only; the app does not intentionally log or persist track metadata.

## Platform access

- Use the documented Windows GSMTC API for supported media sessions.
- Do not access private application stores or bypass Windows access controls.
- The current app does not request notification access, read notification content, declare `userNotificationListener`, or control system output volume.
- No package identity or elevated service is required for the current media-only feature set.

## Startup and privileges

Startup is user-visible, opt-in, and reversible if implemented. Do not require elevation for per-user startup. Do not recreate registrations after disable. Remove only app-owned entries. Do not set `uiAccess=true` without a justified, signed accessibility requirement. No service/driver installation in the initial scope.

## Dependencies and supply chain

Keep dependencies minimal; review maintenance, licensing, transitive packages, and security history. Do not download executable code dynamically. The current media-only app does not require NAudio. Sign release artifacts when distribution is established.

## Diagnostics

Default to no telemetry. If local logs are introduced, rotate them, avoid personal content, and provide a clear path to delete them. Any future telemetry needs a documented purpose, consent design, and privacy review.

## Initial threat model

| Threat | Mitigation |
|---|---|
| Malformed media metadata | Treat external text as untrusted; bound displayed text |
| Native callback crash/leak | Safe callback boundaries, deterministic cleanup, tests |
| Stale startup entry | Centralized registration and cleanup tests |
| Dependency compromise | Minimal dependencies, review, signed releases |
| Sensitive logs | Redaction and data minimization |

## Review checklist

- [ ] No unnecessary elevation or network traffic.
- [ ] No media metadata in logs.
- [ ] Startup visible and reversible if implemented.
- [ ] Dependencies and licenses reviewed.
- [ ] Native handles/COM objects released deterministically.
- [ ] Errors do not expose sensitive paths/data.
- [ ] Installer/uninstaller touch only app-owned resources.
