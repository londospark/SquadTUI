# Comprehensive Test Plan — Patches

**Date:** 2026-02-19
**By:** Patches (Tester)
**Sprint:** 11+ (ongoing)

---

## 1. Current Test Coverage Summary

**Total tests:** 180 (as listed by `dotnet test --list-tests`)

| Category | Count | Files |
|----------|-------|-------|
| Unit / Models | 14 | `ModelTests.cs` |
| Unit / Screens | 21 | `AppStateTests.cs`, `HelpScreenTests.cs`, `MetricsChartDataTests.cs`, `SettingsScreenTests.cs` |
| Unit / Services | 8 files | `TeamServiceTests.cs`, `DecisionServiceTests.cs`, etc. |
| Unit / SampleData | 21 | `SampleDataTests.cs` |
| Unit / Theme | varies | `ThemeManagerTests.cs`, `ThemeBackgroundTests.cs`, `ThemeEscapeCodeLengthTests.cs` |
| Unit / Rendering | 4+ | `PanelRendererTests.cs`, `MarkdownRendererTests.cs` |
| Integration | 7 files | `CIPipelineTests.cs`, service integration tests |
| E2E | 17 files | Navigation, screens, responsive, themes, tabs |

---

## 2. Coverage Gaps Identified

### A. Untested Production Code

| File | What's Missing |
|------|---------------|
| `DataBridge.cs` | No tests for `LoadCharterContentAsync()`, `LoadTasksFromRosterAsync()` task-status mapping logic, error propagation |
| `SettingsService.cs` | No tests for JSON serialization round-trip, file-not-found handling, corrupt JSON handling, directory creation |
| `FileWatcherService.cs` | `FileWatcherServiceTests.cs` exists but no E2E test verifies live dashboard refresh actually triggers |
| `MigrationService.cs` | No tests for `.ai-team/` → `.squad/` migration — git mv path, filesystem move path, already-migrated guard, error cases |
| `SquadDetector.cs` | `SquadDetectorTests.cs` exists but no test for git-root-based detection path (spawns `git` subprocess) |
| `SquadPathResolver.cs` | No test for `GetActiveDirectoryName()` when neither directory exists |
| `MarkdownRenderer.cs` | No unit tests for `ExtractHeadings()` method. Inline bold toggle logic (`**text**`) has edge cases untested (unclosed `**`, nested formatting) |
| `AppLayout.cs` | No tests for migration banner rendering, sub-screen overlay behavior (MemberDetail/Charter/Help/Settings showing tabs but with none selected) |
| `SkillsScreen.cs` | `GetRelatedMembers()` and `GetConfidenceLevel()` are private static methods with hardcoded skill→member mapping. No tests verify these return correct data for real skill names vs unknown skills |

### B. Untested UI Behaviors

| Behavior | Risk |
|----------|------|
| Settings toggle via Enter key | SettingsScreen `OnItemActivated` handler not E2E tested |
| Theme change from Settings screen | `ToggleSetting` case 0 cycles themes but no E2E test |
| V key burndown toggle on Metrics | E2E tests exist but don't verify chart data changes |
| Live dashboard refresh (file watcher) | `HasPendingRefresh` flag is set but no test verifies dashboard re-renders with new data |
| Error state display | `state.ErrorMessage` is set on data load failure but no screen ever reads or displays it |
| Loading state display | `state.IsLoading` is set but no screen checks it — no spinner, no "Loading…" |
| M key migration | No E2E test for migration flow |
| C key squad creation | `NoSquadGuardTests` tests file creation, but doesn't verify the created files have correct content |

---

## 3. High-Risk Test Areas

### 3.1 SampleData Leak Detection Tests (NEW — P0)

**Goal:** Ensure production screens never show SampleData when real data is available.

| Test | Description |
|------|-------------|
| `WhenRealDataLoaded_DashboardShowsRealMemberNames` | Populate AppState with custom members, verify SampleData names (Sonic, Tails, etc.) don't appear |
| `WhenRealDataLoaded_RosterShowsRealMemberNames` | Same for Roster screen |
| `WhenRealDataLoaded_DecisionsShowRealContent` | Populate Decisions, verify SampleData decision titles absent |
| `CharterScreen_AlwaysShowsSampleData` | Regression test — document that CharterScreen currently always uses SampleData until fixed |
| `MetricsScreen_AlwaysShowsSampleSprintData` | Regression test — document that sprint history is always hardcoded |
| `MemberDetailScreen_DefaultsMemberToSonic` | Verify `state.SelectedMemberName ?? "Sonic"` behavior |

### 3.2 Data Loading Edge Cases (P0)

| Test | Description |
|------|-------------|
| `DataBridge_LoadFails_ErrorMessageSet` | Verify `state.ErrorMessage` populated on exception |
| `DataBridge_PartialLoadFails_OtherDataStillAvailable` | If one Task.WhenAll task fails, other data still populates |
| `DataBridge_EmptyDirectory_ReturnsEmptyCollections` | `.ai-team/` exists but has no files |
| `DataBridge_CharterNotFound_ReturnsNull` | Member exists but no charter.md file |
| `DataBridge_TaskStatus_MappingCorrectness` | Verify `MemberStatus.Working → InProgress`, `Active → InProgress`, `Idle → Pending` |

### 3.3 Theme Switching (P1)

| Test | Description |
|------|-------------|
| `ThemeCycle_AllFourThemes_RenderWithoutCrash` | Cycle through Ocean→Heist→Sunset→HighContrast→Ocean at each screen |
| `ThemeChange_FromSettings_UpdatesGlobalTheme` | Enter on Theme setting cycles theme, verify rendering changes |
| `ThemeColors_AllMethodsReturnValidAnsi` | GetAccentCode, GetSecondaryAccent, GetPanelHeaderBg, etc. for all 4 indices |
| `ThemeColors_NegativeIndex_NoException` | Pass -1, -100 to theme color methods |
| `ThemeColors_LargeIndex_WrapsCorrectly` | Pass 5, 100, int.MaxValue to theme color methods |

### 3.4 Responsive Layout Tests (P1)

| Test | Description |
|------|-------------|
| `MetricsScreen_WideLayout_ShowsThreeColumns` | ≥120 cols shows Sprint Overview + Chart + Contributions |
| `MetricsScreen_MediumLayout_ShowsTwoColumns` | ≥80 cols shows Sprint Overview + Chart |
| `MetricsScreen_NarrowLayout_ShowsSingleColumn` | <80 cols shows compact metrics |
| `HelpScreen_WideLayout_ShowsTwoColumns` | ≥100 cols shows Navigation + Actions columns |
| `HelpScreen_NarrowLayout_ShowsSingleColumn` | <100 cols shows all sections stacked |
| `SettingsScreen_AllWidths_ShowsSettingsOptions` | Settings at 40, 60, 80, 120 cols |
| `AllScreens_At1Col_NoException` | Extreme: 1-column terminal doesn't crash |

### 3.5 Tab Navigation Tests (P1)

| Test | Description |
|------|-------------|
| `TabPanel_SyncWithCurrentScreen` | Changing screen via 1-6 keys updates TabPanel selected state |
| `TabPanel_SelectionChanged_UpdatesCurrentScreen` | Clicking/selecting tab updates AppState.CurrentScreen |
| `OverlayScreens_ShowTabsButNoneSelected` | MemberDetail/Charter/Help/Settings show tab bar with no selected tab |
| `TabPanel_ReturnsToCorrectTab_AfterOverlay` | Navigate to Roster→MemberDetail→Escape returns to Roster tab |

### 3.6 Navigation Corner Cases (P2)

| Test | Description |
|------|-------------|
| `Escape_FromCharter_GoesToMemberDetail` | Not to Dashboard |
| `Escape_FromSettings_GoesToDashboard` | Settings → Dashboard |
| `Escape_FromHelp_ReturnsToPreviousScreen` | If PreviousScreen is set |
| `F1_Toggle_ReturnsToSameScreen` | F1 from Roster shows Help, F1 again returns to Roster |
| `J_K_OnSettings_NavigatesOptions` | SettingsSelectedIndex changes with j/k |
| `Enter_OnSettings_TogglesValue` | Settings Enter handler fires correctly |
| `J_OnLastItem_StaysAtEnd` | Roster, Decisions, Skills, ActivityLog — doesn't go past last item |
| `K_OnFirstItem_StaysAtZero` | Doesn't go negative |
| `H_L_DoNotWorkOnNoSquadScreen` | Guards checked |
| `SelectedMemberName_NullOnStartup_DefaultsGracefully` | MemberDetailScreen handles null |

### 3.7 MarkdownRenderer Tests (P2)

| Test | Description |
|------|-------------|
| `ExtractHeadings_Level1` | `# Title` → (1, "Title") |
| `ExtractHeadings_Level2` | `## Section` → (2, "Section") |
| `ExtractHeadings_Level3` | `### Subsection` → (3, "Subsection") |
| `ExtractHeadings_MultipleHeadings` | Mixed levels in one document |
| `ExtractHeadings_NoHeadings_Empty` | Plain text returns no headings |
| `Render_Null_ReturnsEmptyWidget` | Null markdown → single empty text widget |
| `Render_Empty_ReturnsEmptyWidget` | Empty string → single empty text widget |
| `Render_BoldToggle_UnclosedBold` | `**open but not closed` — doesn't crash |
| `Render_NestedListItems` | `  - sub-item` gets correct indent |
| `Render_HorizontalRule` | `---` renders as line |
| `Render_Blockquote` | `> text` renders with `│` prefix |

### 3.8 Migration Service Tests (P2)

| Test | Description |
|------|-------------|
| `Migrate_NoLegacyDir_ReturnsFalse` | No `.ai-team/` → failure result |
| `Migrate_NewDirAlreadyExists_ReturnsFalse` | `.squad/` already present |
| `Migrate_SuccessfulFilesystemMove` | `.ai-team/` → `.squad/` via Directory.Move |
| `Migrate_PreservesDirectoryContents` | Files inside `.ai-team/` appear in `.squad/` |
| `NeedsMigration_TrueWhenOnlyLegacy` | Has `.ai-team/` but no `.squad/` |
| `NeedsMigration_FalseWhenBothExist` | Has both — `.squad/` takes precedence |
| `NeedsMigration_FalseWhenOnlyNew` | Has `.squad/` only |

### 3.9 Settings Persistence Tests (P2)

| Test | Description |
|------|-------------|
| `SettingsService_Save_CreatesDirectory` | `~/.config/squadtui/` created |
| `SettingsService_SaveAndLoad_RoundTrip` | Save settings → Load → values match |
| `SettingsService_Load_FileNotFound_ReturnsDefaults` | No file → default AppSettings |
| `SettingsService_Load_CorruptJson_ReturnsDefaults` | Invalid JSON → default AppSettings |
| `SettingsService_Save_Exception_DoesNotThrow` | Write failure swallowed gracefully |

### 3.10 Monadic Type Tests (Future — when Result<T> is introduced)

| Test | Description |
|------|-------------|
| `Result_Success_ContainsValue` | `Result<T>.Ok(value)` holds value |
| `Result_Failure_ContainsError` | `Result<T>.Fail(error)` holds error message |
| `Result_Map_TransformsSuccessValue` | Functor law |
| `Result_Map_PreservesFailure` | Failure short-circuits |
| `Result_Bind_ChainsSuccessfully` | Monad bind composes |
| `Result_Bind_ShortCircuitsOnFailure` | First failure stops chain |
| `DataBridge_ReturnsResult_OnServiceFailure` | Service throws → Result.Fail, not exception |
| `Screen_HandlesResultFailure_ShowsErrorUI` | Result.Fail renders error message on screen |

---

## 4. UI Inconsistencies Found

### 4.1 Formatting Issues

1. **DashboardScreen progress bar off-by-one.** Line 91: `doneWidth + activeWidth` can exceed 30 when rounding, causing `remaining < 0` which is clamped but creates a shorter bar.

2. **MetricsScreen duplicate code.** The sprint overview stats block (lines 72-74, 130-132, 160-161) is copy-pasted across all 3 responsive layouts with `SampleData.*` references. Should be extracted to a helper.

3. **MemberDetailScreen shows member name twice.** Header line 35 shows `👤 {member.Name}` and line 37 shows status badge + `{member.Name}` again immediately below.

4. **HelpScreen says `?` toggles help** (lines 55, 113) but there's no `?` key binding in `AppLayout.BindKeys()`. Only F1 and Escape are bound for Help. The help text is misleading.

5. **NoSquadScreen references `npx github:bradygaster/squad`** but the correct package reference should be verified. Also says "Press C to create a basic squad structure" but the `C` key handler creates `.squad/` directory, not `.ai-team/`.

6. **SkillsScreen hardcoded confidence levels.** `GetConfidenceLevel()` returns hardcoded bars for 5 known skills. Any real skill from the service gets a default "Medium" bar. No real confidence data exists.

### 4.2 Spacing Issues

1. **ActivityLogScreen** has extra leading `detail.Text("")` at line 45 before the detail header — creates unnecessary blank space at top of detail pane.

2. **DecisionsScreen** has extra `detail.Text("")` at line 42 — same issue.

3. **SkillsScreen** has extra `detail.Text("")` at line 49 — same issue.

---

## 5. User Stories (Tester's Perspective)

### US-1: As a user, I want to see real data from my project, not demo data
**Acceptance:** When `.ai-team/` or `.squad/` has valid files, no SampleData content appears on any screen. When loading, a spinner or "Loading…" text is shown.

### US-2: As a user, I want to know when I'm seeing demo data
**Acceptance:** If real data isn't available, a visible "Demo" or "Sample" badge appears. User is never silently shown fabricated data.

### US-3: As a user, I want my settings to persist across sessions
**Acceptance:** Theme choice, vim bindings toggle, and other settings survive app restart. Settings are stored in `~/.config/squadtui/settings.json`.

### US-4: As a user, I want to migrate from .ai-team/ to .squad/ safely
**Acceptance:** Press M shows migration progress. Files are preserved. Git history is maintained if in a git repo. App works normally after migration.

### US-5: As a user, I want the Help screen to accurately reflect available keybindings
**Acceptance:** Every key listed in Help (?) actually works. No phantom keybindings listed.

### US-6: As a user, I want the app to handle edge cases gracefully
**Acceptance:** Empty directories, missing files, corrupt data, extreme terminal sizes — none of these crash the app. Each shows a reasonable fallback.

### US-7: As a user, I want to view and edit real member charters
**Acceptance:** Charter screen shows content from `.ai-team/agents/{name}/charter.md`, not hardcoded SampleData text.

### US-8: As a tester, I want deterministic E2E tests that don't depend on file system state
**Acceptance:** TestAppBuilder can be seeded with custom AppState data. Tests don't touch the real file system or depend on `.ai-team/` directory existence.

---

## 6. Test Infrastructure Recommendations

1. **Add `TestAppBuilder.WithData()` method** — Allow E2E tests to pre-populate AppState with custom members, decisions, etc. This enables SampleData leak detection tests.

2. **Add test category attributes** — `[Trait("Category", "SampleDataLeak")]`, `[Trait("Category", "Responsive")]`, etc. for selective test runs.

3. **Create `TestFixtures.CustomMembers`** — A set of member/decision/task fixtures with obviously non-SampleData names (e.g., "TestMember1") for leak detection.

4. **Add MarkdownRenderer unit tests** — `ExtractHeadings()` is a pure function that can be tested without Hex1b context.

5. **Consider snapshot testing** — For E2E tests, capture terminal snapshots as text files and diff against known-good baselines. This catches unintended UI regressions more precisely than `ContainsText()`.
