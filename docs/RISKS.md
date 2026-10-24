# Risks, Assumptions, and Open Decisions

## Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Media metadata varies by player | Medium | Medium | Fallback UI and player/session matrix |
| A player does not support a transport command | Medium | Medium | Disable the declined command and keep other controls usable |
| GSMTC callback/lifecycle bugs | Medium | High | Narrow adapter boundary, deterministic cleanup, regression tests |
| DPI/monitor changes move island off-screen | Medium | Medium | Recalculate placement and test display combinations |
| Startup registration remains after disable/uninstall if added later | Medium | Medium | Centralized registration and cleanup tests |
| Always-on-top overlay obstructs full-screen apps | Medium | Medium | Define full-screen policy and hide behavior |
| Performance assumptions are wrong | Medium | Medium | Reproducible baseline before optimization claims |
| Self-contained distribution is too large | Low/medium | Medium | Compare deployment modes |

## Assumptions

- Initial target is Windows desktop, not macOS/Linux.
- Low background resource use is more important than complex visual effects.
- Local-only behavior is sufficient for the first release.
- C# / WPF is acceptable for the initial shell.
- GSMTC support and metadata vary by media player.
- Exact Windows versions and packaging requirements remain unconfirmed.

## Open decisions

1. Minimum supported Windows build?
2. Multi-monitor support in first public release?
3. Auto-hide during full-screen games/video?
4. Signed installer required? MSIX or another installer?
5. x64 only or ARM64 too?
6. User-selectable display and top offset?

## Technical spikes

- **Native backdrop:** compare WPF transparency with supported native options; measure idle CPU/memory and visual consistency; keep fallback.
- **Media-session events:** validate callback threading, latency, session changes, missing metadata, transport support, and shutdown on Windows.

Notification reading and system volume controls are intentionally outside the current product scope.
