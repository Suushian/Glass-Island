# UX and Visual Design

## Direction

A compact dark-glass surface with restrained highlights. Readability and idle efficiency take priority over visual effects.

## Interaction

- **Collapsed:** small horizontal capsule near the top-center of the primary display.
- **Expanded:** one focused media section with track title, artist, playback state, and supported transport controls.
- **Language and voice:** the entire in-app experience is English. Use friendly, conversational microcopy with short, natural phrases; keep errors precise rather than jokey.
- No notification permission or first-run integration prompt.
- Passive media updates must not steal focus.
- Pointer and keyboard can expand/collapse the island.
- Escape collapses the expanded surface.
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

## Media states

- Active session with metadata: show the published title and artist.
- Missing title/artist: show a concise fallback; do not imply that missing metadata was recovered.
- No active session or unavailable GSMTC: explain that nothing is playing and disable transport controls.
- Unsupported command: disable only the command the player does not support.

## Accessibility

- Keyboard-operable actions and visible focus.
- Accessible names and tooltips for icon-only buttons.
- Support text scaling and high-contrast settings where feasible.
- Respect reduced-motion preferences.
- Do not rely on color alone for playing or unavailable states.
- Keep pointer targets reliably usable.

## Acceptance checks

Readable over varied wallpapers; no focus stealing; no clipping at supported DPI/text scaling; no off-screen placement after monitor changes; idle animations stop; media controls remain keyboard-accessible.
