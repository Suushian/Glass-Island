# UX and Visual Design

## Direction

**Soft White Glass**: a light, near-opaque floating surface with restrained violet accents, fine borders, calm depth, and precise typography. The surface should feel native to a modern Windows desktop without competing with the content behind it. Prefer readability over blur-heavy effects; retain a solid light surface and border as the dependable fallback.

## Visual system

| Token | Value / intent |
|---|---|
| Surface | Near-opaque white glass, `#FDFDFEFF` |
| Elevated surface | Soft cool white, `#FFF8F9FC` |
| Border | Light neutral, `#FFE4E7F0` |
| Primary text | Deep charcoal, `#FF202334` |
| Secondary text | Muted slate, `#FF73788A` |
| Tertiary text | Soft slate, `#FF969BAD` |
| Accent | Muted violet, `#FF7C70E8` |
| Accent surface | Pale violet, `#FFF0EDFF` |
| Radius | 10–28 px; capsule-like shell, compact control surfaces |
| Spacing | 4 px base rhythm; use consistent 8/12/16 px groupings |
| Typography | Segoe UI / Windows system font; title 13–14 px, secondary 10–11 px |
| Shadow | Soft neutral shadow, low opacity; avoid heavy glow |
| Motion | 220–260 ms cubic easing; no ambient idle loops |

Values are implementation tokens, not user-tested results. The white surface is deliberately near-opaque to preserve contrast over varied wallpapers.

## Interaction

- **Collapsed:** compact capsule showing current media title, secondary artist/status, media glyph, and a clear expand affordance.
- **Expanded:** a focused media section with title, artist/status, previous, play/pause, and next. The primary play/pause action receives the strongest visual emphasis.
- Expand/collapse uses a short eased height transition with a subtle panel fade/vertical settle; avoid bounce and idle motion.
- Transport controls use consistent vector glyphs, clear hover/pressed/disabled states, accessible names, and tooltips.
- The entire in-app experience is English. Use concise, natural microcopy; errors and unavailable states should be explicit rather than jokey.
- No notification permission or first-run integration prompt.
- Passive media updates must not steal focus.
- Pointer and keyboard can expand/collapse the island.
- Escape collapses the expanded surface.
- Unsupported features show a clear unavailable state rather than simulated functionality.

## Media states

- Active session with metadata: show the published title and artist.
- Missing title/artist: show a concise fallback; do not imply that missing metadata was recovered.
- No active session or unavailable GSMTC: explain that nothing is playing and disable transport controls.
- Unsupported command: disable only the command the player does not support.
- Playback state is communicated through the play/pause glyph and its accessible name; do not rely on accent color alone.

## Accessibility and motion

- Keyboard-operable actions and visible focus.
- Accessible names and tooltips for icon-only buttons.
- Support text scaling and high-contrast settings where feasible.
- Skip nonessential motion when Windows client-area animations are disabled.
- Keep pointer targets reliably usable and maintain readable contrast on the near-opaque surface.
- Do not steal focus during passive media updates.

## Acceptance checks

- Readable over varied wallpapers; no focus stealing.
- No clipping at supported DPI/text scaling; no off-screen placement after monitor changes.
- Expanded and collapsed states animate without jumps, overlap, or stale opacity.
- Rapid expand/collapse input leaves the panel in a consistent state.
- Previous, play/pause, and next use correct glyphs and accurately reflect supported commands.
- Disabled controls remain visually distinct and cannot be invoked.
- Idle animations stop; media controls remain keyboard-accessible.
