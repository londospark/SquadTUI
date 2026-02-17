# User Stories — SquadTUI UX Improvements

**Date:** 2026-02-18  
**By:** Firekeeper (UX/Design Lead)  
**Status:** Specification  
**Audience:** LondoSpark, Siegmeyer, Andre, Patches

---

## Preamble: Why These Stories?

After auditing SquadTUI's UX, I identified recurring friction points that slow down power users and confuse newcomers. These stories address navigation clarity, information density, keyboard consistency, and first-run experience. Each story is written from the **user's perspective**, not the builder's — they describe a problem and desired outcome, not implementation details.

---

## Story 1: Dashboard-First Navigation

### Title
**As a SquadTUI user, I want the Dashboard to be my home, so I can navigate to any view without hunting through tabs.**

### Current Pain
- Tabs are scattered across the top (6 of them: Dashboard, Roster, Decisions, Skills, Activity, Metrics)
- User has to mentally map "which number opens what"
- Pressing `1` opens Dashboard, but then to see Roster you need to remember it's `2` or Tab multiple times
- No visual guide for where you are in the tab stack

### Desired Behavior
1. **App starts on Dashboard** (home, no negotiation)
2. **Dashboard shows 3–4 focusable panels** (Roster, Activity, Decisions, Metrics)
3. **Tab key moves focus between panels** — visual highlight shows which panel is "active"
4. **Enter key drills into that panel** — opens full-screen view of the roster, activity feed, etc.
5. **Escape key pops back** — always returns to Dashboard
6. **Footer hints guide the user:** `[Tab] Navigate · [Enter] Drill in · [Esc] Back`

### Acceptance Criteria
- ✅ No tab bar visible (removed entirely)
- ✅ Dashboard panels are visibly focusable (highlight, border glow, or accent color)
- ✅ Tab cycles through all visible panels (adaptive to terminal width)
- ✅ Enter opens full-screen view of focused panel
- ✅ Escape returns from any drilled-in view to Dashboard
- ✅ Footer text updates to reflect current screen context
- ✅ Works at 60/80/100/120/160 col terminal widths

### Why This Matters
- **Clarity:** One entry point (Dashboard) eliminates decision fatigue
- **Speed:** Tab-and-Enter rhythm is faster than hunting for numbers
- **Discoverability:** New users see panels and naturally press Tab (vs. guessing `1–6`)
- **Mobile-friendly:** Touch users can see focusable regions without tab bar clutter

---

## Story 2: Discoverable Keyboard Shortcuts

### Title
**As a power user, I want keyboard shortcuts to be consistent and easy to discover, so I don't have to memorize a different key for every screen.**

### Current Pain
- Help screen exists but keys are scattered (1–6, ↑↓, J/K, Q, T, ?)
- No consistent pattern (why is theme toggle `T` but not `Th`?)
- Footer changes from screen to screen but doesn't organize by category
- Users don't know what keys are available until they read Help (F1)

### Desired Behavior
1. **Consistent key patterns:**
   - Navigation: Always `↑↓` or `J/K` (user's choice in Settings)
   - Activation: Always `Enter`
   - Back: Always `Escape`
   - Quit: Always `Q`
   - Help: Always `F1` or `?`
2. **Footer is self-documenting** — shows only actions available on current screen
3. **Help screen is organized by category** (Navigation, Actions, Settings, Help)
4. **Tutorial on first run** — optional walkthrough of key patterns (not forced)

### Acceptance Criteria
- ✅ All navigation uses the same keys (no conflicting bindings per screen)
- ✅ Footer text is categorized (`[Nav]`, `[Actions]`, `[Settings]`, `[Help]`)
- ✅ F1 opens Help from any screen
- ✅ Help screen groups keys by category with descriptions
- ✅ Settings has "Keybinding Style" option: Default (arrows) or Vim (j/k)
- ✅ First-run modal explains core shortcuts (optional, can be dismissed)

### Why This Matters
- **Muscle memory:** Users build confidence faster if patterns are predictable
- **Accessibility:** Consistent keys help screen reader users navigate more intuitively
- **Reduced cognitive load:** New users don't memorize 20 keys; they learn 5 patterns
- **Error reduction:** No "wrong" key guess if all screens follow same logic

---

## Story 3: Settings as a Modal, Not a Screen

### Title
**As a user, I want to adjust Settings (theme, keybindings) without leaving my current view, so I don't lose my place in the dashboard.**

### Current Pain
- Settings opens as a full-width screen (like any other screen)
- User has to press number key to get back to Dashboard/Roster/etc.
- Interrupts workflow (context switch)
- Can't compare settings changes in real-time (e.g., toggle theme, see it live on dashboard behind modal)

### Desired Behavior
1. **Settings opens as a centered modal overlay** (floats above Dashboard or drilled-in view)
2. **Background dims** so modal is clearly the focus, but user still sees content behind it
3. **Modal size is adaptive** (50–60 cols wide on normal terminals, full width on narrow)
4. **Escape closes the modal** — user returns to exact place they left
5. **Theme changes are immediate** (modal still open, so user sees the change live)
6. **No separate "Settings screen"** in the screen rotation

### Acceptance Criteria
- ✅ `S` key (or Settings command) opens centered modal
- ✅ Modal has box border (bright, using Accent color)
- ✅ Background behind modal is visibly dimmed (40–50% opacity)
- ✅ Theme toggle shows effect immediately on content behind modal
- ✅ Escape closes modal, returns to previous screen
- ✅ Tab order in modal works (↑↓ navigate, Enter toggles/selects)
- ✅ Modal works at all terminal widths (min 50 cols)

### Why This Matters
- **Reduced context switching:** Settings is a quick adjustment, not a navigation event
- **Live preview:** Users can see theme changes immediately
- **Familiarity:** Web/mobile users are comfortable with modal overlays
- **Workflow continuity:** User stays mentally focused on their task

---

## Story 4: Error States & Empty Data Handling

### Title
**As a user with no squad loaded (or missing files), I want clear, helpful error messages, so I don't think the app is broken.**

### Current Pain
- NoSquad screen exists but is confusing (box-drawn borders, unclear instructions)
- If data fails to load, user sees blank panels with no explanation
- No distinction between "still loading" and "failed to load"
- Recovery path is not obvious (how do I fix this?)

### Desired Behavior
1. **Welcome/Onboarding screen** (NoSquad) is bright and inviting
   - Explains what SquadTUI is, what a Squad is, how to set it up
   - Provides clear next steps (links to docs, example `.ai-team/` structure)
   - Uses emoji and friendly language (not technical jargon)
2. **Loading states** are explicitly indicated
   - "Loading dashboard..." message while async data fetches
   - Spinner or progress indicator (if Hex1b supports it)
3. **Error messages are actionable**
   - "Missing `decisions.md` in `.ai-team/` — using sample data" (not a red wall of text)
   - Graceful fallback to SampleData (already implemented, but not visible to user)
4. **Recovery is automatic** — app doesn't crash or hang

### Acceptance Criteria
- ✅ NoSquad welcome screen is visually inviting (no scary borders)
- ✅ Loading states show a message or spinner
- ✅ Error messages are < 2 lines, end with actionable hint
- ✅ App falls back to SampleData gracefully (user sees placeholder message)
- ✅ No blank panels — every panel has either real data, loading message, or helpful error
- ✅ Help screen explains error states

### Why This Matters
- **Confidence:** Users aren't confused or worried the app broke
- **Onboarding:** Welcome screen sells SquadTUI to new users
- **Resilience:** App handles missing files without crashing
- **Trust:** Transparent errors build user confidence

---

## Story 5: Information Density at All Terminal Widths

### Title
**As a user on a narrow terminal (or in a split pane), I want the dashboard to adapt gracefully, so I can still see essential information without horizontal scrolling.**

### Current Pain
- Dashboard is designed for 120+ col terminals
- At 80 cols, panels overflow or get cut off
- At 60 cols (phone-width or split pane), it's unusable
- No adaptive breakpoints (design jumps abruptly, or content disappears)

### Desired Behavior
1. **Responsive breakpoints at 3 levels:**
   - **Wide (≥120 cols):** 4-panel grid (Roster, Activity, Decisions, Metrics)
   - **Medium (80–119 cols):** 2–3 panels, or 2 rows of 2 panels
   - **Narrow (<80 cols):** Single column, panels stack vertically (scroll to see all)
2. **Information density is optimized per width:**
   - **Wide:** Full content, full context
   - **Medium:** Abbreviated summaries, expandable sections
   - **Narrow:** Headers + counts only (drill in for details)
3. **No horizontal scroll** — all content fits within terminal width
4. **Tab cycling adapts** — only focuses visible panels in that layout

### Acceptance Criteria
- ✅ Dashboard renders correctly at 60/80/100/120/160 col widths
- ✅ No content is cut off (no horizontal scrolling)
- ✅ Panel layout shifts smoothly at breakpoints (no jarring jumps)
- ✅ Tab key only cycles through visible panels
- ✅ E2E tests cover all 5+ breakpoint widths
- ✅ Footer text wraps or abbreviates if terminal is <50 cols

### Why This Matters
- **Inclusivity:** Users on older terminals, split panes, or mobile SSH can still use SquadTUI
- **Robustness:** Tests at multiple widths catch layout bugs early
- **Professional polish:** Adaptive design signals quality
- **Accessibility:** Zoomed/enlarged text often reduces terminal width; app still works

---

## Story 6: Breadcrumb & Context Cues

### Title
**As a user who drills into a detailed view, I want to always know how to get back and where I am in the hierarchy, so I don't feel lost.**

### Current Pain
- When user presses Enter on a panel, they jump to full-screen Roster/Decisions/etc.
- No visual cue that they can press Escape to go back
- No indication they're "inside" Dashboard (vs. a separate app)
- Help screen must be read to find out how to navigate back

### Desired Behavior
1. **Screen title includes a back cue:**
   - Roster: `← 👥 Roster` (left arrow hints at "back")
   - Or subtitle: "Viewing from Dashboard"
2. **Footer always shows:** `[Esc] Back to Dashboard` (or context-specific "Back")
3. **Optional subtle breadcrumb** at top: `🏠 Dashboard > 👥 Roster` (dim color, 1 line)
4. **Escape key is always available** and returns to Dashboard

### Acceptance Criteria
- ✅ All drilled-in screens show a back cue (arrow or breadcrumb)
- ✅ Footer includes "[Esc] Back" hint on every drilled-in screen
- ✅ Screen titles are distinct from Dashboard title
- ✅ Breadcrumb (if implemented) doesn't clutter layout
- ✅ No text-only "You are here" — visual design carries the message

### Why This Matters
- **Orientation:** Users always know where they are and how to escape
- **Confidence:** Visual cues reduce anxiety (no "what if I'm stuck?")
- **Speed:** Users don't pause to think "how do I go back?" — they see footer hint
- **Mobile-friendly:** Breadcrumb is familiar to touch users

---

## Story 7: First-Run Experience & Onboarding

### Title
**As a new SquadTUI user, I want a gentle introduction to the app, so I can start being productive without reading docs.**

### Current Pain
- App launches silently (no welcome message)
- User sees Dashboard with 3 panels but doesn't know what they do
- Keybindings are hidden in Help screen (not obvious they exist)
- NoSquad welcome screen is confusing

### Desired Behavior
1. **First launch shows a brief tour** (optional, can be skipped)
   - "Press Tab to navigate panels, Enter to explore"
   - "What is each panel? (Roster = team members, Activity = recent updates, Decisions = team decisions)"
   - "Press F1 for help anytime"
2. **Tour runs once, then never again** (stored in Settings or local state)
3. **NoSquad welcome explains** what a Squad is and how to set it up
4. **Every screen has a self-documenting footer** (no assumption of prior knowledge)

### Acceptance Criteria
- ✅ First launch shows optional tour modal
- ✅ Tour can be dismissed by pressing Escape or any key
- ✅ Tour doesn't show on subsequent launches
- ✅ NoSquad welcome is clear and friendly
- ✅ All footers include at least one hint (not blank)
- ✅ Help screen covers all core concepts

### Why This Matters
- **Reduced barrier to entry:** New users can start using the app immediately
- **Confidence:** Tour teaches the core interaction model (Tab → Enter → Escape)
- **Repeatability:** Users can always press F1 to re-learn
- **Retention:** Good onboarding reduces "I don't get it" abandonment

---

## Story 8: Accessible Terminal Navigation

### Title
**As a user with accessibility needs (screen reader, keyboard-only, high-contrast), I want SquadTUI to work with assistive tech, so I can use the app like any other user.**

### Current Pain
- No screen reader support (Hex1b may not expose ARIA, but text content should be readable)
- Colors are used as sole indicator of state (focus, errors, status)
- No explicit "role" labels (user doesn't know if an element is a button, a list, etc.)
- Contrast might be too low in some themes

### Desired Behavior
1. **Keyboard-only navigation** (already mostly true, but audit for gaps)
   - Every action accessible without mouse
   - Tab order is logical and expected
2. **Color + text for all states** (never color-only)
   - Focus: Bright border + status text "Focused"
   - Error: Red text + emoji (🚫) + descriptive message
   - Status: Emoji + text (not just color)
3. **High-contrast mode** (optional in Settings)
   - Bright colors, no subtle grays
   - Larger emoji/symbols
4. **Screen reader hints** (if Hex1b supports)
   - Announce panel names on Tab
   - Announce modal opening/closing
   - Read error messages aloud

### Acceptance Criteria
- ✅ All UI elements accessible via keyboard (no mouse required)
- ✅ Colors are never the only indicator (text/emoji backup)
- ✅ Tab order is logical and consistent
- ✅ High-contrast theme option in Settings
- ✅ No WCAG AAA violations (aim for at minimum AA)
- ✅ Tested with at least one screen reader (NVDA on Windows)

### Why This Matters
- **Inclusivity:** Accessibility is a human right, not a feature
- **Legality:** WCAG compliance avoids legal exposure
- **Quality:** Accessible code is often cleaner, more robust
- **Stigma-free:** Power users (not just disabled users) benefit from a11y features

---

## Summary Table

| Story | Focus | Why Now | Priority |
|-------|-------|---------|----------|
| 1. Dashboard-First Navigation | Information architecture | Eliminates tab bar confusion | 🔴 P0 |
| 2. Discoverable Keyboard Shortcuts | Navigation consistency | Users stuck on keyboard learning | 🔴 P0 |
| 3. Settings Modal | Workflow continuity | Settings shouldn't be context-switching | 🟠 P1 |
| 4. Error States & Onboarding | First-run experience | Confusing for newcomers | 🟠 P1 |
| 5. Responsive Layout | Information density | Fails on narrow terminals | 🔴 P0 |
| 6. Breadcrumbs & Context | Navigation clarity | Users confused about where they are | 🟠 P1 |
| 7. First-Run Tour | Onboarding | New users don't know how to start | 🟠 P1 |
| 8. Accessibility | Inclusive design | Compliance + power user features | 🟡 P2 |

---

## Relationship to Stack Navigation Spec

These stories inform the **Stack Navigation UX Specification** (`firekeeper-stack-nav-ux-spec.md`):

- **Story 1** → Stack navigation (Dashboard home, Tab → Enter → Escape)
- **Story 2** → Keyboard consistency rules in spec (section 5)
- **Story 3** → Settings modal spec (section 4)
- **Story 4** → Error states spec (section 9)
- **Story 5** → Responsive behavior spec (section 7)
- **Story 6** → Breadcrumb/footer spec (sections 2 & 6)
- **Story 7** → First-run UX spec (section 8)
- **Story 8** → Accessibility spec (section 10)

The spec is the **implementation blueprint**; these stories are the **user problems** that the spec solves.

---

## Questions for Team

1. **Which stories are most urgent?** (P0 vs. P1 vs. P2)
2. **Do any stories conflict with existing tech decisions?** (e.g., Hex1b limitations)
3. **Should first-run tour be required, optional, or conditional?** (based on squad data existing)
4. **For accessibility:** Which assistive tech should we test with? (NVDA, JAWS, VoiceOver?)
5. **Which stories should be E2E tested?** (all of them? just P0?)

---

**Status:** Ready for team discussion, prioritization, and assignment.

**Next Steps:**
1. Team reviews stories in standup
2. Prioritize by impact + effort
3. Assign to sprints (P0 = this sprint, P1 = next sprint, P2 = backlog)
4. Andre/Siegmeyer estimate effort for each story
5. Patches creates test cases for each story's acceptance criteria
