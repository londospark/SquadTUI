# Decision: Use BindCI helper for case-insensitive key bindings

**Author:** Firekeeper (UX/Design)
**Date:** 2026-02-17
**Status:** Implemented

## Context

Hex1b treats `Key(Hex1bKey.Q)` as lowercase `q`. To handle uppercase `Q` (Shift or Caps Lock), every key binding must be duplicated with `Shift().Key(Hex1bKey.Q)`. This caused ~160 lines of exact copy-paste in AppLayout.cs across three binding methods.

## Decision

Introduce `BindCI(InputBindingsBuilder keys, Hex1bKey key, Action action, string label)` — a private helper in AppLayout that registers both lowercase and Shift+Key variants in one call. All letter-key bindings now use `BindCI()` instead of manual duplication.

Non-letter keys (Escape, Enter, F1, Tab, arrows) don't need this — they're case-insensitive by nature.

## Consequences

- **Net deletion:** ~160 lines removed from AppLayout.cs
- **Convention:** All future letter-key bindings in AppLayout should use `BindCI()` 
- **No behavior change:** Exact same keys are bound, just without copy-paste
