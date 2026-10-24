# Risks, Assumptions, and Open Decisions

## Risks
| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Notification access unavailable for desired scenarios | High | High | Early API/permission proof-of-concept; redesign scope if unsupported |
| Media session metadata varies by player | Medium | Medium | Fallback UI and player/session matrix |
| Core Audio COM lifecycle bugs | Medium | High | Narrow adapter boundary, deterministic cleanup, integration tests |
| Media/audio adapter behavior differs by player or endpoint | Medium | Medium | Test supported players, missing metadata, external volume changes, device switches, and shutdown on Windows |
| DPI/monitor changes move island off-screen | Medium | Medium | Recalculate placement and test display combinations |
| Startup registration remains after disable/uninstall | Medium | Medium | Centralized registration and cleanup tests |
| Always-on-top overlay obstructs full-screen apps | Medium | Medium | Define full-screen policy and hide behavior |
| Performance assumptions are wrong | Medium | Medium | Reproducible baseline before optimization claims |
| Self-contained distribution is too large | Low/medium | Medium | Compare deployment modes |

## Assumptions
- Initial target is Windows desktop, not macOS/Linux.
- Low background resource use is more important than complex visual effects.
- Local-only behavior is sufficient for the first release.
- C# / WPF is acceptable for the initial shell.
- Exact Windows versions and packaging requirements remain unconfirmed.

## Open decisions
1. Minimum supported Windows build?
2. Multi-monitor support in first public release?
3. Notification preview: message content, app-only, or opt-in?
4. Signed installer required? MSIX or another installer?
5. Auto-hide during full-screen games/video?
6. x64 only or ARM64 too?
7. User-selectable display and top offset?

## Technical spikes
- **Notifications:** API, documented support, permissions, OS versions, limitations, test matrix.
- **Native backdrop:** compare WPF transparency with supported native options; measure idle CPU/memory and visual consistency; keep fallback.
- **Media/audio events:** validate callback threading, latency, endpoint/session changes, and shutdown.
