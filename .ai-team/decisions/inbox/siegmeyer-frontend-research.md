# Siegmeyer — Frontend Research: Dashboard Nav, Modal Themes, Charts, New Themes, User Stories

**By:** Siegmeyer (Frontend Dev)  
**Date:** 2026-02-18  
**Requested by:** LondoSpark  
**Status:** Research & Plan — not implemented yet

---

## 1. BackdropWidget Modal for Theme Selection

### Research Findings

Hex1b provides **two** viable approaches for a modal theme selector:

#### Option A: BackdropWidget + ZStack (Simpler)

`BackdropWidget` (Hex1b.Widgets) fills its entire bounds and intercepts all input. Combined with a `ZStack`, it creates a proper modal overlay. The child widget is **auto-centered** within the backdrop.

```csharp
// In AppLayout.Build(), wrap the main content in a ZStack when modal is open:
if (state.ShowThemeModal)
{
    return ctx.ZStack(z => [
        // Layer 0: Normal app content (non-interactive while backdrop is up)
        z.VStack(v => [ /* existing TabPanel content */ ]).Fill(),

        // Layer 1: Semi-transparent backdrop + centered theme picker
        z.Backdrop(
            z.VStack(modal => [
                modal.Text($"  {B}{acc}🎨 Theme Selection{R}"),
                modal.Text($"  {sec}━━━━━━━━━━━━━━━━━━━━━━━━━━━━{R}"),
                modal.Text(""),
                modal.List(ThemeManager.ThemeNames.ToList() as IReadOnlyList<string>)
                    .OnSelectionChanged(e =>
                    {
                        // Live preview: apply theme immediately
                        state.SelectedThemeIndex = e.SelectedIndex;
                        options.Theme = ThemeManager.GetTheme(e.SelectedIndex);
                    })
                    .OnItemActivated(e =>
                    {
                        // Confirm selection, close modal
                        state.ShowThemeModal = false;
                    })
                    .Fill(),
                modal.Text(""),
                modal.Text($"  {D}↑↓ Navigate  Enter Confirm  Esc Cancel{R}"),
            ]).FixedWidth(40).FixedHeight(16)
        )
        .Opaque(Hex1bColor.FromRgb(5, 5, 10))  // Dark semi-opaque overlay
        .OnClickAway(() => { state.ShowThemeModal = false; })
    ]);
}
```

**Key behaviors:**
- `BackdropNode` always returns `InputResult.Handled` — prevents click-through to content beneath
- Escape triggers `OnClickAway` handler automatically
- Child is auto-centered (no manual positioning needed)
- `.Opaque(color)` fills entire terminal with a dark wash — creates the "dimmed background" effect

#### Option B: WindowPanel Modal (Richer)

Hex1b's `WindowPanel` + `WindowManager` provides full modal dialog semantics with title bar, close button, and result handling:

```csharp
// Trigger from keybinding or settings screen:
e.Windows.Window(w => w.VStack(v => [
    v.Text("Select a theme:"),
    v.List(ThemeManager.ThemeNames.ToList() as IReadOnlyList<string>)
        .OnItemActivated(e2 =>
        {
            w.Window.CloseWithResult(e2.ActivatedIndex);
        }),
]))
.Title("🎨 Theme")
.Size(40, 14)
.Modal()
.Position(WindowPositionSpec.Center)
.OnResult<int>(result =>
{
    if (!result.IsCancelled)
    {
        state.SelectedThemeIndex = result.Value;
        options.Theme = ThemeManager.GetTheme(result.Value);
    }
})
.Open(e.Windows);
```

**Tradeoff:** WindowPanel gives us title bar, drag, resize, and result handling for free — but requires wrapping our root layout in a `WindowPanel`. BackdropWidget is simpler for our use case.

### Recommendation

**Use BackdropWidget (Option A).** It's simpler, works within our existing VStack/HStack layout model, and the auto-centering + input blocking matches exactly what LondoSpark described. Live preview is trivial: just mutate `options.Theme` in the `OnSelectionChanged` callback.

### AppState Changes Needed

```csharp
// Add to AppState:
public bool ShowThemeModal { get; set; } = false;
```

### Key Binding

```csharp
// In AppLayout.BindKeys — trigger modal on Shift+T or from Settings screen:
keys.Key(Hex1bKey.T).WithShift().Action(() =>
{
    state.ShowThemeModal = !state.ShowThemeModal;
}, "Theme Picker Modal");
```

---

## 2. Tab Navigation Between Dashboard Panels

### Current State

The Dashboard has 3 visual panels in wide layout (Roster, Activity, Decisions/Metrics) but **no focus management between them**. All panels are static `Text()` widgets — no interactive elements within them. The top-level `TabPanel` already uses Tab to switch between main screens.

### Problem

LondoSpark wants Tab to cycle focus **within** the Dashboard's sub-panels (roster, tasks, activity, decisions, metrics), Enter to drill into the focused panel, and Escape to back out.

### Approach: State-Driven Focus Ring

Since our Dashboard panels are built from `Text()` widgets (not focusable), we can't rely on Hex1b's native Tab focus. Instead, implement a **state-driven focus ring**:

```csharp
// Add to AppState:
public int DashboardFocusedPanel { get; set; } = 0;
public static readonly string[] DashboardPanels = ["Roster", "Activity", "Decisions", "Metrics"];
```

#### Visual Focus Indicator

Wrap each panel's header in a highlight when focused:

```csharp
// In DashboardScreen.Render():
var panels = new[] { "Roster", "Activity", "Decisions", "Metrics" };
var focusIdx = state.DashboardFocusedPanel;

// For each panel header, apply reverse-video when focused:
string PanelHeader(int panelIdx, string emoji, string title)
{
    if (panelIdx == focusIdx)
        return $"  \x1b[7m{acc} {emoji} {title} \x1b[0m";  // Reverse video = focused
    else
        return $"  {hBg}{B}{acc}{emoji} {title}{R}";         // Normal
}
```

#### Keyboard Handling

```csharp
// In AppLayout.BindKeys():
keys.Key(Hex1bKey.Tab).Action(() =>
{
    if (state.CurrentScreen == Screen.Dashboard)
    {
        state.DashboardFocusedPanel = (state.DashboardFocusedPanel + 1) % 4;
    }
}, "Next Panel");

keys.Key(Hex1bKey.Enter).Action(() =>
{
    if (state.CurrentScreen == Screen.Dashboard)
    {
        // Drill into the focused panel's full screen
        state.CurrentScreen = state.DashboardFocusedPanel switch
        {
            0 => Screen.Roster,
            1 => Screen.ActivityLog,
            2 => Screen.Decisions,
            3 => Screen.Metrics,
            _ => Screen.Dashboard
        };
    }
}, "Drill In");

// Escape already goes back to Dashboard from sub-screens (existing behavior)
```

#### Alternative: Native Focus with Hex1b Button Widgets

We could make each panel header a `Button` widget, which would get native Tab focus management. But this would mean the panels need to be interactive widgets, not just `Text()`, and we'd need to restructure the layout. The state-driven approach is less invasive.

### Concern: Tab Key Conflict

The `TabPanel` widget at the top level already uses Tab/Shift+Tab for navigation. We need to ensure our Dashboard-level Tab doesn't conflict. Options:

1. **Use a different key** for panel cycling within Dashboard (e.g., `Shift+Tab` cycles panels, plain `Tab` still moves to TabPanel tabs)
2. **Intercept Tab only when Dashboard has focus** — check `state.CurrentScreen == Screen.Dashboard` in the handler
3. **Use arrow keys instead** — Left/Right already exist for screen switching via H/L

**Recommendation:** Use **Shift+Tab** or dedicate **Left/Right arrow keys** for panel focus within Dashboard (since H/L already do screen switching). Or, explore whether we can use Hex1b's `WithInputBindings` on the Dashboard's root widget specifically, so Tab is scoped.

---

## 3. New Themes (4-6 Additional Themes)

### Design Principles

Each theme follows our established pattern from ThemeManager:
- Base background color (GlobalTheme.BackgroundColor)
- Foreground tint (GlobalTheme.ForegroundColor)  
- Accent color (primary) — used in headers, selected items, progress bars
- Secondary accent — used in rules, dividers, muted elements
- Panel bg / detail bg / alt bg — three-tier panel background shading
- List selected colors — inverted accent for list highlights
- Splitter divider — panel separation color
- Border colors — invisible (space chars) per our WithModernBorders pattern

### Theme Designs

#### 🌲 Forest — Deep green woodland

```
Base BG:       (10, 18, 12)   — Very dark forest floor
Foreground:    (195, 210, 190) — Soft sage
Accent:        (72, 199, 110)  — Bright emerald
Secondary:     (50, 140, 78)   — Moss
Panel BG:      (14, 24, 16)
Detail BG:     (18, 30, 20)
Alt BG:        (10, 20, 13)
Selected BG:   (72, 199, 110)  — Emerald highlight
Selected FG:   (10, 18, 12)    — Dark on emerald
Splitter:      (45, 75, 52)
ANSI accent:   \x1b[38;2;072;199;110m
ANSI secondary:\x1b[38;2;050;140;078m
```

#### 🌃 Cyberpunk — Neon pink on dark purple

```
Base BG:       (12, 8, 20)    — Deep violet-black
Foreground:    (220, 210, 230) — Lavender white
Accent:        (255, 50, 200)  — Hot neon pink
Secondary:     (170, 40, 140)  — Dark magenta
Panel BG:      (18, 12, 28)
Detail BG:     (24, 16, 36)
Alt BG:        (14, 10, 24)
Selected BG:   (255, 50, 200)
Selected FG:   (12, 8, 20)
Splitter:      (60, 40, 80)
ANSI accent:   \x1b[38;2;255;050;200m
ANSI secondary:\x1b[38;2;170;040;140m
```

#### 🌙 Midnight — Deep blue-black with ice blue accents

```
Base BG:       (8, 10, 18)    — Near-black midnight blue
Foreground:    (190, 200, 220) — Cool gray-blue
Accent:        (100, 160, 255) — Ice blue
Secondary:     (65, 105, 180)  — Steel blue
Panel BG:      (12, 15, 26)
Detail BG:     (16, 20, 34)
Alt BG:        (9, 12, 22)
Selected BG:   (100, 160, 255)
Selected FG:   (8, 10, 18)
Splitter:      (35, 50, 80)
ANSI accent:   \x1b[38;2;100;160;255m
ANSI secondary:\x1b[38;2;065;105;180m
```

#### 🔥 Ember — Warm charcoal with orange-amber glow

```
Base BG:       (18, 12, 8)    — Dark warm brown
Foreground:    (220, 210, 195) — Warm cream
Accent:        (255, 160, 50)  — Bright amber
Secondary:     (180, 110, 40)  — Burnt orange
Panel BG:      (24, 16, 10)
Detail BG:     (30, 20, 14)
Alt BG:        (20, 14, 9)
Selected BG:   (255, 160, 50)
Selected FG:   (18, 12, 8)
Splitter:      (70, 50, 35)
ANSI accent:   \x1b[38;2;255;160;050m
ANSI secondary:\x1b[38;2;180;110;040m
```

#### ❄️ Arctic — Light theme, bright background with cool blue

```
Base BG:       (235, 240, 245) — Ice white-blue
Foreground:    (30, 40, 55)    — Dark navy text
Accent:        (0, 100, 180)   — Deep blue
Secondary:     (80, 120, 160)  — Muted slate
Panel BG:      (225, 232, 240)
Detail BG:     (215, 224, 235)
Alt BG:        (230, 236, 242)
Selected BG:   (0, 100, 180)
Selected FG:   (235, 240, 245)
Splitter:      (180, 195, 210)
ANSI accent:   \x1b[38;2;000;100;180m
ANSI secondary:\x1b[38;2;080;120;160m
```

**Note:** Arctic is our first light theme. Status emoji and ANSI foreground codes should still work because we use bright-enough colors. The `GetDimRule()` and `GetSecondaryAccent()` will need the dark secondary to be visible on the light background.

#### 📺 Retro — CRT green phosphor on black

```
Base BG:       (5, 5, 5)      — Near-black
Foreground:    (40, 200, 40)   — Classic green phosphor
Accent:        (80, 255, 80)   — Bright phosphor green
Secondary:     (30, 140, 30)   — Dim green
Panel BG:      (8, 10, 8)
Detail BG:     (12, 16, 12)
Alt BG:        (6, 8, 6)
Selected BG:   (80, 255, 80)
Selected FG:   (5, 5, 5)
Splitter:      (25, 50, 25)
ANSI accent:   \x1b[38;2;080;255;080m
ANSI secondary:\x1b[38;2;030;140;030m
```

### Implementation Sketch

```csharp
// ThemeManager.cs additions:
public static readonly string[] ThemeNames = [
    "Ocean", "Heist", "Sunset", "HighContrast",
    "Forest", "Cyberpunk", "Midnight", "Ember", "Arctic", "Retro"
];

// Each new theme follows the same CreateXxxTheme() pattern:
public static Hex1bTheme CreateForestTheme() => WithModernBorders(
    new Hex1bTheme("Forest")
        .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(195, 210, 190))
        .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(10, 18, 12))
        .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(20, 35, 24))
        .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(72, 199, 110))
        .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(10, 18, 12))
        .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(72, 199, 110))
        .Set(ListTheme.SelectedIndicator, "  ")
        .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(72, 199, 110))
        .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(18, 30, 20))
        .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(45, 75, 52)));
```

All `GetAccentCode`, `GetSecondaryAccent`, `GetPanelBgColor`, `GetPanelDetailBgColor`, `GetPanelAltBgColor`, `GetPanelHeaderBg`, `GetPanelDetailBg` methods need new switch cases for indices 4-9. The modular `(index % ThemeNames.Length)` pattern already handles wrap-around.

---

## 4. Improved Charts & Metrics

### Current State

MetricsScreen uses:
- `BarChart` (horizontal bars) for velocity/burndown data
- `BreakdownChart` for task status proportions
- Manual `█▓░` text progress bars in Dashboard

### Hex1b Chart Widgets Available (from hex1b.dev docs)

| Widget | Description | Current Use |
|--------|-------------|-------------|
| `BarChart` | Horizontal bars | ✅ Used |
| `BreakdownChart` | Proportional segments | ✅ Used |
| `ColumnChart` | Vertical columns | ❌ Not used |
| `TimeSeriesChart` | Braille line plots | ❌ Not used |
| `ScatterChart` | Braille dot plots | ❌ Not used |
| `Progress` | Native progress bar widget | ❌ Not used (manual `█▓░`) |

### Recommended Enhancements

#### 1. Replace Manual Progress Bar with `Progress` Widget

The Dashboard's `█▓░` progress bar is manually constructed. Hex1b's `Progress` widget provides proper theming, animation, and sub-cell precision:

```csharp
// Before (manual):
mid.Text($"  \x1b[32m{new string('█', doneWidth)}\x1b[33m{new string('▓', activeWidth)}{D}{new string('░', remaining)}{R}  {pct}%")

// After (native):
mid.Progress(completedTasks).Range(0, totalTasks).FixedWidth(30)
```

#### 2. Add TimeSeriesChart for Velocity Trends

Our sprint velocity data is inherently time-series. A braille line chart shows trends much better than bars:

```csharp
// TimeSeriesChart for velocity trend line:
mid.TimeSeriesChart(sprints.Select(s => new ChartItem(s.SprintName, s.CompletedTasks)).ToArray())
    .Title("Velocity Trend")
    .Fill(FillStyle.Braille)  // Braille dots for sub-cell precision
    .ShowGridLines(true)
    .Fill()
```

#### 3. Add ColumnChart for Sprint Comparison

Vertical columns are more natural for "sprint by sprint" comparisons than horizontal bars:

```csharp
// Multi-series stacked column chart:
mid.ColumnChart(sprints)
    .Label(s => s.SprintName)
    .Series("Completed", s => s.CompletedTasks, Hex1bColor.Green)
    .Series("Carried Over", s => s.CarriedOver, Hex1bColor.Yellow)
    .Layout(ChartLayout.Stacked)
    .Title("Sprint Breakdown")
    .ShowValues(true)
    .Fill()
```

#### 4. Use Table for Per-Member Contributions

Currently member contributions are rendered as `Text()` lines. A proper `Table` widget would add sorting, focus tracking, and better alignment:

```csharp
mid.Table(memberStats)
    .Header(h => [
        h.Cell("Member").Width(SizeHint.Fill),
        h.Cell("Done").Width(SizeHint.Fixed(6)).Align(Alignment.Right),
        h.Cell("Active").Width(SizeHint.Fixed(8)).Align(Alignment.Right),
        h.Cell("Pending").Width(SizeHint.Fixed(8)).Align(Alignment.Right),
    ])
    .Row((r, m) => [
        r.Cell(m.Name),
        r.Cell(m.TasksDone.ToString()),
        r.Cell(m.TasksActive.ToString()),
        r.Cell(m.TasksPending.ToString()),
    ])
    .Compact()
    .Fill()
```

#### 5. ScatterChart for Effort vs Completion

If we track estimated effort alongside completion, scatter plots visualize the correlation:

```csharp
mid.ScatterChart(tasks)
    .X(t => t.EstimatedHours)
    .Y(t => t.ActualHours)
    .GroupBy(t => t.Status.ToString())
    .Title("Effort vs Completion")
    .Fill()
```

This requires adding effort tracking to the data model (future enhancement).

#### 6. Indeterminate Progress for Loading States

When data is loading async, show an animated spinner/progress:

```csharp
if (state.IsLoading)
    v.Progress().Indeterminate().FixedWidth(30)
```

### Recommended MetricsScreen Rewrite Plan

- **Wide (≥120):** 3 columns — Left: ColumnChart (stacked sprint breakdown) + BreakdownChart (task status). Center: TimeSeriesChart (velocity trend line). Right: Table (per-member contributions).
- **Medium (≥80):** 2 columns — Left: ColumnChart + stats. Right: TimeSeriesChart.
- **Narrow (<80):** Single column — BreakdownChart + compact stats.
- Toggle V key switches between velocity line chart and burndown line chart (both as TimeSeriesChart).

---

## 5. User Stories for SquadTUI

### Core UX Stories

**US-1: Theme Discovery**  
*As a user, I want to preview themes live before committing, so I can find the one that fits my terminal aesthetic.*
- Modal overlay with live theme switching
- Currently: T key cycles blindly; no preview or list

**US-2: Panel Drill-Down**  
*As a user, I want to Tab between Dashboard panels and Enter to dive into one, so I can navigate without reaching for number keys.*
- Focus ring with visual indicator (reverse-video header)
- Enter maps focused panel to its full screen

**US-3: Keyboard Discoverability**  
*As a new user, I want to see available keybindings contextually, so I don't need to memorize them all upfront.*
- Could use a persistent status bar or contextual hints per screen
- Hex1b `InfoBar` or `Notifications` for contextual tips

**US-4: Notification Feed**  
*As a user, I want to see when squad data changes (new decisions, task updates), so I know the dashboard is live.*
- Hex1b `NotificationPanel` + `ZStack` — toast-style notifications
- Already have `IsLiveEnabled` and file watcher infrastructure
- Post notifications when `HasPendingRefresh` fires

**US-5: Data Tables**  
*As a user, I want sortable, navigable tables for roster and task data, so I can find information quickly.*
- Replace `List` + `Text` detail pattern with `Table` widget
- Add focus tracking, row activation, and column sizing
- Particularly impactful on RosterScreen and MetricsScreen

**US-6: Rich Metrics Visualization**  
*As a team lead, I want line charts showing velocity trends over time, so I can spot patterns at a glance.*
- TimeSeriesChart with braille rendering
- ColumnChart with stacked series for sprint breakdowns

**US-7: Searchable Roster**  
*As a user with a large squad, I want to filter/search the member list, so I can find people quickly.*
- TextBox widget above the List for filtering
- Filter `Members` list in real-time as user types

**US-8: Charter Editing in TUI**  
*As a user, I want to edit charters directly in the TUI with a multi-line text editor, so I don't have to switch to another tool.*
- Hex1b `TextBox` with multi-line mode
- Save back to `.squad/agents/{name}/charter.md`

**US-9: Sprint/Task Board View**  
*As a user, I want a kanban-style task board with columns for each status, so I can visualize work in progress.*
- HStack with 4 columns (Pending | Active | Blocked | Done)
- Each column is a VStack of task cards
- Cards use BackgroundPanelWidget with status-tinted backgrounds

**US-10: Responsive Help Overlay**  
*As a user, I want the help screen to show keybindings relevant to my current screen context, not a global list.*
- Dynamic help content based on `state.CurrentScreen`
- Show screen-specific keys first, then global keys

### Nice-to-Have Stories

**US-11: Drag-and-Drop Task Reordering**  
*Mouse-based task reordering using Hex1b's DragBarPanel.*

**US-12: Terminal-in-Terminal**  
*Embed a child terminal session for running squad commands directly from the TUI (Hex1b `Terminal` widget).*

**US-13: QR Code Sharing**  
*Generate a QR code for the squad repo URL — fun use of Hex1b's `QrCode` widget.*

**US-14: Window-Based Multi-View**  
*Use Hex1b `WindowPanel` for floating detail windows — view multiple members simultaneously.*

**US-15: Animation & Polish**  
*Use `StatePanel` + `EffectPanel` for shimmer effects on loading states, fade transitions between screens.*

---

## 6. Unused Hex1b Widgets Worth Adopting

| Widget | Priority | Where to Use |
|--------|----------|-------------|
| `Progress` | 🔴 High | Dashboard progress bar, loading states |
| `TimeSeriesChart` | 🔴 High | MetricsScreen velocity trends |
| `ColumnChart` | 🟡 Medium | MetricsScreen sprint breakdown |
| `Table` | 🟡 Medium | RosterScreen, MetricsScreen |
| `Picker` | 🟡 Medium | SettingsScreen theme selection (inline, non-modal) |
| `NotificationPanel` | 🟡 Medium | Live data refresh notifications |
| `Spinner` | 🟢 Low | Loading indicator |
| `ToggleSwitch` | 🟢 Low | Settings toggles |
| `Slider` | 🟢 Low | Settings numeric values (if any) |
| `WindowPanel` | 🟢 Low | Floating detail views |
| `Tree` | 🟢 Low | Skills hierarchy display |
| `ScatterChart` | 🟢 Low | Effort/completion correlation |
| `Navigator` | 🟢 Low | Could replace our manual Screen enum routing |

---

## Implementation Priority (Suggested Sprint Order)

1. **New themes** (Forest, Cyberpunk, Midnight, Ember, Arctic, Retro) — pure additive, no risk
2. **Theme modal** (BackdropWidget approach) — small AppState change + new overlay code
3. **Dashboard panel focus ring** — state-driven, minimal code change
4. **Progress widget** — replace manual `█▓░` bars
5. **TimeSeriesChart + ColumnChart** — MetricsScreen enhancement
6. **Notification system** — ZStack + NotificationPanel integration
7. **Table widget adoption** — RosterScreen and MetricsScreen refactors
