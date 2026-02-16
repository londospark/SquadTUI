---
name: "Responsive TUI Layout Design"
description: "Design terminal UIs that scale across vastly different viewport sizes (40-col mobile to 160-col IDE) with progressive disclosure and layout fallbacks."
domain: "UX design, terminal UX, responsive design"
confidence: "medium"
source: "earned"
tools:
  - name: "ASCII layout sketching"
    description: "Use ASCII art to mock layouts at different breakpoints"
    when: "Before implementing, visualize 3+ breakpoints to ensure responsive behavior"
---

## Context

Terminal UIs face extreme viewport variability:
- **Mobile:** 40–79 columns (phone, small window)
- **Laptop:** 80–119 columns (standard terminal)
- **IDE:** 120+ columns (split panes, wide monitor)

Most TUI projects pick one and break at others. **Responsive TUI design** ensures functionality at all sizes, with graceful degradation and progressive disclosure.

## Patterns

### 1. Identify Breakpoints Early
Define 3+ layout tiers *before* designing:

| Tier | Range | Name | Layout Type |
|------|-------|------|------------|
| **Narrow** | <80 | Mobile | 1-column, full-screen sections |
| **Medium** | 80–119 | Laptop | 2-column (sidebar optional), horizontal tabs |
| **Wide** | ≥120 | IDE | 3-panel (sidebar + main + footer), all UI visible |

Each tier has *one* primary layout, not multiple variations.

### 2. Progressive Disclosure

Show essentials; hide details. Tier by tier:

| Content | Wide (≥120) | Medium (80–119) | Narrow (<80) |
|---------|------|--------|--------|
| Summary metrics | 4-card grid | Vertical stack | 1 card, expandable |
| List items | 8–10 rows visible | 5–6 rows | 2–3 rows |
| Sidebar | Always visible | Toggleable [S] | Hidden |
| Detail panes | Side-by-side | Stacked or tabbed | Single (full-screen) |

**Rule:** If content doesn't fit, remove it from this tier. Don't shrink font or wrap text awkwardly. Provide navigation to see more (button, tab, separate screen).

### 3. Splitter/Resizable Panels (Wide Only)

In wide layout, use splitters for user-adjustable columns:

```
Min: 18 chars (sidebar at minimum)
Max: 180 chars (prevent runaway sidebar)
Draggable: With mouse or keyboard (rare; usually mouse-only)
```

In medium/narrow, no splitters—fixed layout.

### 4. Consistent Navigation Across Tiers

Navigation patterns stay the same:
- **Keyboard:** Arrow keys, Enter, Tab (same everywhere)
- **Mouse:** Click, drag, scroll (same everywhere)
- **Hotkeys:** Global shortcuts (1–6, ?, Q) independent of layout

User can seamlessly resize terminal and keep navigating with same muscle memory.

### 5. Breakpoint Transition Rules

When viewport crosses threshold:
- **Entering narrow:** Sidebar hides. If focused on sidebar item, shift focus to main content.
- **Entering medium:** Sidebar becomes toggleable [S]. Previous state (visible/hidden) is remembered.
- **Entering wide:** Sidebar becomes visible. Restore from saved state.

**No jarring reflows.** Content reflows smoothly; active selections stay valid.

### 6. Content Truncation Rules

When content doesn't fit:

| Type | Behavior |
|------|----------|
| **List item name** | Truncate to 12 chars, append `…` |
| **Timestamps** | Use short format (HH:MM not HH:MM:SS) in narrow |
| **Long text** | Wrap to next line (don't shrink font) |
| **Badges/labels** | Use icons instead of text (e.g., 🟢 not "Active") |

### 7. Tab Headers at Different Widths

Tabs should adapt:

| Width | Tabs | Appearance |
|-------|------|-----------|
| ≥120 | 4+ tabs | Horizontal, full labels: `[Overview] [Decisions] [Activity]` |
| 80–119 | 4 tabs | Horizontal, abbreviated: `[Over...] [Deci...] [Acti...]` or icons only |
| <80 | 4 tabs | Vertical or collapsed; show current tab, arrow keys to switch |

## Examples

### Wide Layout (≥120 cols)
```
┌─────────────────────────────────────────────────────┐
│ [Nav Bar]  [Nav Bar]  [Nav Bar]                   │
├──────────┬──────────────────────────────────────────┤
│ Sidebar  │ [Tab] [Tab] [Tab] [Tab]                 │
│ 18–30ch  │ ├────────────────────────────────────────┤
│ (resiz)  │ │  Main Content Area (full width)       │
│          │ │  Lots of space for 4-column grids     │
│          │ │                                        │
├──────────┴──────────────────────────────────────────┤
│ [Bottom notification bar]                           │
└─────────────────────────────────────────────────────┘
```

### Medium Layout (80–119 cols)
```
┌─────────────────────────────┐
│ [Nav Bar Abbreviated]       │
├─────────────────────────────┤
│ [Tab] [Tab] [Tab]          │
├─────────────────────────────┤
│ Main Content (2-col stack)  │
│ • No permanent sidebar      │
│ • Tab access with [S]       │
│                             │
├─────────────────────────────┤
│ [Bottom bar]                │
└─────────────────────────────┘
```

### Narrow Layout (<80 cols)
```
┌──────────────────┐
│ [Icons Only Nav] │
├──────────────────┤
│ [Current Tab]    │
├──────────────────┤
│ Single Column    │
│ One Section      │
│ at a Time        │
│                  │
├──────────────────┤
│ [Icons] [Icons]  │
└──────────────────┘
```

### SquadTUI Example: Dashboard Overview Card Grid

**Wide (≥120):**
```
┌────────────┐  ┌────────────┐  ┌────────────┐  ┌────────────┐
│ Team       │  │ Tasks      │  │ Decisions  │  │ Velocity   │
│ 8 Members  │  │ 5 In-Prog  │  │ 2 This Wk  │  │ +42 pts    │
└────────────┘  └────────────┘  └────────────┘  └────────────┘
```

**Medium (80–119):**
```
┌────────────────────────┐
│ Team: 8 Members        │
├────────────────────────┤
│ Tasks: 5 In-Prog       │
├────────────────────────┤
│ Decisions: 2 This Wk   │
├────────────────────────┤
│ Velocity: +42 pts      │
└────────────────────────┘
```

**Narrow (<80):**
```
┌──────────────────┐
│ [Overview] ▼     │
├──────────────────┤
│ Team: 8 Members  │
│ Tasks: 5 In-Prog │
│ Decisions: 2     │
│ Velocity: +42    │
└──────────────────┘
```

## Anti-Patterns

### ❌ Don't: Shrink Font or Reduce Row Padding
If content doesn't fit, don't make it smaller. Users can't read 6pt font in a terminal. Instead, hide content or move to a separate screen/tab.

### ❌ Don't: Hard-Code Layout for One Size
Avoid "works great at 120 cols, breaks at 90." Plan for all 3 tiers from the start.

### ❌ Don't: Forget About Splitter Limits
If you let users resize sidebar freely, cap it (min 18 cols, max 180 cols). Otherwise sidebar can consume entire viewport.

### ❌ Don't: Lose Navigation Context on Resize
If user is viewing a detail panel and terminal shrinks, don't hide the back button or detail header. Keep escape route visible.

### ❌ Don't: Use Horizontal Scroll in Narrow
If content overflows horizontally in narrow layout, that's a sign the design is wrong. Reflow vertically instead.

### ❌ Don't: Show Different Data at Different Sizes
Overview tab should show same data (Team count, active count) at all sizes. Just layout changes, not content.

## Checklist

Before shipping a responsive TUI:

- [ ] Designed layouts for all 3 tiers (narrow, medium, wide)
- [ ] Tested breakpoint transitions (no data loss or lost focus)
- [ ] Keyboard nav works at all sizes (same shortcuts everywhere)
- [ ] Mouse interaction works at all sizes (click, scroll, drag)
- [ ] No horizontal scroll in narrow layout
- [ ] Tab headers adapt (abbreviated or icons at smaller sizes)
- [ ] Sidebar (if present) has min/max width limits
- [ ] Long text wraps, doesn't truncate mysteriously
- [ ] Status badges visible at all sizes (use emoji or icons)
- [ ] Documented breakpoints and layout rules in design doc

