# Security and Privacy

## Data minimization
The first release should require no account, network service, analytics, or remote storage. Keep media/audio state in memory. If notification previews are implemented, retain text only briefly and never log it. Store only necessary user settings.

## Notification privacy
- Use documented APIs and comply with permissions.
- Never read private notification stores or bypass platform controls.
- Keep message previews off by default until implications are clearly explained, or require explicit opt-in.
- Offer app-only previews where feasible.
- Avoid persistence of notification content and redact diagnostics.

## Startup and privileges
Startup is user-visible, opt-in, and reversible. Do not require elevation for per-user startup. Do not recreate registrations after disable. Remove only app-owned entries. Do not set `uiAccess=true` without a justified, signed accessibility requirement. No service/driver installation in the initial scope.

## Dependencies and supply chain
Keep dependencies minimal; review maintenance, licensing, transitive packages, and security history. Pin versions/lock dependencies where appropriate. Do not download executable code dynamically. Sign release artifacts when distribution is established.

## Diagnostics
Default to no telemetry. If local logs are introduced, rotate them, avoid personal content, and provide a clear path to delete them. Any future telemetry needs a documented purpose, consent design, and privacy review.

## Initial threat model
| Threat | Mitigation |
|---|---|
| Private notification exposed | Preview controls, privacy-first defaults |
| Malformed media metadata | Treat external text as untrusted; bound text/image sizes |
| Native callback crash/leak | Safe callback boundaries, deterministic cleanup, tests |
| Stale startup entry | Centralized registration and cleanup tests |
| Dependency compromise | Minimal dependencies, review, signed releases |
| Sensitive logs | Redaction and data minimization |

## Review checklist
- [ ] No unnecessary elevation or network traffic.
- [ ] No notification text in logs.
- [ ] Startup visible and reversible.
- [ ] Dependencies and licenses reviewed.
- [ ] Native handles/COM objects released deterministically.
- [ ] Errors do not expose sensitive paths/data.
- [ ] Installer/uninstaller touch only app-owned resources.
