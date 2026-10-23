# UX and Visual Design

## Direction
A compact dark-glass surface with restrained highlights. Readability and idle efficiency take priority over visual effects.

## Interaction
- **Collapsed:** small horizontal capsule near the top-center of the selected display; show only the most relevant active state.
- **Expanded:** media controls, volume, and supported notification previews in clear sections.
- Passive updates must not steal focus.
- Pointer and keyboard can expand/collapse the island.
- Clicking outside may collapse it, but must not interrupt an active control interaction.
- Unsupported features show a clear unavailable state rather than simulated functionality.

## Initial visual tokens
| Token | Starting point |
|---|---|
| Surface | Dark neutral, roughly 88–94% opaque |
| Border | Low-contrast white with subtle alpha |
| Text | Near-white primary; muted neutral secondary |
| Accent | Restrained violet or system accent |
| Radius | Capsule collapsed; 16–28 px expanded |
| Shadow | Soft and static |
| Motion | Short transitions; no idle loop |
| Typography | Segoe UI/system UI, scalable |
| Spacing | 4 px base rhythm |

These are starting values, not validated design specifications. Maintain an opaque fallback when composition effects are unavailable.

## Accessibility
- Keyboard-operable actions and visible focus.
- Accessible names and tooltips for icon-only buttons.
- Support text scaling and high-contrast settings where feasible.
- Respect reduced-motion preferences.
- Do not rely on color alone for playing, muted, or error states.
- Sliders must work with keyboard and screen readers.
- Keep pointer targets reliably usable.

## Notification privacy
Notification content can expose messages and one-time codes. Offer previews off by default or require explicit opt-in before displaying message text. Consider app-only previews, configurable expiry, and hiding previews while a full-screen app is active if reliably detectable. Do not log preview content.

## Acceptance checks
Readable over varied wallpapers; no focus stealing; no clipping at supported DPI/text scaling; no off-screen placement after monitor changes; idle animations stop; expanded controls remain keyboard-accessible.
