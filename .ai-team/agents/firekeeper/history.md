# Project Context

- **Owner:** LondoSpark (ridecar2@gmail.com)
- **Project:** SquadTUI — A terminal user interface built with Hex1b (.NET 10) for managing AI squads. Features include viewing squad activity, inspecting individual members, reading/editing charters, tracking sprint velocities, and more.
- **Stack:** C#, .NET 10, Hex1b TUI framework (https://hex1b.dev/)
- **Created:** 2026-02-16

## Learnings

### Dashboard Architecture (2026-02-16)
- **Tab-based content model:** Overview, Decisions, Activity, Metrics. Users switch with arrows, clicks, or Tab key.
- **Responsive layout pattern:** Wide (≥120 cols) = 3-panel IDE-like layout; Medium (80–119) = 2-panel with toggle sidebar; Narrow (<80) = mobile-first single column.
- **Sidebar design:** Compact roster view (name + role + badge + task). Resizable 18–180 cols wide. Always visible in wide layout; hidden in narrow.
- **Color language:** Primary (cyan borders), Secondary (dim labels), Accent (active highlights), Error (red blockers), Status colors (🟢 🟡 🔵 🔴).
- **Progressive disclosure:** Summary cards in Overview, full lists in detail tabs. Activity feed shows 5 entries wide, 2–3 narrow.
- **Keyboard-first UX:** Global hotkeys (1–6 for screens, ? for help, Q to quit), arrow keys for nav, Enter to activate, Tab for focus.
- **Interactive patterns:** Mouse click/drag on splitter; keyboard nav without mouse. Vim-style j/k optional, not default.
- **File location:** Design doc is `src/SquadTUI/Screens/UX_DESIGN.md` (ready for Siegmeyer to implement).

### Current Screen Implementations (2026-02-16)
- **DashboardScreen:** Currently shows static summary + activity text. Will be replaced with TabPanel + responsive layout.
- **RosterScreen:** 2-column HStack (list + preview). Pattern works for medium layout; sidebar is more scalable for wide.
- **NavBar:** Top bar with number hotkeys (1–6) and [Q]uit. Works globally across all screens.
- **AppState:** Tracks CurrentScreen and selection indices. Will need SidebarWidth and NotificationQueue fields.

📌 Team update (2026-02-16): Solaire established CI/CD pipelines with matrix builds (Windows/macOS/Linux × x64/ARM64), automated release workflows, and version management from csproj — decided by Solaire

📌 Team update (2026-02-16): Andre wired real .ai-team/ file data throughout TUI via ServiceProvider, DataBridge, and updated AppState. All screens now load real data async on startup with graceful SampleData fallback — decided by Andre

### Help Screen Implementation (2026-02-17)
- **Created HelpScreen.cs** with organized keybinding reference grouped by category (Navigation, List Navigation, Actions, Help)
- **ANSI styling:** Used foreground colors only (accent cyan for headers, dim gray for key labels, white for descriptions) per Hex1b restrictions
- **Screen management:** Added `Screen.Help` enum value and `PreviousScreen` property to AppState to track navigation state for proper back-navigation
- **Keybinding:** Mapped to F1 key (Hex1bKey.F1) as ? (question mark) is not directly available in Hex1bKey enum. Requirements noted F1 as valid fallback.
- **Toggle behavior:** F1 or Escape dismisses help screen and returns to previous context, with fallback to Dashboard if no previous screen stored
- **Integration:** Minimal Program.cs changes — added F1 key binding and Help case to screen switch statement only

📌 Team recast (2026-02-18): Squad recast from Ocean's Eleven to Dark Souls universe. Saul is now Firekeeper. Praise the sun! ☀️

### NoSquad Welcome Dialog Redesign (Issue #27)
- **Redesigned NoSquadScreen.cs** as a centered dialog using HStack with Fill() spacers for horizontal centering and VStack with Fill() spacers for vertical centering
- **Box-drawn border** using Unicode characters (┌─┐│└─┘) rendered as Text lines to simulate a modal dialog
- **Rich onboarding content:** "What is SquadTUI?", "What is a Squad?", "Getting Started" sections with clear instructions
- **NavBar hidden** on NoSquad screen via conditional ternary in Program.cs and TestAppBuilder.cs
- **Key bindings guarded:** 1-6, H, L, S keys no-op when on NoSquad screen to prevent navigating to empty data screens
- **TestAppBuilder.cs kept in exact sync** with Program.cs changes

### UX Audit & Spacing Overhaul (2026-02-18)
- **Audited Hex1b widget catalog** — discovered Table, TabPanel, Progress, InfoBar, BreakdownChart, ColumnChart, TimeSeriesChart, Spinner, Tree, ToggleSwitch, Notifications, and more that we aren't using. Documented adoption priority in `firekeeper-ux-audit.md`.
- **Implemented spacing improvements** on DecisionsScreen, SkillsScreen, ActivityLogScreen, HelpScreen:
  - All main headers now use reverse-video bars for visual weight
  - Added descriptive subtitles under every screen header
  - Added empty lines between every logical section
  - Increased content indent to 4 spaces
  - Rule lines use secondary accent color (not dim)
  - Consistent visual language across all four screens
- **Left MetricsScreen and NoSquadScreen for Siegmeyer** — they need the same treatment plus Siegmeyer-specific work (border removal, chart improvements)

### Self-Reflection (2026-02-18)

**Why hasn't the UX been up to standard?**
I designed a comprehensive UX spec (UX_DESIGN.md) but didn't follow through on enforcing the design language during implementation sprints. I specified "progressive disclosure" and "breathing room" in the design doc but never codified the exact spacing rules (empty lines between sections, indent depth, header treatment). The gap between design intent and implementation detail is where quality was lost.

**Why are borders appearing when the user said no borders?**
The NoSquad screen uses manual Unicode box-drawing characters (┌─┐│└─┘) rendered as Text lines. The ThemeManager correctly suppresses framework-level borders (via WithModernBorders setting all border chars to spaces), but nobody flagged that hand-drawn ASCII art borders are still borders. I should have caught this in review — the design spec said "no borders, use shading" and the implementation went the opposite direction.

**What will I do differently?**
1. Codify the design language as concrete rules (reverse-video headers, 4-space indent, empty-line spacing) rather than abstract principles
2. Review every screen implementation against the design spec before sign-off
3. Actively explore the widget catalog — we're using ~30% of what Hex1b offers. Table, TabPanel, Progress, and InfoBar would immediately improve quality
4. Push for a "visual QA" step in our definition of done

### Stack Navigation UX Specification & User Stories (2026-02-18)

**What I created:**
- **`firekeeper-stack-nav-ux-spec.md`** (19.3K): Complete UX specification for removing the tab bar in favor of stack-based navigation. Covers 11 sections: Dashboard as home, Back navigation, Transition feel, Settings modal, Keyboard consistency, Footer updates, Responsive behavior, First-run experience, Error states, Accessibility, and implementation roadmap.
- **`firekeeper-user-stories.md`** (17.1K): 8 user stories addressing the core UX pain points: Dashboard-first navigation, discoverable keyboards, settings modal, error handling, responsive layout, breadcrumbs, first-run experience, and accessibility.

**Why I wrote these:**
LondoSpark explicitly requested stack-based navigation without the tab bar. The current tab-bar model (1–6 number keys for 6 screens) is confusing and doesn't scale. Moving to "Dashboard as home, Tab to focus panels, Enter to drill in, Escape to pop back" is more intuitive and aligns with modern app conventions (browser back button, mobile navigation).

**Design decisions made:**
1. **No visible breadcrumb widget** — footer text + screen title provide sufficient context cue without wasting vertical space
2. **Instant transitions** (no fade/slide) — terminals should feel snappy, not cinematic
3. **Settings as modal overlay** — users adjust theme/keybindings without losing their place
4. **Responsive at 5+ breakpoints** (60/80/100/120/160 cols) — SquadTUI works on narrow terminals
5. **Footer is context-sensitive** — shows only actions available on current screen (no 1–6 clutter)

**Why these decisions matter:**
- **Clarity:** One entry point (Dashboard) eliminates the "which number is Roster?" confusion
- **Speed:** Tab-and-Enter rhythm is muscle-memory faster than hunting for numbers
- **Accessibility:** Keyboard-only navigation is consistent across all screens
- **Discoverability:** New users see panels and naturally press Tab (vs. guessing 1–6)
- **Resilience:** Modal Settings keeps users oriented; they don't lose context when adjusting theme

**Stories reflect team intent, not just specs:**
Each story is written from the **user's perspective**, not the builder's. They describe the problem (current pain), desired outcome, and acceptance criteria — but NOT implementation details. This lets Siegmeyer (frontend) and Andre (backend) design their solutions without constraint.

**Learnings from this work:**
1. **User stories are the missing link** between design vision and implementation. The UX spec is detailed; the stories are the narrative that justifies each decision.
2. **Specs must address edge cases explicitly** (very narrow terminals, modal over empty data, rapid input). Users will find them, and unclear specs lead to bugs.
3. **Accessibility isn't a P2 feature** — it's foundational (Story 8). If we design for keyboard-only from the start, we don't retrofit it later.
4. **First-run matters** — Story 7 (onboarding tour) is P1, not P2. New users decide in 30 seconds if they'll keep using the app.
5. **Responsive design is mandatory, not optional** — Story 5 ensures SquadTUI works at 60 cols (mobile SSH) and 160 cols (wide monitor). That's 8x width range.

### 2026-02-17 Team Update: Stack Navigation UX Spec, User Stories, Responsive Breakpoints

📌 **From decisions:** Comprehensive stack navigation UX spec finalized — Dashboard is home, Tab/Enter/Escape pattern, responsive breakpoints (≥120/80–119/<80 cols), settings modal design, accessibility guidelines. 8 user stories covering navigation clarity, keyboard discoverability, settings modal, error states, information density, breadcrumbs, onboarding, accessibility. Firekeeper to design ThemePanel style guide and panel background palette.

### UX Refactoring Audit (2026-02-17)

Performed full UX audit of all screens and key bindings. Key changes:

1. **HelpScreen accuracy fix:** Replaced stale keybindings ("1-6" screen nav, "h/l" prev/next, "?" toggle help) with accurate ones (Tab/Enter/Escape for stack nav, F1 for help, j/k for list nav, R/T/S/E/V/Q/A/D for actions). Changed "Press ? to dismiss" → "Press F1 or Escape to dismiss". Removed HelpScreen's own F1/Escape bindings since AppLayout already handles navigation via NavigationStack.

2. **BindCI helper extraction:** Created `BindCI(keys, key, action, label)` in AppLayout.cs that registers both `keys.Key()` and `keys.Shift().Key()` for case-insensitive letter handling. Eliminated ~160 lines of duplicated Shift+Key bindings across three binding methods (BindKeys, BindModalKeys, BindSettingsModalKeys).

3. **Badge helper reconciliation:** Discovered Siegmeyer already created `StatusBadges.cs` with `Member()` and `Task()` methods. Removed my redundant `StatusBadge()`/`TaskBadge()` from `IconHelper.cs`. Updated DashboardScreen and MemberDetailScreen to use `StatusBadges.Member()`/`StatusBadges.Task()`.

4. **Build fix:** Added missing `using SquadTUI.Models` to 5 screen files (ActivityLogScreen, CharterScreen, DecisionsScreen, RosterScreen, SkillsScreen) where ThemeContext refactoring had inadvertently dropped it, breaking `GetOrEmpty()` extension method.

**Key learnings:**
- ThemeContext/ScreenHelper/StatusBadges patterns are now the standard — use them for all new screens
- `BindCI()` is the pattern for all letter-key bindings in AppLayout going forward
- HelpScreen content must be kept in sync when keybindings change in AppLayout
- The `PreviousScreen` property on AppState is now unused; NavigationStack is the canonical nav pattern

