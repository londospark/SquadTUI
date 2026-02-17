---
name: "TUI Color Language & Semantic Palette"
description: "Define and apply a consistent color system for terminal UIs using semantic meaning (primary, status, feedback) instead of arbitrary color choices."
domain: "UX design, terminal UX, visual design, color theory"
confidence: "medium"
source: "earned"
tools:
  - name: "ANSI color codes"
    description: "Terminal color system (16 standard colors + 256 extended + true color)"
    when: "When implementing colors in TUI framework (Hex1b, Blessed, etc.)"
---

## Context

Poorly chosen colors in terminal UIs:
- **Inconsistent:** Red used for both "delete" and "warning" (same action, different meaning)
- **Illegible:** Low contrast (dark text on dark background)
- **Unmapped:** Colors chosen arbitrarily without reason
- **Inaccessible:** No fallback for colorblind users

**Semantic color language** solves this by:
1. Defining a fixed palette with *meaning*
2. Applying colors by *context* (status, feedback, hierarchy)
3. Ensuring contrast and accessibility

## Patterns

### 1. Core Palette: 5 Colors (Not More)

Limit to 5 roles. Each color has a purpose:

| Role | Semantic | Usage | Hex-Like | Terminal Code |
|------|----------|-------|----------|---------------|
| **Primary** | Navigation, focus, structure | Borders, labels, active tabs | Bright cyan `#00ffff` | `36` (bright cyan) |
| **Secondary** | Subtle, muted, diminished | Timestamps, disabled text, role badges | Dim gray `#444444` | `8` (bright black) |
| **Accent** | Highlight, emphasis, active | Selected list item, current tab highlight | Bold magenta/cyan `#ff00ff` | `35` (bright magenta) |
| **Error** | Danger, failure, blocker | Error messages, blocked tasks, sync failure | Red `#ff0000` | `31` (bright red) |
| **Background** | Depth, alternation, subtle contrast | Alternating rows, panel backgrounds | Very dark gray `#1a1a1a` | `240` (dark gray) |

**Rule:** Use 4 of these consistently. A 5th (Background) is optional for depth.

### 2. Status Colors: 4 Semantics Across All Content

Map every status to one of 4 universal colors. Users learn instantly:

| Status | Color | Icon/Emoji | Meaning | Usage |
|--------|-------|-----------|---------|-------|
| **Done / Active / Success** | 🟢 Green `#00ff00` | ✓ ✅ | Good state, action complete | Active members, completed tasks, merged PRs |
| **Pending / Idle / Waiting** | 🟡 Yellow `#ffff00` | ⊘ ⏱ | On hold, not yet started | Idle members, pending tasks, draft decisions |
| **In-Progress / Working** | 🔵 Blue `#0088ff` | ◉ 🔄 | Active work happening | Working members, in-progress tasks |
| **Blocked / Error / Offline** | 🔴 Red `#ff0000` | ✗ ⚠ | Failure, stop signal | Blocked tasks, offline members, errors |

**Principle:** Never use red for good news. Never use green for a warning. The color *is* the message.

### 3. Contrast & Readability

All text must be readable:

| Foreground | Background | Contrast Ratio | Safe? |
|-----------|-----------|----------------|-------|
| White | Dark gray/black | 12:1+ | ✅ Excellent |
| Bright cyan | Dark gray/black | 8:1+ | ✅ Good |
| Bright yellow | Black | 10:1+ | ✅ Good |
| Dim gray | Black | 3:1 | ⚠ Marginal (info only, not critical) |
| Bright red | Black | 5:1+ | ✅ Acceptable |

**Never do:**
- Dark red on black (unreadable)
- Dark blue on black (unreadable)
- Any color on a color (too hard to distinguish)

### 4. Color Application by Component Type

#### Labels & Structural Text
```
Primary color (cyan) for:
  • Panel titles
  • Column headers
  • Emphasized labels ("Team:", "Status:", "Role:")
  
Secondary color (dim gray) for:
  • Filler text
  • Timestamps
  • "Role: Developer" sub-labels
```

#### Active State (Selection, Focus)
```
Accent color (bright magenta/cyan) for:
  • Selected list item background (inverted: accent bg + white text)
  • Current tab underline or background
  • Focused input field border
  • Highlighted search result
```

#### Status Badges & Indicators
```
Status colors (🟢 🟡 🔵 🔴) for:
  • Member status in roster
  • Task status badge
  • Blockers (red)
  • Activity type indicators
  
Use emoji OR colored text, depending on contrast:
  • Emoji always works (no contrast issue)
  • Colored text only if contrast ratio ≥ 4.5:1
```

#### Borders & Dividers
```
Primary color (cyan) for:
  • Panel borders
  • Section dividers
  • Tab header border
  
Accent color for:
  • Active tab border (highlight current)
  • Focused element border
```

#### Error & Warning Messages
```
Error color (red) for:
  • Error text ("Failed to sync")
  • Error badge (🔴 Blocked)
  • Error icon (✗)
  
Warning color (yellow) for:
  • Cautions ("Unsaved changes")
  • Pending state (⏱ Waiting)
  • Non-critical failures
```

#### Info & Success Messages
```
Accent or green for:
  • Success notification ("✓ Saved")
  • Info badge (ℹ New member added)
  • Confirmation checkmark
```

### 5. Colorblind Safety

Avoid relying *solely* on color to convey meaning:

**✅ Good:**
```
🟢 Active ✓
🟡 Idle ⏱
🔵 Working ◉
🔴 Blocked ✗
```
(Color + emoji + text)

**❌ Bad:**
```
[GREEN CELL]    ← What does this mean?
[RED CELL]      ← Is it bad or hot?
```
(Color only, no text/icon)

**Protanopia (red-blindness) fix:**
- Ensure primary palette doesn't rely on red/green distinction alone
- Use icons/badges alongside color
- Test with simulator: https://www.color-blindness.com/coblis-color-blindness-simulator/

### 6. Light Mode vs Dark Mode

Most TUI applications default to **dark mode** (dark background, bright text).

| Theme | Foreground | Background | Primary | Error |
|-------|-----------|-----------|---------|-------|
| **Dark** | White / bright | Black / dark gray | Bright cyan | Bright red |
| **Light** (rare) | Black / dark gray | White / light | Dark cyan | Dark red |

Define *both* even if you only ship dark mode initially. This makes future light mode trivial.

## Examples

### SquadTUI Dashboard Palette

**Core 5-color system:**
```
Primary:   Bright cyan (#00ffff)  — Borders, labels, nav
Secondary: Dim gray (#444444)     — Timestamps, disabled
Accent:    Bright magenta (#ff00ff) — Active tab, selection
Error:     Bright red (#ff0000)   — Blockers, sync fail
Background: Very dark gray (#1a1a1a) — Subtle alternation
```

**Status colors (applied everywhere):**
```
Member Anna (active):    🟢 Green + white text
Member Bob (idle):       🟡 Yellow + white text
Member Carol (working):  🔵 Blue + white text
Member Dave (blocked):   🔴 Red + white text
```

**In context:**
```
┌────────────────────┐                           Primary (cyan)
│ Dashboard          │  ← Panel title in Primary
├────────────────────┤
│ • Anna    (Lead)   │  ← Name in default, role in Secondary
│ • Bob     (Dev)    │  ← Status 🟢 as emoji badge
│ • Carol   (Design) │  ← Selected item (Carol) in Accent bg
│ • Dave    (QA)     │
└────────────────────┘

Team: 8 Members        ← Label in Primary, value in default
7 Active 🟢            ← Status emoji
1 Idle 🟡              ← Status emoji
Last synced 2m ago     ← Timestamp in Secondary
```

### Wide Layout Tab Headers

```
╔════════════════════════════════════════════════════╗
║ 📊 [Overview]  📋 [Decisions]  📝 [Activity]      ║  ← Primary (cyan) border
║                                                    ║
║ Overview tab = active, Accent-colored underline  ║
│ [Overview]  ← underlined in Accent (magenta)     ║
│ [Decisions] ← normal text                        ║
└────────────────────────────────────────────────────┘
```

### Error State Example

```
┌─────────────────────────────┐
│ Sync Failed                 │  ← Error color (red)
├─────────────────────────────┤
│ 🔴 Connection timeout       │  ← Red badge + icon
│ Retry in 30s...             │  ← Normal text
│ [Retry Now]  [Dismiss]      │
└─────────────────────────────┘
```

## Anti-Patterns

### ❌ Don't: Use More Than 5 Base Colors
Adding "tertiary," "quaternary," "success," "warning," "info" = color chaos. Stick to 5 roles + 4 status colors.

### ❌ Don't: Change Color Meaning Per Screen
If green = active on Dashboard, it must = active on Roster. Consistency is the whole point.

### ❌ Don't: Color Text Without Sufficient Contrast
If status text is dark red on dark background, users with vision issues can't read it. Use bright red or add an icon.

### ❌ Don't: Assume Color Alone Conveys Status
Always pair color with:
- **Text label** ("Active" not just green)
- **Icon/emoji** (🟢 not just green)
- **Badge** (colored background + text, not just foreground color)

### ❌ Don't: Forget Terminal Limitations
Some terminals only support 16 colors. Test your palette:
- Does it work in 16-color mode?
- Are there fallbacks for 256-color and true-color terminals?

### ❌ Don't: Use High-Saturation Colors for Everything
Bright magenta for borders + bright cyan for text + bright red for errors = visual noise. Use Primary (cyan) for structure, Accent (magenta) for highlights only.

## Palette Template (Copy-Paste Starter)

```
Color Language for [Project Name]

Core Palette:
- Primary (navigation, structure):     #00ffff (bright cyan)     ANSI 36
- Secondary (subtle, muted):          #444444 (dim gray)        ANSI 8
- Accent (highlight, active):         #ff00ff (bright magenta)  ANSI 35
- Error (danger, failure):            #ff0000 (bright red)      ANSI 31
- Background (depth, alternation):    #1a1a1a (very dark gray)  ANSI 16

Status Across All Content:
- Done / Active / Success:   🟢 #00ff00  (bright green)     ANSI 32
- Pending / Idle / Waiting:  🟡 #ffff00  (bright yellow)    ANSI 33
- In-Progress / Working:     🔵 #0088ff  (bright blue)      ANSI 34
- Blocked / Error / Offline: 🔴 #ff0000  (bright red)       ANSI 31

Application Rules:
1. Labels & titles:    Primary color
2. Active selection:   Accent background + white text
3. Status badges:      Status colors + emoji
4. Error messages:     Error color + ✗ icon
5. Borders:            Primary (active tabs in Accent)
6. Timestamps/filler:  Secondary color
7. All text:           Minimum 4.5:1 contrast ratio
```

## Checklist

- [ ] Defined core 5-color palette
- [ ] Mapped all status values (done, pending, working, blocked)
- [ ] Tested contrast ratios (all text ≥4.5:1 against background)
- [ ] Ensured same color = same meaning across all screens
- [ ] Added icons/text alongside colors (not color-only)
- [ ] Tested in colorblind simulator (protanopia, deuteranopia, tritanopia)
- [ ] Documented palette in design doc or README
- [ ] Applied consistently across all components
- [ ] Verified colors in both 16-color and 256-color terminals
- [ ] No more than 5 base colors + 4 status colors (9 total)

