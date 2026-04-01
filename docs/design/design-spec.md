# Design System Specification: The Botanical Archivist

## 1. Overview & Creative North Star
This design system is built upon the Creative North Star of **"The Botanical Archivist."** It rejects the cluttered, utility-first aesthetic of standard tracking apps in favor of a high-end, editorial experience that feels like a cross between a modern architectural greenhouse and a premium nature journal.

To move beyond a "template" look, we employ **intentional asymmetry** and **tonal layering**. Layouts should avoid rigid, centered grids. Instead, use off-balance compositions where a large `display-sm` headline might sit flush left while a glassmorphic card floats slightly offset to the right. We treat the screen not as a flat canvas, but as a curated space with depth, breathing room, and organic flow.

## 2. Color & Surface Philosophy
The palette is a sophisticated interplay of forest chromas and lithic neutrals. 

### The "No-Line" Rule
**Explicit Instruction:** Designers are prohibited from using 1px solid borders for sectioning or containment. Boundaries must be defined solely through background color shifts or subtle tonal transitions. For example, a feature section should be defined by transitioning from `surface` (#faf9f5) to `surface-container-low` (#f4f4f0), creating a soft "edge" through value change rather than a hard stroke.

### Surface Hierarchy & Nesting
Treat the UI as a series of physical layers—like stacked sheets of heavy-weight vellum.
- **Base Layer:** `surface` (#faf9f5) or `surface-bright`.
- **Primary Content Area:** `surface-container-low` (#f4f4f0).
- **Interactive Cards/Elements:** `surface-container-highest` (#e3e2df) or `surface-container-lowest` (#ffffff) to create "pop" through contrast.

### The "Glass & Gradient" Rule
To inject "soul" into the interface, use Glassmorphism for floating navigation bars or quick-action overlays. Use the `surface` token at 70% opacity with a `16px` backdrop-blur. 
For primary CTAs, avoid flat fills. Use a subtle linear gradient from `primary` (#396637) to `primary-container` (#517f4e) at a 135-degree angle to mimic the natural variation in a leaf’s surface.

## 3. Typography: Plus Jakarta Sans
We utilize **Plus Jakarta Sans** for its clean, modern geometry and friendly terminals. 

- **Display & Headlines:** Use `display-md` and `headline-lg` for editorial moments. Set these with a slightly tighter letter-spacing (-0.02em) to create an authoritative, premium feel.
- **Body:** Use `body-lg` (1rem) as the standard for readability. Ensure a generous line-height (1.6) to promote a "calm" reading experience.
- **Labels:** Use `label-md` in all-caps with increased letter-spacing (+0.05em) for category headers or metadata, referencing the aesthetic of botanical specimen tags.

## 4. Elevation & Depth
Traditional drop shadows are too "digital." We achieve hierarchy through **Tonal Layering** and **Ambient Light.**

### The Layering Principle
Depth is achieved by "stacking" the surface-container tiers. Place a `surface-container-lowest` card on a `surface-container-low` background to create a soft, natural lift.

### Ambient Shadows
When a floating effect is required (e.g., a FAB or a modal), shadows must be extra-diffused:
- **Blur:** 24px - 40px.
- **Opacity:** 4% - 6%.
- **Color:** Use a tinted version of `on-surface` (#1b1c1a) rather than pure black to keep the shadows "warm" and integrated.

### The "Ghost Border" Fallback
If a border is absolutely necessary for accessibility, it must be a **Ghost Border**: use the `outline-variant` (#c2c9bc) at 20% opacity. Never use 100% opaque, high-contrast borders.

## 5. Components

### Buttons
- **Primary:** Gradient fill (`primary` to `primary-container`), white text, `xl` (1.5rem) rounded corners.
- **Secondary:** `secondary-fixed` (#ffdcc5) background with `on-secondary-fixed` (#301400) text. No border.
- **Tertiary:** Text-only using `primary` color, with a slight background tint of `primary` at 8% opacity on hover.

### Input Fields
Avoid the "boxed" look. Use `surface-container-highest` as a solid background fill with an `xl` corner radius. The label sits above in `label-md`. On focus, the background shifts to `surface-container-lowest` with a subtle `primary` ghost border (20% opacity).

### Cards & Lists
**Strict Rule:** No divider lines. Separate list items using the spacing scale (e.g., `spacing-3` / 1rem) or by alternating subtle background shifts between `surface` and `surface-container-low`.
- **Cards:** Use `xl` (1.5rem) corner radius. Use `surface-container-low` for the card body to create a soft-touch feel.

### Specialized App Components
- **The Hydration Gauge:** A wide, horizontal pill-shaped progress bar using `tertiary-container` (#617e10) as the track and `tertiary` (#4b6400) as the fill.
- **Growth Timeline:** A vertical arrangement where the "line" is actually a `spacing-0.5` wide gap between `surface-container` blocks, creating a negative-space path.

## 6. Do’s and Don’ts

### Do:
- **Use White Space as a Tool:** Use `spacing-10` (3.5rem) or `spacing-12` (4rem) to separate major sections. Let the app breathe.
- **Embrace Asymmetry:** Align text to the left but allow imagery or cards to bleed off-center to create a dynamic, editorial rhythm.
- **Use Organic Shapes:** Utilize the `full` (9999px) roundedness for chips and tags to contrast with the `xl` roundedness of cards.

### Don’t:
- **Don’t Use Pure Black:** Always use `on-surface` (#1b1c1a) for text to maintain the soft, earthy tone.
- **Don’t Use Hard Dividers:** Never use a 1px line to separate content. Use a `spacing-4` (1.4rem) gap or a color-block shift instead.
- **Don’t Over-elevate:** Avoid heavy shadows. If a layout feels flat, try changing the background color of the container before reaching for a shadow effect.