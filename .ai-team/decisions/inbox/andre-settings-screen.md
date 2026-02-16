# SettingsScreen — dynamic options parameter

**Date:** 2026-02-17
**By:** Andre

**What:** `SettingsScreen.Render()` accepts `dynamic options` as its 4th parameter to enable runtime theme switching. This mirrors how `Program.cs` already mutates `options.Theme` in the T-key handler. Using `dynamic` avoids needing to know the exact Hex1b options type at compile time.

**Why:** The settings screen needs to change the active theme immediately when the user selects a new one. The `options` object from `WithHex1bApp` is the only way to do this, and its type is inferred. Using `dynamic` keeps the screen decoupled from Hex1b internals.

**Also:** `ListItemActivatedEventArgs.ActivatedIndex` is the correct property for the item activation callback (not `SelectedIndex`).
