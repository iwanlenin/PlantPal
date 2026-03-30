# PlantPal Design System — "The Botanical Archivist"

Status: **Confirmed**

---

## Design tokens

| Token | Value |
|---|---|
| Font | Plus Jakarta Sans |
| Background | `#faf9f5` warm off-white |
| Primary | `#396637` → `#517f4e` (gradient) |
| Text | `#1b1c1a` (never pure black) |
| Surface stack | surface → container-low → container-highest |
| Card radius | xl = `1.5rem` |
| Chip radius | full = `9999px` |
| Icons | Material Symbols Outlined |
| Nav style | Glassmorphic 70% opacity + 16px blur |
| Borders | None — tonal layering only |
| Shadows | Ambient only: 24–40px blur, 4–6% opacity |
| Headlines | tracking `-0.02em`, editorial left-aligned |
| Labels | UPPERCASE + tracking `+0.05em` |

---

## Design rules

- No 1px divider lines — use spacing or color-block shifts instead
- No hard borders — ghost border only when required: outline-variant at 20% opacity
- Primary CTAs always use gradient fill (primary → primary-container, 135°)
- Input fields: surface-container-highest bg, no border, xl radius
- Overdue badge: error-container bg + error dot indicator
- Upcoming items: compact row with circular thumbnail, tertiary color for day count
- Dashboard editorial header: category label + large bold headline
- Active nav item: gradient pill (primary → primary-container)
- FAB: round, gradient fill, bottom-right, 64×64px
- Hydration progress: tertiary-container track + tertiary fill, pill shape

---

## Confirmed screens

### Dashboard
Editorial header "Your collection is breathing today." · Thirsty Now cards (large photo + Watered CTA) · Upcoming Care compact rows · Weekly Hydration gauge

### Add Plant
Wide photo banner · 2-col form grid · SUGGESTED badge (tertiary-container) · Pro Care Tip info card · Save to Collection gradient button

### Empty state
Large rotated image card · NEW BEGINNINGS label · Blank canvas headline · Add Your First Plant CTA · Quick tips bento grid

---

## MAUI implementation notes

> [!IMPORTANT]
> MAUI does not support Tailwind or Material Symbols directly. Translate as follows:

| Web/Design spec | MAUI equivalent |
|---|---|
| Material Symbols Outlined | `MauiIcons.Material` package |
| Color tokens | `ResourceDictionary` Color entries in `Resources/Styles/Colors.xaml` |
| Surface hierarchy | `BackgroundColor` on layouts |
| Glassmorphism nav | MAUI Shell TabBar with semi-transparent `BoxView` overlay |
| Gradient CTAs | `LinearGradientBrush` on Button background |
| Card radius xl | `CornerRadius="24"` on Border/Frame |
| Chip radius full | `CornerRadius="9999"` on Border |
