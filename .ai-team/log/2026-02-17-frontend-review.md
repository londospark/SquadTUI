# 2026-02-17: Frontend Architecture Review Ceremony

**Facilitator:** Solaire (Lead)  
**Participants:** Solaire, Firekeeper (UX/Design), Siegmeyer (Frontend Dev)  
**Duration:** 60 minutes  
**Outcome:** Comprehensive widget audit, theming strategy clarification, panel background recommendations

---

## Ceremony Agenda

1. ✅ Audit all screens — which Hex1b widgets from the full widget catalog are we NOT using but SHOULD be?
2. ✅ Review theming approach — are we using ANSI escape codes where we should be using ThemePanel and the Hex1b theme system?
3. ✅ Panel backgrounds — clarify BackdropWidget doesn't exist; recommend ThemePanel / EffectPanel / Surface patterns.
4. ✅ Identify areas where we're doing things manually that Hex1b provides widgets for.
5. ✅ Assign action items to Siegmeyer (implementation) and Firekeeper (UX spec updates).

---

## Key Findings

### 1. Widget Audit Results

**Currently Used (7 widgets):**
- Layout: VStack, HStack, Responsive, Fill
- Interactive: List
- Display: Text, BarChart

**Unused HIGH PRIORITY (11 widgets):**
- TabPanel (we manually render tabs)
- Border (we use `━━━` ANSI)
- Scroll (long content gets cut off)
- Progress (manual ASCII progress bars)
- Table (roster could be multi-column)
- Splitter (manual FillWidth ratios)
- Align (manual centering)
- InfoBar (removed but could return)
- Charts (ColumnChart, BreakdownChart, TimeSeriesChart, ScatterChart)
- Tree (charter hierarchies)
- ToggleSwitch (boolean settings)

**Total unused widgets with clear use cases:** 11 out of 26 available widgets (42% adoption gap).

---

### 2. Theming Audit Results

**❌ PROBLEM: 41+ manual ANSI background escape codes**

Screens using `ThemeManager.GetPanelHeaderBg()` (returns raw ANSI):
- DashboardScreen: 5 occurrences
- RosterScreen: 4 occurrences
- DecisionsScreen: 4 occurrences
- MetricsScreen: 4 occurrences
- SkillsScreen: 5 occurrences
- ActivityLogScreen: 6 occurrences
- HelpScreen: 10 occurrences
- NoSquadScreen: 1 occurrence (reverse-video)

**Why this breaks:**
1. Bypasses Hex1b's `GlobalTheme.BackgroundColor` abstraction
2. Theme changes don't propagate dynamically
3. Fragments theme logic across 7 files
4. Maintenance burden — new themes require updating 7 files

**✅ SOLUTION: ThemePanel + Hex1b theme properties**

Replace:
```csharp
var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
detail.Text($"  {hBg}{B}{acc}Header{R}"),
```

With:
```csharp
detail.ThemePanel(tp => [
    tp.Text($"  {B}{acc}Header{R}"),
], theme => theme.Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(22, 28, 38)))
```

---

### 3. Panel Backgrounds Clarification

**User misconception:** "BackdropWidget"

**Reality:** BackdropWidget does NOT exist in Hex1b 0.87.0.

**Correct approaches:**
1. **ThemePanel** — Scoped theme mutations (BackgroundColor, ForegroundColor). Use for section backgrounds.
2. **EffectPanel** — Post-processing effects on rendered cells. Use for gradients, zebra stripes, custom effects.
3. **Surface** — Direct cell buffer access. Use for complex custom rendering.

**Recommendation:** Start with ThemePanel for 90% of cases. EffectPanel for advanced effects. Surface only when necessary.

---

### 4. Manual vs. Widget Patterns

**Manual patterns found:**
- Progress bars (ASCII `█▓░` strings) → Use `Progress` widget
- Tab rendering (manual emoji labels) → Use `TabPanel` widget
- Separators (`━━━` ANSI) → Use `Border` widget
- Centering (nested HStack/VStack) → Use `Align` widget
- URLs (plain text) → Use `Hyperlink` widget
- Multi-column lists → Use `Table` widget

**Impact:** Code duplication, inconsistent styling, missed framework benefits (theming, accessibility, rendering optimizations).

---

## Action Items

### For Siegmeyer (Implementation)

**Sprint 7 — Priority 1 (Critical):**
- [ ] Remove all manual ANSI background codes from DashboardScreen, RosterScreen, DecisionsScreen
- [ ] Replace with ThemePanel wrapping sections that need backgrounds
- [ ] Add Scroll widgets to DecisionsScreen markdown, RosterScreen charter, ActivityLogScreen detail
- [ ] Replace manual progress bars with Progress widget in DashboardScreen, MetricsScreen

**Sprint 8 — Priority 2 (High Value):**
- [ ] Replace `━━━` separators with Border widgets across all screens
- [ ] Convert AppLayout tab rendering to TabPanel widget
- [ ] Use Align widget in NoSquadScreen for idiomatic centering

**Sprint 9+ — Priority 3 (Nice to Have):**
- [ ] Add Table widget to RosterScreen for multi-column display
- [ ] Add TimeSeriesChart and BreakdownChart to MetricsScreen
- [ ] Add Hyperlink widget to NoSquadScreen URL
- [ ] Add Notifications widget for transient feedback

### For Firekeeper (UX Spec Updates)

**Sprint 7:**
- [ ] Document ThemePanel usage patterns — create style guide
- [ ] Define panel background color palette (exact RGB per theme)

**Sprint 8:**
- [ ] Chart integration spec — TimeSeriesChart and BreakdownChart layouts
- [ ] Table layout spec — roster multi-column design (columns, widths, sorting)

**Sprint 9:**
- [ ] Scrolling UX spec — scroll indicators, page-down hints
- [ ] Loading states spec — Spinner placement during async I/O

---

## Metrics

- **Screens audited:** 7 (Dashboard, Roster, Decisions, Metrics, Skills, ActivityLog, Help, NoSquad)
- **Manual ANSI codes found:** 41+ occurrences
- **Unused widgets with clear use cases:** 11
- **Widget adoption rate:** 58% (using 15 of 26 available widgets)
- **Priority 1 issues:** 4 (ANSI removal, Scroll, Progress, Border)

---

## Retrospective Notes

**What went well:**
- Comprehensive catalog of unused widgets with specific use cases
- Clear separation of manual patterns vs. framework widgets
- Concrete examples of ThemePanel usage

**What could improve:**
- Need official Hex1b 0.87.0 API docs for EffectPanel and Surface patterns
- Chart widget APIs need exploration (parameters, data formats)
- Table widget API unknown — need spike to validate feasibility

**Next ceremony:** Sprint 8 Planning (implement Priority 2 items)
