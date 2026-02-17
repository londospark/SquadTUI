# Decision: Theme Menu with Live Preview via Settings Modal Overlay

**Date:** 2026-02-17
**By:** Solaire (Architecture Review Ceremony)
**Participants:** Firekeeper, Andre

## Context

Currently themes are cycled with T key (Ocean → Heist → Sunset → HighContrast → wrap). SettingsScreen exists as a full screen replacement. LondoSpark wants more themes, a settings menu with easy choosing and previewing, and a BackgroundWidget modal overlay.

## Decision

**SettingsScreen becomes a modal overlay, not a separate tab replacement.** Theme selection gets a live preview strip.

### Architecture

1. **Modal Overlay Pattern:**
   - When S is pressed, render SettingsScreen as a semi-transparent overlay on top of the current screen content
   - The BackgroundPanelWidget already provides colored backgrounds; extend or create `ModalOverlayWidget` that dims the background and renders settings content in a centered panel
   - Escape dismisses the overlay and returns to the underlying screen (already works via existing key binding)

2. **Theme Preview:**
   - In the Settings modal, theme selection shows a 3-line preview strip for each theme:
     ```
     ┌─ Ocean ──────────────────┐
     │  ⚡ Dashboard   👥 Roster │  ← Rendered in theme colors
     │  ━━━━━━━━━━━━━━━━━━━━━━  │
     └──────────────────────────┘
     ```
   - Moving selection up/down through themes applies the theme immediately (live preview of full UI behind the modal)
   - Pressing Enter confirms selection, Escape reverts to previous theme

3. **T key behavior preserved:**
   - T still cycles themes globally (quick switch for power users)
   - S opens the full settings modal with preview (for browsing/exploring)
   - Both methods persist the selection via `SettingsService.Save()`

4. **Additional Themes (future):**
   - `ThemeManager.ThemeNames` is already an array — adding themes is just extending `GetTheme()`, the color methods, and the array
   - Candidate new themes: `Solarized`, `Dracula`, `Nord`, `Monokai`, `GruvBox`
   - Each theme needs: base theme, accent code, secondary accent, panel bg colors (4 methods × 4 variants = manageable)

### AppState Changes

```csharp
public bool ShowSettingsOverlay { get; set; } = false;
public int PreviewThemeIndex { get; set; } = -1; // -1 means "use SelectedThemeIndex"
```

### Key Binding Changes

- S key: Toggle `ShowSettingsOverlay` instead of navigating to `Screen.Settings`
- Inside overlay: J/K or Up/Down to browse themes with live preview
- Enter: Confirm and close overlay
- Escape: Revert theme and close overlay

## Consequences

- **Firekeeper:** Design the modal overlay layout, theme preview strip
- **Siegmeyer:** Implement `ModalOverlayWidget`, refactor SettingsScreen rendering to overlay mode, add preview logic
- **Andre:** No backend changes

## Risks

- Hex1b may not support true overlay/z-index rendering — if not, fake it by conditionally rendering settings instead of the screen content (current behavior) but styled as a centered card
- Live preview on every keystroke may cause flicker — buffer theme changes and apply on next render cycle
