# Architecture Decision Records

## ADR-001 — C# / WPF for initial shell
**Status:** Accepted for prototype.  
**Context:** Windows-only utility needing a compact always-on-top window and Windows API access.  
**Decision:** Use C# and WPF on a Windows-specific .NET target.  
**Trade-offs:** Mature desktop model and direct interop; modern backdrop effects require deliberate work. Revisit if native Windows 11 backdrop fidelity becomes mandatory.

## ADR-002 — Avoid Electron/WebView2 initially
**Status:** Accepted.  
A small interface does not justify a browser runtime unless a demonstrated requirement outweighs memory and process overhead. Trade-off: less CSS/UI reuse.

## ADR-003 — Event-driven integrations
**Status:** Accepted.  
Subscribe to OS/media/audio changes where supported; do not continuously poll in the idle path. Callback threading, reconnect behavior, and cleanup are required.

## ADR-004 — Validate notification access
**Status:** Needs validation.  
Do not promise broad notification history access. Document API, permissions, supported OS versions, failure modes, and a test matrix before implementation.

## ADR-005 — Startup is opt-in
**Status:** Planned.  
Launch-at-sign-in must be user-visible, reversible, and cleaned up without modifying unrelated registrations.

## ADR-006 — No telemetry by default
**Status:** Proposed.  
No analytics or usage data transmission in the initial release. Local diagnostics, if added, should be minimal and user-controlled.
