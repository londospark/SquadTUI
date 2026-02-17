# Decision: Dashboard Panel Navigation with Tab/Enter/Escape

**Date:** 2026-02-17
**By:** Solaire (Architecture Review Ceremony)
**Participants:** Firekeeper, Andre

## Context

LondoSpark wants Tab key to navigate between dashboard panels for drill-in capability. Currently the Dashboard has 3 panels (Team Roster, Activity & Progress, Decisions) in the wide layout but no way to focus or drill into any of them. The TabPanel widget handles top-level screen navigation; this is about intra-screen panel focus.

## Decision

**Implement panel focus cycling with Tab, drill-in with Enter, back-out with Escape.**

### Interaction Model (Firekeeper's UX Design)

| Key | Action |
|-----|--------|
| `Tab` | Cycle focus to next panel (left → center → right → left) |
| `Shift+Tab` | Cycle focus to previous panel |
| `Enter` | Drill into focused panel (navigate to that screen's full view) |
| `Escape` | If in drilled-in view, return to dashboard. If on dashboard, no-op |
| `1-6` | Still works for global screen navigation (unchanged) |

### Panel Focus Mapping

| Panel | Drill-in Target |
|-------|----------------|
| Team Roster | → `Screen.Roster` |
| Activity & Progress | → `Screen.ActivityLog` |
| Decisions | → `Screen.Decisions` |
| Sprint Metrics | → `Screen.Metrics` |

### AppState Changes

```csharp
public int DashboardFocusedPanel { get; set; } = 0; // 0=Roster, 1=Activity, 2=Decisions, 3=Metrics
```

### Visual Indicator

Focused panel gets a brighter border or subtle glow effect using the theme's accent color. Non-focused panels use the standard panel background. Firekeeper to design the exact visual treatment, but the principle is: **one panel is visually distinct at all times** when Tab has been used.

### Implementation Notes

- Tab key binding only activates when `state.CurrentScreen == Screen.Dashboard`
- Panel focus state resets to 0 when returning to Dashboard from another screen
- Responsive layouts (medium/narrow) adapt panel count — focus cycles through visible panels only
- The BackgroundPanelWidget already exists; extend it to accept a `focused` boolean that changes the background shade

## Consequences

- **Firekeeper:** Design focused/unfocused panel visual states
- **Siegmeyer:** Implement Tab/Shift+Tab/Enter bindings in `AppLayout.BindKeys()`, update `DashboardScreen` to pass focus state to `BackgroundPanelWidget`
- **Andre:** No backend changes needed

## Risks

- Tab key may conflict with Hex1b's TabPanel widget navigation — test that Tab on Dashboard doesn't also switch top-level tabs. May need to intercept Tab in Dashboard context before it reaches TabPanel.
