# User Stories — Sprint 17 Navigation Revamp

**Author:** Siegmeyer  
**Date:** 2026-02-19  
**Sprint:** 17

## Completed

### US-1: Stack-Based Navigation
**As a** user navigating the TUI,  
**I want** a stack-based navigation model (Enter to drill in, Escape to go back),  
**So that** navigation is intuitive and predictable with clear forward/backward flow.

**Acceptance Criteria:**
- [x] TabPanel removed from rendering pipeline
- [x] Number keys (1-6) and H/L cycling removed
- [x] `NavigationStack` in AppState tracks navigation depth
- [x] Enter on Dashboard panels drills into corresponding screen
- [x] Escape pops back to previous screen
- [x] Escape on Dashboard is a no-op (stack empty)
- [x] Right/Left arrows cycle panel focus on Dashboard

### US-2: Settings Modal Overlay
**As a** user opening settings,  
**I want** settings to appear as a centered modal overlay on top of the current screen,  
**So that** I don't lose context of where I was when adjusting settings.

**Acceptance Criteria:**
- [x] ZStack renders current screen as layer 0, Backdrop+modal as layer 1
- [x] Modal is fixed 56×17 characters, centered
- [x] OnClickAway dismisses the modal (Escape also works)
- [x] Closing settings returns to the exact screen underneath

### US-3: Dashboard Panel Sizing Consistency
**As a** user viewing the Dashboard,  
**I want** all panel headers to be the same width regardless of focus state,  
**So that** the layout doesn't shift when I move focus between panels.

**Acceptance Criteria:**
- [x] Focused and unfocused PanelHeader() use identical text structure
- [x] Only ANSI color codes differ between focused/unfocused
- [x] No layout reflow when cycling panel focus

## Future (Proposed)

### US-4: Animated Transitions
**As a** user navigating between screens,  
**I want** subtle slide or fade transitions,  
**So that** navigation feels polished and spatial.

### US-5: Breadcrumb Trail
**As a** user deep in the navigation stack,  
**I want** a breadcrumb indicator showing my navigation path (e.g., Dashboard > Roster > Member),  
**So that** I always know where I am in the hierarchy.

### US-6: Keyboard Shortcut Overlay
**As a** user learning the TUI,  
**I want** a quick-reference overlay showing available shortcuts for the current screen,  
**So that** I can discover features without memorizing the help screen.
