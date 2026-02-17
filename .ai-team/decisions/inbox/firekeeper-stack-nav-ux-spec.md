# Stack Navigation UX Specification — SquadTUI Dashboard Redesign

**Date:** 2026-02-18  
**By:** Firekeeper (UX/Design Lead)  
**Status:** Specification  
**Requested by:** LondoSpark

---

## Executive Summary

This specification defines the interaction model for stack-based navigation in SquadTUI, replacing the tab bar with a modal-stack architecture. The Dashboard becomes the home screen. Users navigate by focusing/entering dashboard panels (Tab/Shift+Tab, Enter), drilling into full-screen views, and returning via Escape (pop). Settings becomes a centered modal overlay, not a full-width screen. All navigation is discoverable through keyboard shortcuts and visual feedback.

---

## 1. Dashboard as Home

### Current State (Tab-based)
- Dashboard is one of 6 screens accessible via 1–6 number keys
- 3 dashboard content tabs (Overview, Decisions, Activity, Metrics) navigable with arrows
- Panels are embedded but not independently focusable

### New Model (Stack-based)
- **Dashboard is the entry point** — app starts here; all other screens are drilled-in views
- **3–4 focusable panels** visible in wide layout (Team Roster, Activity & Progress, Decisions, Sprint Metrics)
- **Panels are always discoverable** — users can Tab to any panel without hunting through tabs

### Panel Navigation Interaction

| Key | Action |
|-----|--------|
| `Tab` | Focus next panel (left → center → right → left, cycling) |
| `Shift+Tab` | Focus previous panel |
| `Enter` | Drill into focused panel → full-screen view |
| `Escape` | Return to Dashboard (if in drilled-in view) |

### Visual Feedback — Focused Panel Indicator

**Focused panel (before Enter):**
- Border or background tint using theme's **Accent color** (currently bright cyan)
- Subtle animation: glow or highlight pulse (if feasible without blocking input)
- Footer guidance: "Press **Enter** to drill in, **Tab** to navigate, **Escape** to exit"

**Non-focused panels:**
- Standard background (no border glow)
- Slightly dimmed or neutral tone to recede visually

**Example in Terminal:**
```
┌─────────────────┐         ┌──────────────┐          ┌────────────────┐
│ 🏠 Roster       │  [TAB]  │ 📊 Activity  │  [TAB]   │ 📋 Decisions   │
│ ───────────────│◄──────▶│ ────────────│◄─────▶ │ ──────────────│
│ 5 members       │         │ 12 updates   │         │ 8 open items   │
└─────────────────┘         └──────────────┘          └────────────────┘
                            ▲ Focused (bright border)

Footer: [Enter] Drill in · [Tab] Navigate · [Esc] Exit
```

### Responsive Behavior

| Terminal Width | Visible Panels | Tab Cycles Through |
|---|---|---|
| ≥120 cols (wide) | 4 panels | Roster → Activity → Decisions → Metrics |
| 80–119 cols (medium) | 2–3 panels | Roster → Activity → Decisions |
| <80 cols (narrow) | 1 panel | Roster only (stack panels vertically) |

---

## 2. Back Navigation

### Escape as Universal Pop

- **On Dashboard:** Escape is a no-op (user is already home)
- **On drilled-in screen (Roster, Decisions, Activity, Metrics):** Escape pops back to Dashboard
- **On any nested modal (future-proofing):** Escape pops to previous layer

### Visual "Breadcrumb" — Transparent Not Literal

Do **not** render a literal breadcrumb bar (wastes vertical space). Instead:

1. **Footer guidance changes context-sensitively:**
   - Dashboard footer: `[Tab] Navigate · [Enter] Drill in · [Q] Quit`
   - Roster footer: `[Esc] Back to Dashboard · [J/K] Scroll · [Q] Quit`
   - Decisions footer: `[Esc] Back to Dashboard · [↑↓] Navigate · [Q] Quit`

2. **Screen title includes origin cue** (optional, but recommended):
   - Roster title: `← 👥 Roster` (left arrow as "back" cue)
   - Decisions title: `← 📋 Decisions`
   - Or use subtitle: "Viewing from Dashboard"

3. **No separate breadcrumb widget** — let screen title + footer carry the UX signal

### Alternative: Ghost Breadcrumb (Advanced)

If visual depth matters, a **very subtle breadcrumb** at the top (1 line):
```
🏠 Dashboard > 👥 Roster
```
- Rendered in **secondary accent color** (dim gray, not bright)
- Takes only 1 line (text-only, no interactive clicking)
- Serves as "you are here" label, not navigation
- Can be toggled on/off via Settings if it feels noisy

**Recommendation:** Start without breadcrumb; add it only if users report confusion about their location.

---

## 3. Transition Feel — Instant vs. Fade

### Philosophy

SquadTUI is a **terminal UI**, not a web app. Terminal apps prioritize **speed** and **responsiveness** over cinematic transitions. Users expect instant feedback.

### Implementation

**Default: Instant screen swap** (no fade/slide)
- Drill into panel: Clear dashboard, render full screen immediately
- Pop back: Clear screen, render dashboard immediately
- Rationale: Feels snappy, reduces input lag perception, respects terminal traditions

**Future Enhancement: Optional "Depth" Cue** (if Hex1b supports it)
- When drilling in: Slightly offset new screen (right or lower) for 1 frame
- When popping back: Offset clears away
- Creates visual sense of "layering" without blocking interaction

### No Animations That Block Input

- User presses Escape on Roster screen
- Dashboard must render and respond to next key press instantly
- No 300ms fade animation that prevents the user from pressing Tab immediately

---

## 4. Settings Modal

### Modal Positioning

**Center of terminal screen** — Settings floats above Dashboard or any drilled-in screen.

### Size & Layout

| Terminal Width | Modal Width | Modal Height | Rules |
|---|---|---|---|
| ≥100 cols | 50–60 cols | 20–24 rows | Centered with 15–25 col margin left/right |
| 80–99 cols | 40–50 cols | 16–20 rows | Centered, adaptive to content |
| <80 cols | 100% width - 4 cols | 80% height | Full width with 2-col padding (mobile mode) |

### Visual Treatment

1. **Modal frame:**
   - Use Unicode box-drawing or modern box chars (┌─┐│└─┘ or ╔═╗║╚╝)
   - Border uses **theme's Accent color** (bright, to indicate focus)
   - No shadow (terminal limitation) — relies on center positioning + border brightness

2. **Background dimming:**
   - **Everything behind the modal dims** (use theme's Secondary background shade, ~40% opacity)
   - Or use ANSI dim filter if available (`SGR(2)` — browser-dependent)
   - **Do not black out** — user should still see Dashboard/Roster behind modal

3. **Modal content:**
   - Header: "⚙️ Settings" (reverse-video bar, 1 line)
   - Body: 4-space indent for option groups (Theme, Keybindings, Accessibility)
   - Footer: `[↑↓] Navigate · [Enter] Select · [Esc] Close`

### Interaction Model

| Key | Action |
|-----|--------|
| `↑` / `↓` or `J` / `K` | Scroll through options |
| `Enter` | Activate selected option (toggle, show dropdown, etc.) |
| `Escape` | Close modal, return to previous screen (Dashboard or drill-in) |
| `Tab` | Move to next option group (optional, if many options) |

### Example Layout (50-col wide modal)

```
                 ┌────────────────────────────────────┐
                 │ ⚙️ Settings                        │
                 ├────────────────────────────────────┤
                 │                                    │
                 │ 🎨 Theme                          │
                 │    [●] Ocean   [○] Heist          │
                 │    [○] Sunset                      │
                 │                                    │
                 │ ⌨️  Keybindings                    │
                 │    [○] Default  [●] Vim            │
                 │                                    │
                 │ ♿ Accessibility                   │
                 │    [●] Emoji enabled               │
                 │    [○] Dim borders                 │
                 │                                    │
                 │ [Esc] Close · [↓] Next · [Return] │
                 └────────────────────────────────────┘
```

---

## 5. Keyboard Consistency Without Tab Bar

### Current Tab Bar (Number Keys)

Today, users press `1–6` to navigate to different screens (Dashboard, Roster, Decisions, etc.). With stack-based nav, this model breaks — **there's only Dashboard at the top level**.

### New Model: Entry Points Disappear, Navigation is Contextual

- **1–6 keys:** Reserved for future use or removed entirely
  - Option A: Keep them as shortcuts to drill directly into panels (e.g., `1` = open Roster)
  - Option B: Remove them, rely on Tab/Enter from Dashboard

**Recommendation: Option B (cleaner)**
- Users always land on Dashboard
- Users Tab to desired panel, press Enter to drill in
- Simpler mental model, fewer conflicts with future features

### Keybinding Map (Comprehensive)

#### Dashboard

| Key | Action | Discovery |
|-----|--------|-----------|
| `Tab` / `Shift+Tab` | Cycle panel focus | Visual focus ring |
| `Enter` | Drill into focused panel | Footer hint |
| `Q` | Quit | Help screen (F1) |
| `T` | Toggle theme | Help screen |
| `S` | Open Settings | Help screen |
| `F1` / `?` | Show Help | Common convention |

#### Any Drilled-In Screen (Roster, Decisions, Activity, Metrics)

| Key | Action | Discovery |
|-----|--------|-----------|
| `Escape` | Pop back to Dashboard | Footer hint |
| `↑` / `↓` or `J` / `K` | Scroll/select | Help screen |
| `Enter` | Open item detail (if applicable) | Contextual |
| `Q` | Quit | Help screen |
| `T` | Toggle theme | Help screen |
| `S` | Open Settings | Help screen |
| `F1` | Show Help | Common convention |

#### Settings Modal (Anywhere)

| Key | Action |
|-----|--------|
| `↑` / `↓` | Navigate options |
| `Enter` | Select/toggle |
| `Escape` | Close modal, return to previous context |

### Consistency Principles

1. **Escape always means "back"** (pop stack or dismiss modal)
2. **Enter always means "activate"** (drill in, select, toggle)
3. **Arrow keys + J/K always mean "navigate"** (consistent across all screens)
4. **Q always means quit** (global, non-negotiable)
5. **Help is always F1 or ?** (standardized, discoverable from any screen)

---

## 6. Footer Updates

### Current Footer (Tab-Bar Era)

```
[1] Dashboard · [2] Roster · [3] Decisions · [4] Skills · [5] Activity · [6] Metrics · [?] Help · [Q] Quit
```
- **Problem:** Cluttered, 60+ characters, wastes space, redundant (numbers don't mean anything without context)

### New Footer (Stack-Navigation Era)

**Dashboard:**
```
[Tab] Navigate · [Enter] Drill in · [S] Settings · [F1] Help · [Q] Quit
```
- 50 characters
- Shows only actions available from Dashboard
- Guides user toward core interaction (Tab → Enter)

**Roster (drilled-in):**
```
[↑↓] Navigate · [Enter] View · [Esc] Back · [S] Settings · [F1] Help · [Q] Quit
```
- Explains "Esc" pops back
- Contextual [Enter] label (e.g., "View" for roster items)

**Settings Modal:**
```
[↑↓] Navigate · [Enter] Select · [Esc] Close
```
- Minimal, focused on modal interaction
- No global actions (Q, F1) — user must close modal first

### Footer Design Rules

1. **Max 70 characters** (fits 80-col terminal with margin)
2. **Lead with most common action** (Tab on Dashboard, ↑↓ on detail screens)
3. **Always include Escape cue** when applicable ("Back", "Close")
4. **Hide irrelevant actions** (no [1–6] screen nav, no theme toggle hints on every screen)
5. **Abbreviate aggressively** (`[↑↓]` instead of `[Up/Down Arrow]`, `[Esc]` not `[Escape]`)

---

## 7. Responsive Behavior Summary

### Wide Layout (≥120 cols)

```
┌────────────────────────────────────────────────────────────────────┐
│ 🏠 Dashboard                                                      │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│ ┌─────────────┐    ┌──────────────┐    ┌────────────────┐        │
│ │ 🏠 Roster   │    │ 📊 Activity  │    │ 📋 Decisions   │        │
│ │ [TAB FOCUS] │    │              │    │                │        │
│ └─────────────┘    └──────────────┘    └────────────────┘        │
│                                                                    │
│ [Tab] Navigate · [Enter] Drill in · [S] Settings · [F1] Help · [Q] │
└────────────────────────────────────────────────────────────────────┘
```

**Tab cycles:** Roster → Activity → Decisions → Metrics (if visible) → Roster

---

### Medium Layout (80–119 cols)

```
┌──────────────────────────────────────────┐
│ 🏠 Dashboard                            │
├──────────────────────────────────────────┤
│ ┌──────────────┐   ┌────────────────┐   │
│ │ 🏠 Roster    │   │ 📊 Activity    │   │
│ │ [TAB FOCUS]  │   │                │   │
│ └──────────────┘   └────────────────┘   │
│                                          │
│ ┌─────────────────────────────────────┐  │
│ │ 📋 Decisions                        │  │
│ │                                     │  │
│ └─────────────────────────────────────┘  │
│ [Tab] Navigate · [Enter] Drill in · [S] │
└──────────────────────────────────────────┘
```

**Tab cycles:** Roster → Activity → Decisions

---

### Narrow Layout (<80 cols)

```
┌────────────────┐
│ 🏠 Dashboard  │
├────────────────┤
│ 🏠 Roster      │
│ [TAB FOCUS]    │
│ ───────────── │
│ 5 members      │
│                │
│ 📊 Activity    │
│ (scroll down)  │
│                │
│ [Tab] Scroll · │
│ [Enter] Drill  │
└────────────────┘
```

**Tab cycles:** Roster → Activity → Decisions (vertical scrolling)

---

## 8. First-Run Experience

### Onboarding Sequence

1. **App launches** → Dashboard shown
2. **First time only:** Animated breadcrumb tour shows what each panel is
   - Roster: "Manage squad members"
   - Activity: "See recent updates"
   - Decisions: "Track team decisions"
   - Metrics: "View sprint health"
3. **Footer guides:** `[Tab] Navigate · [Enter] Drill in`
4. **User presses Tab** → Focus highlights move across panels
5. **User presses Enter** → Roster opens full-screen
6. **User presses Escape** → Back to Dashboard
7. **Tutorial complete** — user never sees tour again

### Persistent Guidance

- **Help always available** (F1 or ?)
- **Footer always shows contextual hints** (no assumption of knowledge)
- **Panel headers are self-documenting** (emoji + descriptive text)

---

## 9. Error States & Edge Cases

### Settings Modal Over Empty Data

**Scenario:** User opens Settings while no squad is loaded (NoSquad screen).

**Behavior:**
- Settings modal still appears, centered
- Background dims (but is mostly blank)
- User can adjust theme, keybindings, accessibility
- Escape closes modal, returns to NoSquad welcome

### Modal During Transitions

**Scenario:** User presses Escape to return to Dashboard while Settings is open.

**Behavior:**
- First press closes Settings modal (no double-pop)
- Second press in Dashboard is a no-op (already home)

### Very Narrow Terminals (<60 cols)

**Scenario:** Terminal is only 50 cols wide.

**Behavior:**
- Settings modal takes 100% width - 4 col padding (46 cols usable)
- Footer text wraps or abbreviates (`[↑] [↓] [Ent] [Esc]` instead of full names)
- Content is still legible (min viable width is 50 cols)

---

## 10. Accessibility Considerations

### Keyboard-Only Navigation

✅ All navigation functions work without mouse (already true in terminal UI)

### Screen Reader Compatibility

- **Modal presence:** Announce "Settings modal opened" on Escape if possible
- **Panel focus:** Announce "Focus moved to [Panel Name]" on Tab
- **Footer text:** Read aloud for screen reader users

### High Contrast

- **Modal border:** Use Accent color (bright, already high-contrast)
- **Focus rings:** Use bright colors, not subtle glow (for visibility)
- **Dim backgrounds:** Ensure sufficient contrast vs. text (WCAG AA minimum)

### Terminal Compatibility

- **Fallbacks:** All features work in monochrome terminals
- **Emoji optional:** Render without emoji if terminal doesn't support (graceful degradation)

---

## 11. Implementation Roadmap

### Phase 1: Foundation (Week 1)
1. Remove tab bar UI
2. Implement Dashboard focus state (Tab/Shift+Tab highlight focused panel)
3. Implement Enter → drill-in (navigate to full-screen view)
4. Implement Escape → back (pop from full-screen to Dashboard)

### Phase 2: Polish (Week 2)
1. Update footer guidance text per section 6
2. Add breadcrumb or "back" cue in screen titles
3. Implement Settings modal (centered, dim background)
4. Add help/onboarding tour (first-run only)

### Phase 3: Refinement (Week 3)
1. Responsive layout testing (60/80/100/120/160 col widths)
2. Keyboard consistency audit
3. Accessibility review (screen reader, keyboard-only)
4. Edge case testing (empty data, narrow terminals, rapid input)

---

## Summary of Changes

| Element | Before | After |
|---------|--------|-------|
| **Entry point** | 6 screens accessible via 1–6 | Dashboard only (home) |
| **Navigation** | Tab bar with numbers | Tab/Shift+Tab to focus panels, Enter to drill |
| **Back button** | Not prominent | Escape (prominent in footer) |
| **Settings** | Full-width screen | Centered modal overlay |
| **Breadcrumb** | None | Subtle (footer + title only, no widget) |
| **Footer** | Number hotkeys (1–6) | Contextual action hints |
| **Transitions** | Tab press (no animation) | Instant swap (no fade) |
| **Keyboard consistency** | 1–6 for screens, arrows for tabs | Tab for panels, Enter to drill, Escape to pop |

---

## Design Rationale

**Why stack-based?**
- Familiar mental model (browser back button, mobile navigation)
- Reduces cognitive load (always know how to exit — press Escape)
- Scales better (can add nested views in future without clutter)

**Why modal Settings?**
- Doesn't interrupt workflow (overlay, not full-screen)
- Easier to toggle on/off without losing context
- Follows web/mobile convention (users already know how to use modals)

**Why no breadcrumb widget?**
- Wastes 1–2 rows of screen real estate (vertical space is premium in terminals)
- Footer + screen title provide sufficient context
- Can add if user feedback demands it (not permanent loss)

**Why instant transitions?**
- Terminal UIs expect responsiveness
- Animations can create input lag or jank in some terminals
- Speed signals efficiency and confidence

---

## Questions for Team Feedback

1. **Settings modal size:** Is the 50–60 col width comfortable, or should it be wider (70 cols)?
2. **Breadcrumb:** Should we add the subtle `🏠 Dashboard > 👥 Roster` breadcrumb, or is footer guidance enough?
3. **Panel names:** Are "Team Roster", "Activity & Progress", "Decisions", "Sprint Metrics" clear enough, or do they need emoji refinement?
4. **Keyboard alternatives:** Should J/K (Vim style) be the default, or stay arrows-only?
5. **Settings history:** Should the modal remember last-opened tab, or always reset to "Theme"?

---

**Status:** Ready for team review and implementation by Siegmeyer (frontend) and Andre (settings backend).
