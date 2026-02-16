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

