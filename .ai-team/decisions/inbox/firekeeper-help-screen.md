# Decision: Help Screen Keybinding Implementation

**Date:** 2026-02-17  
**Agent:** Firekeeper (UX/Design)  
**Context:** Issue #18 — Add quick help screen with keybinding support

## Decision
Implemented Help screen triggered by **F1 key** instead of **?** (question mark).

## Rationale
1. **Hex1bKey enum limitation:** The Hex1b framework's `Hex1bKey` enum does not expose a direct key code for the question mark (?). Standard candidates like `Oem2`, `Slash`, or `Question` were unavailable.

2. **Fallback choice:** F1 is the universal help key across terminal UIs and desktop applications. It's immediately discoverable and widely expected.

3. **UX impact:** While the original spec requested ?, the F1 convention is more recognizable to users and aligns with terminal UI patterns.

## Implementation Details
- **Key code:** `Hex1bKey.F1` (primary) + Escape (backup dismiss)
- **Behavior:** Toggle — first press opens Help, subsequent F1 or Escape closes and returns to previous screen
- **Fallback navigation:** If no previous screen stored, returns to Dashboard

## Alternatives Considered
1. Map to a different ASCII key (T for toggle?) — conflicts with Theme toggle
2. Wait for Hex1b update — delays feature delivery
3. Use raw input handling — exceeds Hex1b API scope

## Files Changed
- `src/SquadTUI/Screens/HelpScreen.cs` — Created new help screen
- `src/SquadTUI/Screens/AppState.cs` — Added `Screen.Help`, `PreviousScreen` property
- `src/SquadTUI/Program.cs` — Added F1 binding and Help case to switch

## Success Criteria
✅ Help screen displays all keybindings in organized categories  
✅ F1 toggles help on/off  
✅ Escape dismisses help  
✅ ANSI styling uses accent colors for headers, dim gray for labels  
✅ Previous screen is restored after help dismissed  
✅ Build succeeds with no new errors  

## Notes for Future Work
- If Hex1b adds ? key support in future versions, consider updating to match original spec for max discoverability
- Monitor user feedback on F1 convention acceptance
