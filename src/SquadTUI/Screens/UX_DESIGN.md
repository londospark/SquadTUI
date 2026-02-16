# SquadTUI Dashboard — UX Design Document

**Design Lead:** Saul (UX/Design)  
**Project:** SquadTUI — Terminal interface for AI squad management  
**Scope:** Enhanced dashboard experience with responsive layouts for wide (≥120 cols), medium (80–119 cols), and narrow (<80 cols) terminals  
**Status:** Design phase — ready for Linus (Frontend) to implement  

---

## 1. Core Philosophy

The dashboard is the user's **command center**. On landing, they should see:
- **At a glance:** Team health (members, active tasks, recent wins)
- **What changed:** Recent activity and decisions
- **What's next:** Quick access to the team and their work

We remove decisions from the user—no wasted screen real estate, no "how do I find X?"

---

## 2. Responsive Layout Architecture

### 2.1 Wide Terminal (≥120 columns, ≥40 rows)
**Visual:** IDE-like multi-panel layout. *Resembles VS Code with sidebar, main area, and footer.*

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ [1]Dashboard [2]Roster [3]Decisions [4]Skills [5]Log [6]Metrics  [?]Help [Q]Quit                     │
├──────────────┬──────────────────────────────────────────────────────────────────────────────────────────┤
│              │ ┌─────────────────────────────────────────────────────────────────────────────────────┐ │
│ Team Roster  │ │ [Overview]  [Decisions]  [Activity]  [Metrics]                                     │ │
│ (compact)    │ ├─────────────────────────────────────────────────────────────────────────────────────┤ │
│              │ │                                                                                     │ │
│ • Anna       │ │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐ ┌────────────┐  │ │
│   Lead       │ │  │ Team             │  │ Active Tasks    │  │ Recent Decisions │ │ Velocity   │  │ │
│   🟢 Active  │ │  │ ─────────────── │  │ ────────────── │  │ ────────────── │ │ ──────────│  │ │
│   Sprint π   │ │  │ 8 Members        │  │ 5 in Progress  │  │ 2 This Week    │ │ +42 pts    │  │ │
│              │ │  │ 7 Active         │  │ 2 Blocked      │  │ Weekly avg: 52 │ │ Sprint avg │  │ │
│              │ │  │                  │  │                │  │                │ │            │  │ │
│ • Bob        │ │  └──────────────────┘  └──────────────────┘  └──────────────────┘ └────────────┘  │ │
│   Dev        │ │                                                                                     │ │
│   🟡 Idle    │ │  ┌─────────────────────────────────────────────────────────────────────────────┐  │ │
│   None       │ │  │ Recent Activity                                                             │  │ │
│              │ │  ├─────────────────────────────────────────────────────────────────────────────┤  │ │
│ • Carol      │ │  │ 14:32  @anna   Merged "Color language decisions"  (Decisions)               │  │ │
│   Design     │ │  │ 13:15  @bob    Completed API auth prototype (Task: node-auth)              │  │ │
│   🔵 Working │ │  │ 12:01  @carol  Added 3 new UI sketches (Activity Log)                       │  │ │
│   Task:UI    │ │  │ 11:47  @anna   Reviewed skill matrix with team (Team Activity)              │  │ │
│              │ │  │  9:23  @bob    Started integration tests for API (Task: api-integration)   │  │ │
│              │ │  └─────────────────────────────────────────────────────────────────────────────┘  │ │
│ • Dave       │ │                                                                                     │ │
│   QA         │ │                                                                                     │ │
│   🔵 Working │ │                                                                                     │ │
│   Task:Test  │ │                                                                                     │ │
│              │ │                                                                                     │ │
│              │ │                                                                                     │ │
│              │ └─────────────────────────────────────────────────────────────────────────────────────┘ │
├──────────────┴──────────────────────────────────────────────────────────────────────────────────────────┤
│ 🔔 3 notifications  |  💾 Last synced: 2 min ago  |  [Command Palette: Ctrl+P]  [Search: /]           │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

**Panel anatomy:**

| Panel | Content | Behavior |
|-------|---------|----------|
| **Top Nav** | Global navigation (1-6 for screens, ? for help, Q to quit) | Always visible. Shows current screen with ▶ marker |
| **Left Sidebar** | Compact team roster (name + role + status badge + current task) | Always visible. Click member → detail view. Resizable via drag splitter (50–180 cols wide). Shows 8–15 members depending on terminal height |
| **Main Area** | TabPanel with 4 tabs: Overview, Decisions, Activity, Metrics | Switchable with arrows, numbers, or mouse. Active tab colored with Accent (cyan/blue). Icons: 📊 Overview, 📋 Decisions, 📝 Activity, 📈 Metrics |
| **Bottom Panel** | Notification feed + command palette stub + last-sync indicator | Shows action feedback (e.g., "✓ Member saved", "⚠ Sync failed"), recent commands, status. Max 1-2 lines, scrollable left/right for overflow |

**Sidebar state:**
- Width: 18–30 chars (name + role + badge), resizable to 180 max
- Rows per member: 1 (compact), expandable to 2 on hover/focus
- Status badge colors: 🟢 (Active/Done), 🟡 (Idle/Pending), 🔵 (In-Progress/Working), 🔴 (Blocked/Error)
- Truncate names if <60 cols total width

---

### 2.2 Medium Terminal (80–119 columns)
**Visual:** Focused, tab-based layout. *Sidebar hidden or collapsed; horizontal tabs dominate.*

```
┌──────────────────────────────────────────────────────────────┐
│ [1]Dashboard [2]Roster [3]Decisions [4]Skills [5]Log [6]Metrics │
├──────────────────────────────────────────────────────────────┤
│ [Overview]  [Decisions]  [Activity]  [Metrics]             │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Team: 8 Members (7 active)                                 │
│  Tasks: 12 total — 5 in progress, 4 done                    │
│                                                              │
│  ── Active Tasks ──                                         │
│  • [node-auth] @bob    In-Progress  Updated 14:01           │
│  • [ui-sketch] @carol  In-Progress  Updated 13:45           │
│  • [api-test] @dave    In-Progress  Updated 13:22           │
│                                                              │
│  ── Recent Activity ──                                      │
│  14:32  @anna   Merged "Color language decisions"           │
│  13:15  @bob    Completed API auth prototype                │
│  12:01  @carol  Added 3 new UI sketches                     │
│                                                              │
│  ── Recent Decisions ──                                     │
│  2026-02-15  Color language (Saul) — Rounded borders,       │
│              accent for active tabs                         │
│  2026-02-14  Tab naming (Danny) — Overview, Decisions,      │
│              Activity, Metrics                              │
│                                                              │
│ 🔔 3 notifications  |  💾 Last synced: 2 min ago           │
└──────────────────────────────────────────────────────────────┘
```

**Key changes:**
- Sidebar: **hidden** (or toggleable with `[S]idebar` key for ≥100 cols)
- Main area: full width
- Vertical space: more generous (less info density, clearer hierarchy)
- Summary cards: stack vertically, 1 per row

---

### 2.3 Narrow Terminal (<80 columns)
**Visual:** Mobile-first, single column. *Full-screen content with bottom nav icons.*

```
┌────────────────────────────────┐
│ [1]  [2]  [3]  [4]  [5]  [6]  Q│
├────────────────────────────────┤
│                                │
│  Dashboard                     │
│                                │
│  Team: 8 (7 active)            │
│  Tasks: 12 (5 in, 4 done)      │
│                                │
│  ── Activity ──                │
│  14:32  @anna  Merged PR       │
│  13:15  @bob   API auth ✓      │
│  12:01  @carol UI sketches     │
│                                │
│  ── Decisions ──               │
│  Feb 15  Color language        │
│  Feb 14  Tab naming            │
│                                │
│ [🔔 3]  [? Help]  [↺ Sync]    │
└────────────────────────────────┘
```

**Key changes:**
- No sidebar
- Single column layout
- Bottom nav: icons only (1 char each), labels on hover/focus
- Tab switching: left/right arrow keys or `[<` `[>` text buttons
- Information density: very low; show one section at a time or minimal stack

---

## 3. Dashboard Content: Four-Tab Model

The dashboard main area is a **TabPanel** with 4 tabs. Users switch with:
- **Mouse:** Click tab header
- **Keyboard:** `←`/`→` arrows, `Tab` to focus next tab
- **Numbers:** Future—reserved for per-tab quick actions (not implemented yet)

### 3.1 Overview Tab
**Purpose:** Instant health check—at-a-glance metrics and team snapshot.

**Layout (Wide):**
```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  ┌────────────┐
│ Team            │  │ Active Tasks    │  │ Recent Decisions│  │ Velocity   │
│ ─────────────── │  │ ────────────── │  │ ────────────── │  │ ──────────│
│ 8 Members       │  │ 5 in Progress  │  │ 2 This Week    │  │ +42 pts   │
│ 7 Active ✓      │  │ 2 Blocked ⚠    │  │ Weekly avg: 52 │  │ Sprint avg│
│ 1 Idle          │  │ 0 Completed    │  │                │  │ +38 pts   │
│                 │  │                │  │                │  │           │
└─────────────────┘  └─────────────────┘  └─────────────────┘  └────────────┘

[View All Roster] [View All Tasks] [View All Decisions] [View Metrics]
```

**Cards:**
- **Team:** Total, active count, idle count. Link to Roster screen.
- **Active Tasks:** Count by status (In Progress, Blocked, Completed). Link to task detail (future).
- **Recent Decisions:** Count this sprint, weekly average. Link to Decisions tab.
- **Velocity:** Points this sprint, last 3-sprint average. Link to Metrics tab.

**Wide/Medium layout:** 4-column grid, stacked vertically
**Narrow layout:** 1 column, vertical stack

---

### 3.2 Decisions Tab
**Purpose:** Browse and review recent decisions, sorted by date (newest first).

**Layout:**
```
Recent Decisions (Newest First)

Feb 15  Color language (Saul)
        Rounded borders everywhere. Primary/Secondary/Accent/Error colors.
        Status colors: 🟢 done, 🟡 idle, 🔵 in-progress, 🔴 blocked.
        ▼ Show full decision  [Edit] [Archive]

Feb 14  Tab naming (Danny)
        Overview, Decisions, Activity, Metrics. Consistent across all screens.
        ▼ Show full decision  [Edit]

Feb 12  Keyboard shortcuts (Saul)
        Arrow keys for nav. Vim-style j/k (optional). Tab for focus. ? for help.
        ▼ Show full decision  [Edit]
```

**Interaction:**
- List items are **selectable** (highlight with Accent color on focus)
- **Click/Enter** → expand to show full decision text
- **Mouse drag:** (future) drag to reorder or archive
- Truncate decisions to 50 chars if narrow; show `…` to indicate overflow

**Links:** Click decision → open in a detail view (future) or expand inline

---

### 3.3 Activity Tab
**Purpose:** Chronological log of recent team actions (commits, decisions, task updates).

**Layout:**
```
Recent Activity (Last 5 Days)

Feb 15, 14:32  [DECISION]  @anna   Merged "Color language decisions"
               Saul's design decision on borders, colors, typography.

Feb 15, 13:15  [TASK]      @bob    Completed API auth prototype
               Task: api-auth → DONE. Ready for integration.

Feb 15, 12:01  [LOG]       @carol  Added 3 new UI sketches
               Uploaded designs to decisions/sketches/.

Feb 15, 11:47  [ACTIVITY]  @anna   Reviewed skill matrix with team
               Updated charter with findings.

Feb 14, 09:23  [TASK]      @bob    Started integration tests
               Task: api-integration → IN-PROGRESS.
```

**Columns (left to right):**
- **Date & Time:** Feb 15, 14:32
- **Type:** [DECISION], [TASK], [LOG], [ACTIVITY] — colored badges (Accent for decisions, primary for tasks, muted for logs)
- **Actor:** @username
- **Action & Subject:** Human-readable summary
- **Detail:** Subtitle with context

**Interaction:**
- Scroll (mouse wheel or arrow keys) to see older activity
- Click entry → (future) show details or navigate to related screen
- Filter by type (future): [All] [Decisions] [Tasks] [Activity]

---

### 3.4 Metrics Tab
**Purpose:** Sprint velocity, burndown, and team performance trends.

**Layout (Wide):**
```
Sprint Velocity                    Team Capacity
┌──────────────────────┐          ┌──────────────────┐
│                   /‾ │          │ Anna:   100%  ✓ │
│                /     │          │ Bob:    85%   ⚠ │
│              /       │          │ Carol:  100%  ✓ │
│            /         │          │ Dave:   70%   ⚠ │
│          /           │          │ Eve:    100%  ✓ │
│        /             │          │ Frank:  50%   🔴│
│ Sprint π   π+1  π+2  │          │ Grace:  100%  ✓ │
│    +42    +38   +36  │          │ Henry:  90%   ⚠ │
│                      │          │                  │
└──────────────────────┘          └──────────────────┘

Week-over-week Performance        Blockers
Sprint π-2:  +36 pts  ✓           None (Clean!)
Sprint π-1:  +38 pts  ✓
Sprint π:    +42 pts  ✓ (trend +)

Top performers:  Anna (+12), Carol (+10), Dave (+8)
```

**Cards:**
- **Sprint Velocity:** Line chart (ASCII or text) showing last 3 sprints. Trend indicator (↑ ↓ →).
- **Team Capacity:** List of members with % utilization. Color: 🟢 100%, 🟡 70–99%, 🔴 <70%.
- **Week-over-week:** Table of sprint totals. Highlights best sprints.
- **Blockers:** Short list of active blockers (from task/decision tracking). Empty state: "None (Clean!)".

**Narrow layout:** Stack vertically, reduce chart detail

---

## 4. Color Language & Semantic Palette

### 4.1 Palette Definition

| Name | Use | Example | Hex-Like Notes |
|------|-----|---------|----------|
| **Primary** | Borders, labels, navigation focus | Tab borders, text dividers | Cyan/Blue (bright) |
| **Secondary** | Subtle accents, muted content | Timestamps, role labels | Dim cyan, gray |
| **Accent** | Active state, highlights | Selected tab, active member | Bold cyan or magenta |
| **Error** | Failures, blockers, warnings | Blocked tasks, sync errors | Red/bright red |
| **Background** | Panel alternation, slight depth | Alternating rows in lists | Dim gray, very subtle |

### 4.2 Status Color Mapping

| Status | Color | Icon | Usage |
|--------|-------|------|-------|
| **Active / Done** | 🟢 Green | ✓ | Member online, task complete, decision approved |
| **Idle / Pending** | 🟡 Yellow | ⊘ | Member away, task waiting, decision draft |
| **In-Progress / Working** | 🔵 Blue | ◉ | Member working, task in-progress |
| **Blocked / Error** | 🔴 Red | ✗ | Task blocked, member unavailable, error state |

**Hex values (Hex1b/ANSI color codes):**
- 🟢 Green: `#00ff00` or bright-green
- 🟡 Yellow: `#ffff00` or bright-yellow
- 🔵 Blue: `#0088ff` or bright-cyan
- 🔴 Red: `#ff0000` or bright-red
- Primary: `#00ffff` (bright cyan)
- Secondary: `#444444` (dim gray)
- Background: `#1a1a1a` (very dark gray)

### 4.3 Application Rules

**Borders:**
- All panel borders: **Primary** (bright cyan)
- Alternating rows in lists: **Background** (subtle dim background), no border
- Tab borders: **Primary**, active tab highlight with **Accent** color (bold underline or background)

**Text:**
- Labels (e.g., "Team:", "Status:"): **Primary** or **Secondary**
- Values (e.g., member names, task titles): Default/white
- Badges (role, status): **Accent** on muted background
- Timestamps: **Secondary** (dim)
- Active selection (list highlight): **Accent** background + white text

**Status cards:**
- Good news (active, done, on-track): **Primary** + 🟢 green accents
- Warnings (idle, pending, at-risk): **Error** + 🟡 yellow
- In-motion (working, in-progress): **Accent** + 🔵 blue
- Failures (blocked, error): **Error** + 🔴 red

---

## 5. Interactive Patterns

### 5.1 Keyboard Navigation

**Global:**
- `1–6`: Jump directly to screen (1=Dashboard, 2=Roster, 3=Decisions, 4=Skills, 5=Log, 6=Metrics)
- `?`: Open help modal (shows shortcuts, navigation tips)
- `Q`: Quit SquadTUI
- `Ctrl+P`: Open command palette (future: quick actions, search)
- `/`: Open search (future: search decisions, members, activity)

**In lists (roster, decisions, activity):**
- `↑` / `↓`: Move selection up/down
- `j` / `k`: Vim-style up/down (optional, opt-in)
- `Enter` / `Space`: Activate selected item (open detail, expand, navigate)
- `Escape`: Deselect or close detail view

**In TabPanel (dashboard):**
- `←` / `→`: Switch tabs left/right
- `Tab`: Focus next element (navigate across tabs and buttons)
- `Shift+Tab`: Focus previous element

**In sidebar (wide layout only):**
- `Shift+←`: Collapse sidebar (if open)
- `Shift+→`: Expand sidebar (if closed)
- `|`: Resize splitter (mouse only, or future keyboard input)

### 5.2 Mouse Interaction

**Click targets:**
- **Tab headers:** Click to switch tabs
- **List items:** Click to select, double-click to activate (or single-click if list supports it)
- **Card titles:** Click to navigate (e.g., "View All Roster" → Roster screen)
- **Member names (sidebar):** Click → Member Detail screen
- **Decision titles:** Click → expand or navigate to detail view
- **Activity entries:** Click → (future) show full entry or navigate to context

**Drag & drop:**
- **Sidebar splitter:** Drag left/right to resize (min 18 chars, max 180 chars)
- **Tab headers:** (Future) drag to reorder tabs or drag item to card

**Scroll:**
- **Mouse wheel:** Scroll lists, activity feed, decision list
- **Page Up / Page Down:** Scroll faster
- **Home / End:** Jump to top/bottom of list

### 5.3 Transitions & Feedback

**Keyboard/input feedback (notifications in bottom panel):**
- ✓ Green badge: "Member saved", "Synced 5 items", "Decision created"
- ⚠ Yellow badge: "Sync in progress…", "Unsaved changes"
- ✗ Red badge: "Failed to save", "Sync error", "Invalid input"
- ℹ Blue badge: "Press ? for help", "Welcome to SquadTUI"

**Transition (tab switches, screen navigation):**
- No fade/wipe (too slow for TUI). Instant swap with a brief "loading" state if async.
- Active tab underline shifts smoothly (visual feedback of focus).

**Selection highlight:**
- Selected list item: Accent background + white text (inverted)
- Focused input: Bright border + cursor highlight

---

## 6. Information Density Strategy

### 6.1 Progressive Disclosure Principle

**Wide terminal (≥120 cols):**
- Show 4 summary cards + recent activity + recent decisions all at once
- Sidebar visible → quick member preview
- All tabs visible at top
- User can see 80% of team info without scrolling

**Medium terminal (80–119 cols):**
- Show 3–4 key metrics, stack activity and decisions
- Sidebar hidden (toggleable)
- Fewer rows in lists, but still comprehensive
- User scrolls occasionally

**Narrow terminal (<80 cols):**
- Show 1 section at a time, or minimal vertical stack
- Activity list: 3–4 items only
- Decisions list: 2–3 items only
- User navigates between screens more

### 6.2 Defaults

**Dashboard overview cards:**
- **Team card:** Show member count, active count only. Details in Roster screen.
- **Tasks card:** Show in-progress and blocked counts. Details in task view (future).
- **Decisions card:** Show count this sprint, weekly avg. Full list in Decisions tab.
- **Velocity card:** Show current sprint points, 1-sprint trend. Chart in Metrics tab.

**Sidebar member rows:**
- Wide: Name (truncated if >12 chars) + role (abbrev. 3–4 chars) + status badge + current task (if any, truncated)
- Medium: Collapsed if present, OR show on-demand with [S]idebar toggle
- Narrow: Hidden entirely

**Activity feed:**
- Wide: 5 entries, 2–3 lines per entry
- Medium: 4 entries, 1–2 lines per entry
- Narrow: 2–3 entries, 1 line per entry

---

## 7. Responsive Breakpoints

| Viewport | Min Width | Max Width | Layout | Sidebar | Nav | Density |
|----------|-----------|-----------|--------|---------|-----|---------|
| Wide | 120 | ∞ | 3-column (sidebar + main + footer) | Always visible | Top bar | High (all info visible) |
| Medium | 80 | 119 | 2-column (main + footer) | Toggleable [S] | Top bar | Medium (compact) |
| Narrow | — | 79 | 1-column (main + footer) | Hidden | Bottom icons | Low (one section at a time) |

**Transition rules:**
- At 119 → 80 cols: Sidebar hides automatically. If user was focused on sidebar member, shift focus to main content.
- At 79 → <80 cols: Tab headers may wrap or consolidate to icons.
- At >120 cols: Show sidebar if previously hidden. Restore full-width main area.

---

## 8. Detailed Mock: Overview Tab (Wide Layout)

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ [1]Dashboard [2]Roster [3]Decisions [4]Skills [5]Log [6]Metrics  [?]Help [Q]Quit                     │
├──────────────┬──────────────────────────────────────────────────────────────────────────────────────────┤
│              │ ┌─────────────────────────────────────────────────────────────────────────────────────┐ │
│ Team Roster  │ │ 📊 [Overview]  📋 [Decisions]  📝 [Activity]  📈 [Metrics]                      │ │
│ (18 chars)   │ ├─────────────────────────────────────────────────────────────────────────────────────┤ │
│              │ │                                                                                     │ │
│ • Anna       │ │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐ ┌────────────┐  │ │
│   Lead       │ │  │ Team             │  │ Active Tasks    │  │ Recent Decisions │ │ Velocity   │  │ │
│   🟢 Active  │ │  │ ─────────────── │  │ ────────────── │  │ ────────────── │ │ ──────────│  │ │
│   Sprint π   │ │  │ 8 Members        │  │ 5 in Progress  │  │ 2 This Week    │ │ +42 pts   │  │ │
│              │ │  │ 7 Active ✓       │  │ 2 Blocked ⚠    │  │ Weekly avg: 52 │ │ Sprint avg│  │ │
│              │ │  │ 1 Idle           │  │ 0 Completed    │  │                │ │ +38 pts   │  │ │
│              │ │  │                  │  │                │  │                │ │            │  │ │
│ • Bob        │ │  │ [View All →]     │  │ [View All →]   │  │ [View All →]   │ │ [Chart →]│  │ │
│   Dev        │ │  │                  │  │                │  │                │ │            │  │ │
│   🟡 Idle    │ │  └──────────────────┘  └──────────────────┘  └──────────────────┘ └────────────┘  │ │
│   —          │ │                                                                                     │ │
│              │ │  ┌─────────────────────────────────────────────────────────────────────────────┐  │ │
│ • Carol      │ │  │ Recent Activity                                                             │  │ │
│   Design     │ │  ├─────────────────────────────────────────────────────────────────────────────┤  │ │
│   🔵 Working │ │  │ 14:32  [DECISION]  @anna   Merged "Color language decisions"               │  │ │
│   Task:UI    │ │  │ 13:15  [TASK]      @bob    Completed API auth prototype ✓                 │  │ │
│              │ │  │ 12:01  [LOG]       @carol  Added 3 new UI sketches                        │  │ │
│              │ │  │ 11:47  [ACTIVITY]  @anna   Reviewed skill matrix with team                │  │ │
│              │ │  │  9:23  [TASK]      @bob    Started integration tests                      │  │ │
│              │ │  │                                                                             │  │ │
│              │ │  │                                                                    [More ↓]  │  │ │
│              │ │  └─────────────────────────────────────────────────────────────────────────────┘  │ │
│              │ │                                                                                     │ │
│              │ │                                                                                     │ │
│              │ └─────────────────────────────────────────────────────────────────────────────────────┘ │
├──────────────┴──────────────────────────────────────────────────────────────────────────────────────────┤
│ 🔔 3 notifications  |  💾 Last synced: 2 min ago  |  ✓ Member "Carol" updated  |  [Ctrl+P Command Palette] │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

**Color annotations:**
- **Primary (cyan):** Borders around all panels, labels ("Team", "Active Tasks", etc.)
- **Accent (bright cyan/magenta):** Active tab underline, "Overview" tab indicator
- **Status colors:** 🟢 Anna (active), 🟡 Bob (idle), 🔵 Carol (working), etc.
- **Error:** If a blocker existed, 🔴 red badge
- **Secondary (dim gray):** Timestamps, "Sprint avg:" labels
- **Background:** Subtle alternating row colors in lists (light gray), borders themselves are primary

**Interactive elements:**
- `[View All →]` buttons are clickable; Enter/Space activates them
- Click "Overview" tab → (already selected; no-op)
- Click "Decisions" tab → switch to Decisions tab
- Click member name in sidebar → navigate to MemberDetail screen
- Hover over activity entry → show tooltip with full text (if truncated)

---

## 9. Detailed Mock: Decisions Tab (Medium Layout)

```
┌─────────────────────────────────────────────────────────────────┐
│ [1]Dashboard [2]Roster [3]Decisions [4]Skills [5]Log [6]Metrics │
├─────────────────────────────────────────────────────────────────┤
│ 📊 [Overview]  📋 [Decisions]  📝 [Activity]  📈 [Metrics]    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ── Recent Decisions (Newest First) ──                         │
│                                                                 │
│  > Feb 15  Color language (Saul)                              │
│    Rounded borders everywhere. Colored based on context.      │
│                                                                 │
│  > Feb 14  Tab naming (Danny)                                 │
│    Overview, Decisions, Activity, Metrics consistent.         │
│                                                                 │
│  > Feb 12  Keyboard shortcuts (Saul)                          │
│    Arrow keys for nav. j/k optional. Tab for focus. ? help.   │
│                                                                 │
│  > Feb 10  Color palette (Saul)                               │
│    Primary, Secondary, Accent, Error. Semantic mapping.       │
│                                                                 │
│  > Feb 08  Info density rules (Saul)                          │
│    Progressive disclosure. Show essentials, reveal on demand.  │
│                                                                 │
│  ▼ Showing 5 of 12 decisions. [Load More ↓]                  │
│                                                                 │
│ 🔔 Synced 2 min ago  |  [↑↓ to navigate] [Enter to expand]    │
└─────────────────────────────────────────────────────────────────┘
```

**Interactions:**
- `↑` `↓`: Navigate list
- `Enter`: Expand selected decision (show full text)
- `→`: Switch to Activity tab
- `←`: Switch to Overview tab
- `[Load More ↓]`: Click or press End to load older decisions

---

## 10. Accessibility & Keyboard-First Design

### 10.1 Focus Management

- **On-screen indicator:** Selected list item has **Accent-colored background** (inverted).
- **Tab order:** Top nav → Tab headers → Sidebar (if visible) → Main content → Bottom notification bar.
- **Keyboard navigation:** Every interactive element is reachable without mouse.
- **Alt/Mnemonic keys:** Number keys (1–6) for screens; `?` for help; `Q` to quit.

### 10.2 Discoverability

- **Help text:** Bottom bar always shows next actions (e.g., "[↑↓ Navigate] [Enter Expand] [? Help]").
- **Inference:** If user presses unknown key, show contextual tip (e.g., user presses `j` in Decisions list → "Vim shortcuts enabled (optional). Press ? to configure.").
- **[?] Help modal:** Shows all shortcuts for current screen, along with global shortcuts.

### 10.3 Readable Contrast

- **Text color:** White/default on dark background (high contrast)
- **Accent colors:** Bright cyan, bright yellow, bright red, bright green (high saturation for visibility)
- **No text on dark background with low contrast:** All labels at least 4:1 ratio

---

## 11. State Management & Persistence

### 11.1 Selection Memory

- **Current screen:** Persisted in `AppState.CurrentScreen`
- **Sidebar selected member:** Persisted in `AppState.SelectedMemberName`
- **List selection indices:** Persisted (RosterSelectedIndex, DecisionSelectedIndex, etc.)
- **Sidebar width:** Persisted in AppState (new field: `SidebarWidth`)
- **Notification history:** Last 3–5 notifications shown in bottom bar (max 1–2 lines)

### 11.2 Data Refresh

- **Auto-sync:** Every 2 minutes (background)
- **Manual sync:** `Ctrl+R` or [Refresh] button
- **Notification:** Bottom bar shows "💾 Last synced: X min ago" and sync status
- **Conflict handling:** If data changes mid-edit, show modal: "Data changed. [Reload] [Keep mine] [Merge]" (future)

---

## 12. Implementation Roadmap for Linus

### Phase 1: Layout & Structure (MVP)
- [ ] Implement wide-layout 3-panel skeleton (sidebar + main + footer)
- [ ] Add TabPanel widget to main area (4 tabs: Overview, Decisions, Activity, Metrics)
- [ ] Implement responsive breakpoints (hide sidebar at <120 cols, hide tabs at <80 cols)
- [ ] Build top nav bar with screen links (1–6) and help (?)
- [ ] Build bottom notification bar with last-sync indicator

### Phase 2: Dashboard Content
- [ ] Overview tab: 4 summary cards + activity feed
- [ ] Decisions tab: List view with expand/collapse
- [ ] Activity tab: Chronological log view
- [ ] Metrics tab: Velocity chart + team capacity + blockers

### Phase 3: Colors & UX Polish
- [ ] Apply color language (Primary, Secondary, Accent, Error)
- [ ] Add status badge colors (🟢 🟡 🔵 🔴)
- [ ] Implement active tab underline highlighting
- [ ] Add notification badges (✓ ⚠ ✗ ℹ)

### Phase 4: Keyboard & Mouse Interaction
- [ ] Implement arrow keys for list nav
- [ ] Implement Enter to activate
- [ ] Implement mouse click to select/navigate
- [ ] Implement mouse drag on sidebar splitter to resize
- [ ] Implement global hotkeys (1–6, ?, Q)

### Phase 5: Refinements
- [ ] Add Vim-style j/k navigation (opt-in)
- [ ] Implement /search and Ctrl+P command palette
- [ ] Add data refresh logic (sync every 2 min)
- [ ] Memory persistence (AppState serialization)

---

## 13. Design Principles Summary

1. **No wasted real estate.** Every pixel serves a purpose. Remove clutter ruthlessly.
2. **Keyboard-first, mouse-friendly.** Power users navigate blind; newcomers see labels.
3. **Progressive disclosure.** Summary first, details on demand. Stack high-level info before dive-deep.
4. **One action per keypress.** Avoid multi-key combos (except Ctrl+P for power-user feature).
5. **Status at a glance.** Colors + icons + badges. No need to read fine print.
6. **Consistent navigation.** Same patterns across all screens: arrow keys, Enter, Tab, ?.
7. **Mobile-first fallback.** Narrow layout is still useful, just less fancy.
8. **Responsive, not adaptive.** Layouts reflow smoothly; no jarring page breaks.

---

## 14. File Locations & Next Steps

**This document:** `src/SquadTUI/Screens/UX_DESIGN.md`

**Related files (for Linus to implement):**
- `src/SquadTUI/Screens/DashboardScreen.cs` — Main dashboard render logic
- `src/SquadTUI/Screens/AppState.cs` — Navigation & selection state
- `src/SquadTUI/Screens/NavBar.cs` — Top navigation
- `src/SquadTUI/Screens/SampleData.cs` — Mock team, task, decision data

**Next phase:** Linus builds the TabPanel widget + responsive layout + color theming. This design doc is his blueprint.

---

**Design By:** Saul (UX/Design)  
**Reviewed By:** [TBD]  
**Status:** Ready for implementation  
**Last Updated:** 2026-02-16
