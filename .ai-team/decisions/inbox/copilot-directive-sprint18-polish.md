# Sprint 18 — Polish, Fixes & Architecture

**Date:** 2025-01-27
**Author:** Coordinator (user directives)
**Status:** Active

## Directives

### 1. Fix Panel Sizing During Dashboard Navigation (Firekeeper + Siegmeyer)
- Dashboard panels change size when focus changes between them
- Investigate DashboardScreen.cs PanelHeader and responsive layout
- Need consistent sizing — focused vs unfocused panels must be identical widths
- May need fixed-width containers, consistent padding, or FillWidth normalization

### 2. Audit Navigation Flow for Lost Screens (Patches)
- Verify ALL screens are reachable via stack navigation
- Current Enter mapping: Panel 0→Roster, 1→ActivityLog, 2→Decisions, 3→Metrics
- **Skills screen is unreachable!** No panel maps to it and no other key navigates there
- Settings is modal-only (S key) — verify this is intentional
- Help is F1 only — verify this is adequate
- Charter is only via MemberDetail → E — verify this chain works
- Map full navigation tree and identify all gaps

### 3. Fix Broken Settings Toggles (Patches → coordinate with devs)
- Vim motions toggle: `VimBindings` flag in AppSettings toggles but BindKeys() never checks it — j/k always bound
- Mouse toggle: `MouseEnabled` flag toggles but Program.cs hardcodes `options.EnableMouse = true` — never re-read
- These toggles save to settings.json but have zero runtime effect
- Need to wire VimBindings to conditionally bind j/k/h/l keys
- Need to wire MouseEnabled to options.EnableMouse and apply on toggle

### 4. Architectural Planning from User Stories (Solaire + Firekeeper)
- Review all user stories from Sprint 17 agents
- Develop architectural proposals for next phases
- Think about what users actually want — usability first
- Consider: notification system, search, personal dashboards, kanban view

### 5. Live Update Strategy Review (Solaire → present options to user)
- Current: FileWatcher + 30s polling timer in Program.cs
- User wants to see options and make the architectural decision themselves
- Present trade-offs: polling intervals, watcher reliability, configurable refresh
- Consider: real-time vs polling UX, battery/CPU impact, configurable settings
