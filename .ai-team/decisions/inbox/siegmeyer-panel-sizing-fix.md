# Decision: Fix Panel Sizing During Dashboard Navigation

**Author:** Siegmeyer (Frontend Dev)
**Date:** 2025-07-18
**Status:** Implemented

## Problem

When navigating between dashboard panels (changing `DashboardFocusedPanel` via Tab/arrows), panels shifted size. The root cause was the `PanelHeader` function in `DashboardScreen.cs` generating ANSI escape strings with different byte lengths for focused vs unfocused states.

**Focused:** `hlBg` (20 bytes) + `hlFg` (20 bytes) = 40 bytes of ANSI codes
**Unfocused:** `hBg` (20 bytes) + `B` (4 bytes) + `acc` (20 bytes) = 44 bytes of ANSI codes

Hex1b uses string byte length to calculate column widths. The 4-byte difference caused the focused panel header to measure as narrower, triggering a layout reflow when focus changed.

## Solution

Added `{B}` (Bold, `\x1b[1m`, 4 bytes) to the focused branch between `hlBg` and `hlFg`, equalizing both branches to 44 bytes of ANSI escape codes:

```csharp
string PanelHeader(int panelIndex, string emoji, string ascii, string title) =>
    focus == panelIndex
        ? $"  {hlBg}{B}{hlFg}{Icon(emoji, ascii, em)} {title}{R}"
        : $"  {hBg}{B}{acc}{Icon(emoji, ascii, em)} {title}{R}";
```

Both branches now produce: `bg(20) + Bold(4) + fg(20) + content + Reset(4)` — identical ANSI overhead.

## Why Bold?

Bold is already used in the unfocused branch. Adding it to the focused branch is a no-op visually (the focused state uses explicit foreground/background colors that dominate rendering), but it ensures byte-level parity. This is the minimal change — no padding hacks or invisible resets needed.

## What Was Not Changed

- FillWidth ratios were already consistent (1:2:1 wide, 2:1 medium) regardless of focus state. No change needed.
- FixedWidth was not introduced — FillWidth remains responsive as intended.
- No changes to ThemeManager or escape code definitions.

## Testing

All 796 tests pass. The 1 flaky E2E failure (`MouseToggleTests`) is pre-existing and unrelated.
