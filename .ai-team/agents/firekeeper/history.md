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
