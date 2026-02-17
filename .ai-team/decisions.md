# Decisions

> Shared decision log. All agents read this before starting work.
> Scribe merges new decisions from `.ai-team/decisions/inbox/`.

### 2026-02-16: Project Architecture (consolidated)

**By:** Solaire

**What:** Single-project solution structure with `src/SquadTUI/` containing `Models/`, `Services/`, and `Screens/` folders. All domain models use C# `record` types with nullable reference types enabled. Service layer uses interfaces (`ITeamService`, `IDecisionService`, etc.) with concrete implementations that take `teamRootPath` as constructor parameter. An `ISquadDataProvider` aggregates all services into a single facade. Services are wired manually in Program.cs — no DI container yet. Hex1b API style uses fluent builder pattern (`ctx.VStack()`, etc.). Model naming: `SquadTask` (not `Task`) to avoid `System.Threading.Tasks.Task` collision.

**Why:** Simple, understandable architecture that allows parallel work — Siegmeyer can build screens against service interfaces, Andre can implement services, Patches can test with mocks. Single version source in csproj prevents drift. Immutable records with value-equality and nullable references provide type safety. No early complexity — DI container added only when warranted.

**Consequences:** Siegmeyer, Andre, and Patches can work independently against stable interfaces. Architecture is digestible in 5 minutes.

---

### 2026-02-16: CI/CD Pipelines for .NET 10

**By:** Solaire

**What:** Established CI and release workflows for .NET 10 project. CI workflow (`.github/workflows/ci.yml`) runs matrix builds (Windows/macOS/Linux × x64/ARM64) on push to `develop`/`main` and PRs. Release workflow (`.github/workflows/release.yml`) triggers on `release/*` branches, publishes 6 self-contained binaries (zip for Windows, tar.gz for Unix), extracts version from csproj `<Version>` property (currently 0.2.0), creates git tag, and publishes GitHub release with auto-generated notes. Standard `.gitignore` added for .NET projects.

**Why:** Previous workflows referenced Node.js (leftover from prior project). Needed proper .NET 10 infrastructure supporting cross-platform distribution and automated versioning. Single source of truth for version in csproj prevents drift. Self-contained binaries eliminate runtime dependencies for end users. Matrix builds catch platform-specific issues early.

---

### 2026-02-16: Phase 1 UI Modernization - Emoji Icons

**By:** Siegmeyer

**What:** Added emoji icons throughout all TUI screens to modernize visual appearance. All screen titles now have contextual emoji (🏠 Dashboard, 👥 Roster, 📋 Decisions, 🔧 Skills, 📊 Activity Log, 📈 Metrics). Status badges use emoji instead of text labels (✅ Active, 🟡 Idle, 🔵 Working, ⚫ Offline). Task status indicators use emoji (🔄 InProgress, ✅ Done, ⏳ Pending, 🚫 Blocked). List items prefixed with relevant emoji for better scannability.

**Why:** Makes the terminal interface more engaging and visually distinct. Emoji provide instant visual recognition for different sections and states without requiring color support. Safe, universally-compatible enhancement that works in all terminal environments.

**Deferred:** Full theme system, responsive layouts, TabPanel, VScroll, and markdown rendering deferred pending official Hex1b API documentation. Test exploration confirmed these features exist but exact API signatures need verification.

---

### 2026-02-16: Service Integration with Real .ai-team/ Data

**By:** Andre

**What:** Replaced static `SampleData` usage throughout the TUI with real service calls that parse `.ai-team/` files. Created `ServiceProvider` singleton for centralized service instantiation and `DataBridge` wrapper that provides UI-friendly methods: `LoadDashboardDataAsync()`, `LoadRosterDataAsync()`, `LoadDecisionsDataAsync()`, `LoadSkillsDataAsync()`, `LoadLogDataAsync()`, `LoadCharterContentAsync(memberName)`. Extended `AppState` to hold loaded data collections (Members, Decisions, Skills, LogEntries, Dashboard). All screens updated to use real data with SampleData fallback. Data loads asynchronously on app startup using parallel tasks for efficiency.

**Why:** TUI was using hardcoded sample data. Services were already implemented but not wired to UI. This bridges that gap, enabling all screens to display real project data from `.ai-team/` directory while gracefully falling back to sample data if files are missing.

**Impact:** All screens now display real project data when `.ai-team/` files exist. Data loads efficiently via parallel `Task.WhenAll()`. Error handling catches exceptions, sets `ErrorMessage` in state, and gracefully falls back.

---

### 2026-02-17: Test Coverage for Theme, Markdown, and Data Bridge

**By:** Patches

**What:** Created comprehensive test suite expansion with 110 total tests covering ThemeManager, MarkdownRenderer, DataBridge, and E2E scenarios. New test files: `ThemeTests.cs` (validates 3 themes: Ocean, Heist, Daylight, with index clamping), `MarkdownRendererTests.cs` (placeholder for E2E behavior validation), `DataBridgeTests.cs` (tests service aggregation and error handling), `ThemeSwitchingTests.cs` (E2E theme cycling with T key), `ResponsiveLayoutTests.cs` (terminal size variations: wide 120×40, narrow 80×24, small 60×20), `MarkdownRenderingTests.cs` (E2E markdown rendering with bullet points/headings), `CIPipelineTests.cs` (validates GitHub Actions workflows and build success).

**Why:** New features (ThemeManager, MarkdownRenderer, DataBridge) need validation. ThemeManager required unit tests for all 3 themes and index clamping. DataBridge aggregates services and needs error handling tests. MarkdownRenderer behavior tested via E2E since it requires WidgetContext from Hex1b rendering pipeline. CI pipeline tests ensure GitHub Actions workflows are valid and build/test steps succeed.

**Current Blocker:** MarkdownRenderer.cs has 5 compilation errors (WidgetContext generic type issue, Hex1bColor not found) preventing test execution. Once fixed, all new tests should pass.

---

### 2026-02-16: Dashboard UX Architecture (consolidated)

**By:** Firekeeper

**What:** Designed comprehensive responsive UX for SquadTUI dashboard. Responsive multi-panel layout tailored to 3 terminal width ranges (wide ≥120 cols, medium 80–119, narrow <80). Tab-based content model (Overview, Decisions, Activity, Metrics) organizes dashboard sections. Compact sidebar roster visible in wide layout, hidden in mobile layouts. Color language with semantic palette (Primary, Secondary, Accent, Error + status colors). Keyboard-first navigation with global hotkeys, arrow keys, Tab, Enter, and mouse support. Progressive disclosure to maximize information density while keeping UI uncluttered. Design documented in `src/SquadTUI/Screens/UX_DESIGN.md`.

**Why:** Terminal size variability — most TUI projects fail this. SquadTUI needs to scale from 160-col displays down to 80-col terminals and mobile scenarios. Information overload — tab model shows essentials while keeping details available. Power user vs. newcomer — keyboard shortcuts for speed, mouse/help text for discovery. Consistent navigation across all screens using identical patterns. Visual hierarchy — color language eliminates ambiguity (green=done, red=blocked, blue=working).

**Decisions Made:**
- Sidebar always visible in wide layout (not collapsible by default) to maximize horizontal real estate
- Tab switching via arrows/Tab rather than number keys (1–6 reserved for global screen navigation)
- Status badges use emoji (🟢 🟡 🔵 🔴) for quick scanning; text fallback in monospace
- Vim-style j/k navigation optional, not default (barrier to entry for non-Vim users)
- Bottom notification bar limited to 1–2 lines (not a full footer) to maximize content area
- No adaptive layout (which breaks suddenly); responsive breakpoints are smooth

**Impacts:** Siegmeyer implements TabPanel, responsive breakpoints, colors, keyboard/mouse handlers. Solaire may need AppState updates (SidebarWidth, NotificationQueue). Andre feeds real data into dashboard components.

---

### 2026-02-16: User Directive — SquadUI Inspiration

**By:** LondoSpark (via Copilot)

**What:** Use https://github.com/csharpfritz/SquadUI as inspiration for SquadTUI, but don't be afraid to go our own way. Team is looking to potentially integrate into that repo.

**Why:** User request — captured for team memory.

---

### 2026-02-17: Fix Decisions Screen Parser

**By:** Andre

**What:** Fixed the `DecisionService.ParseDecisionsMarkdown()` method to correctly parse the real `.ai-team/decisions.md` file format. The parser was expecting an old format but the actual file uses `### {date}: {title}` headings, `**By:**` fields, `**What:**`/`**Why:**` fields, and `---` separators. Rewrote the parser and added `BuildDecisionContent()` helper. The TUI now correctly displays all decisions from `.ai-team/decisions.md`.

**Why:** The Decisions screen was failing because the parser couldn't understand the real decision file format, returning empty or malformed data. Now all screens display real decisions with proper date, title, author, and structured content.

---

### 2026-02-17: SettingsScreen — dynamic options parameter

**By:** Andre

**What:** `SettingsScreen.Render()` accepts `dynamic options` as its 4th parameter to enable runtime theme switching. This mirrors how `Program.cs` already mutates `options.Theme` in the T-key handler. Using `dynamic` avoids needing to know the exact Hex1b options type at compile time. Also confirmed `ListItemActivatedEventArgs.ActivatedIndex` is the correct property for item activation callbacks.

**Why:** The settings screen needs to change the active theme immediately when the user selects a new one. The `options` object from `WithHex1bApp` is the only way to do this, and `dynamic` keeps the screen decoupled from Hex1b internals.

---

### 2026-02-16: User directive — Attach asciinema recording when closing issues

**By:** LondoSpark (via Copilot)

**What:** From now on, when closing any issue, attach an asciinema recording demonstrating the fix/feature. Use https://docs.asciinema.org/getting-started/ for setup. Use GitHub auth flow for account if needed.

**Why:** User request — visual proof of work on every issue closure.

---

### 2026-02-16: User directive — Recast team to Dark Souls universe

**By:** LondoSpark (via Copilot)

**What:** Rename the entire squad to use Dark Souls as the casting universe. Also ensure everything is well documented.

**Why:** User request — Praise the sun!

---

### 2026-02-16: Definition of Done — CI green + releasable + recordings attached

**By:** LondoSpark (via Copilot)

**What:** A task is NOT done unless: (1) all tests pass on CI, (2) we could cut a release, (3) everything testable is tested including corner cases, (4) when closing an issue, attach an asciinema recording as evidence.

**Why:** User request — captured for team memory.

---

### 2026-02-16: User directive — Git flow enforcement

**By:** LondoSpark (via Copilot)

**What:** The project must use git flow workflow (develop, feature branches, release branches, hotfix branches). Currently only has a `main` branch — needs proper git flow setup.

**Why:** User request — captured for team memory.

---

### 2026-02-16: User directive — Public GitHub repo with README and screenshots

**By:** LondoSpark (via Copilot)

**What:** The project needs to be published as a public GitHub repository under the londospark account. Must include a comprehensive README.md with screenshots, feature descriptions, installation instructions, and usage guide.

**Why:** User request — captured for team memory.

---

### 2026-02-16: User directive — Modern UI with proper border styling

**By:** LondoSpark (via Copilot)

**What:** The current UI border characters aren't modern enough. Need to look at opencode (https://github.com/opencode-ai/opencode) for inspiration on a more modern, polished terminal UI style. The UI should be interactive, with docks, panels, and a full dashboard experience for larger terminals.

**Why:** User request — captured for team memory.

---

### 2026-02-16: User directive — Screenshots and demos

**By:** LondoSpark (via Copilot)

**What:** Install hex1b CLI (https://hex1b.dev/guide/cli) and use asciinema (https://asciinema.org/) for demos/screenshots in the README. The README references screenshots that don't exist yet.

**Why:** User request — captured for team memory.

---

### 2026-02-16: User directive — Test all console widths

**By:** LondoSpark (via Copilot)

**What:** E2E tests must cover all console width breakpoints. Currently only testing at 120 cols default — need tests at narrow (60), medium (80), wide (120), and extra-wide (160+) terminal sizes to validate responsive layout behavior.

**Why:** User request — captured for team memory.

---

### 2026-02-17: Help Screen Keybinding Implementation

**By:** Firekeeper

**What:** Implemented Help screen triggered by F1 key instead of `?`. The Hex1b framework's `Hex1bKey` enum does not expose a direct key code for `?`. F1 is the universal help key across terminal UIs and desktop apps. Behavior: toggle — first press opens Help, subsequent F1 or Escape closes and returns to previous screen. Added `Screen.Help` and `PreviousScreen` property to AppState.

**Why:** F1 convention is more recognizable and aligns with terminal UI patterns. Avoids waiting for Hex1b update or using raw input handling beyond API scope.

---

### 2026-02-18: UX Improvement Specification — Screen Audit

**By:** Firekeeper

**What:** Comprehensive UX audit of all screens. Identified 11 unused Hex1b widgets (Table, TabPanel, Progress, InfoBar, BreakdownChart, ColumnChart, TimeSeriesChart, Spinner, Tree, ToggleSwitch, Notifications). Applied reverse-video headers, subtitles, breathing room, and 4-space indent to Decisions, Skills, ActivityLog, and Help screens. Established design language consistency rules: reverse-video main headers, dim subtitles, section separators with empty lines, no box-drawing borders (use shading instead). Recommended Siegmeyer apply same patterns to Metrics and NoSquad screens.

**Why:** LondoSpark flagged cramped layouts, missing explanations, unwanted borders, and overall visual quality issues. Many available Hex1b widgets were unused.

---

### 2026-02-17: Responsive Layout and Theme Switching E2E Tests

**By:** Patches

**What:** Created E2E test coverage for responsive layout breakpoints (60/80/100/120/160/40 cols) and theme switching (T key cycles Ocean → Heist → Sunset with wrapping). Fixed `DecisionService.cs` compilation errors (duplicate variable name `content` causing CS0136). All 29 tests pass (26 existing + 3 new theme + 6 new responsive). Discovered Hex1b testing patterns: `.Key(Hex1bKey.X)` for input, `await sequence.ApplyAsync(terminal)` for rendering, `snapshot.ContainsText()` with emoji for assertions.

**Why:** Previously only tested at default 120-col width. Dashboard has 3 responsive breakpoints that were untested. Theme cycling had no automated validation. User explicitly requested E2E tests at all breakpoints.

---

### 2026-02-17: Sprint 6 UI Cleanup

**By:** Siegmeyer

**What:** Cohesive UI polish sprint covering 7 issues (#11–#15, #17, #19). Removed all ANSI background escape codes (Hex1b's GlobalTheme handles backgrounds). Kept J/K as state-updating bindings (ListNode.MoveDown/Up are internal). Used Button widgets for clickable NavBar. Removed InfoBar entirely. Changed selection highlighting to background-color-only (removed arrow indicators). Removed bracket notation from NavBar labels in favor of emoji labels.

**Why:** Multiple UI polish issues filed to improve visual consistency, remove clutter, and add modern interaction patterns. ANSI bg codes created jarring rectangles against theme backgrounds. InfoBar consumed vertical space with redundant info.

---

### 2026-02-18: Remove Self-Referential CI Tests

**By:** Solaire

**What:** Removed `DotnetBuild_Succeeds` and `DotnetTest_Passes` from `CIPipelineTests.cs` — both spawned child `dotnet` processes causing infinite recursion on CI. Added `--filter` defense to `ci.yml` to exclude future process-spawning tests. 8 CIPipelineTests remain, validating workflow structure. **Rule going forward:** No test may spawn `dotnet build`, `dotnet test`, or any process that re-invokes the test runner.

**Why:** Both tests broke CI on all 6 platforms. The tests were logically circular — CI already runs `dotnet test`, so asserting it passes adds zero signal.

---

### 2026-02-17: Git Flow and GitHub Repository Setup

**By:** Solaire

**What:** Established Git Flow branching strategy with `develop` as integration branch. Updated CI workflow to trigger on `develop`. Branch structure: `main` (production), `develop` (integration), `feature/*`, `release/*`, `hotfix/*`. Created comprehensive README.md, CONTRIBUTING.md, and MIT LICENSE. Configured git remote for `londospark/SquadTUI`.

**Why:** Need stability on `main`, parallel development isolation, controlled releases, and hotfix capability. Public repo enables collaboration with csharpfritz/SquadUI.

---

### 2026-02-17: Screenshot & Demo Infrastructure

**By:** Solaire

**What:** Established screenshot/demo infrastructure using hex1b CLI tool (`dotnet tool install -g Hex1b.Tool` v0.87.0). Added `.WithDiagnostics()` to Program.cs for hex1b CLI discovery. Created `scripts/capture-screenshots.ps1` for automated SVG screenshot capture across all screens with optional asciinema recording. Created `docs/SCREENSHOTS.md` guide. Updated README.md to reference SVG screenshots and asciinema demo.

**Why:** README referenced `.png` screenshots that didn't exist. SVG is hex1b CLI's native high-quality format — scalable, crisp, version-control friendly. Automated script ensures screenshots can be regenerated after UI changes.

