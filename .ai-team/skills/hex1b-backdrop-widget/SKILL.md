# Hex1b BackdropWidget

> Source: `mitchdenny/hex1b` — `src/Hex1b/Widgets/BackdropWidget.cs` & `src/Hex1b/Nodes/BackdropNode.cs`

## Overview
`BackdropWidget` fills its entire available space and intercepts all input. Used as a modal backdrop to prevent interaction with layers below. Can optionally display a background color (for dimming effects) and trigger a callback when clicked (for "click-away to dismiss" behavior).

**Namespace:** `Hex1b.Widgets`  
**Base:** `Hex1bWidget` (sealed record)  
**Available since:** Hex1b 0.87.0  
**Backing node:** `BackdropNode`

## API

### Constructor
```csharp
new BackdropWidget(Hex1bWidget? Child = null)
```

### Extension Methods (on WidgetContext)
```csharp
ctx.Backdrop()                    // Empty backdrop (no child)
ctx.Backdrop(child)               // Backdrop wrapping a child widget
```

### Fluent Methods
```csharp
.Transparent()                    // BackdropStyle.Transparent — base layer shows through, clicks still captured
.Opaque(Hex1bColor color)         // BackdropStyle.Opaque — solid color fills entire area
.WithBackground(Hex1bColor color) // Same as Opaque — sets BackgroundColor + Style=Opaque
.WithLayerId(string id)           // Layer identifier for popup stack management
.OnClickAway(Action handler)      // Simple click-away dismiss
.OnClickAway(Func<Task> handler)  // Async click-away
.OnClickAway(Action<BackdropClickedEventArgs> handler) // Rich event args
.OnClickAway(Func<BackdropClickedEventArgs, Task> handler)
```

### BackdropStyle Enum
```csharp
BackdropStyle.Transparent  // No background — base layer shows through
BackdropStyle.Opaque       // Solid background color fill
```

### BackdropClickedEventArgs
```csharp
BackdropWidget Widget   // The backdrop that was clicked
int X                   // Click X coordinate
int Y                   // Click Y coordinate
string? LayerId         // Layer identifier (for popup stack integration)
bool Handled            // Whether the click was handled
```

## Rendering Behavior (from BackdropNode source)

The `BackdropNode`:
1. **MeasureCore**: Fills ALL available space (`constraints.MaxWidth × constraints.MaxHeight`). Also measures child if present.
2. **ArrangeCore**: Child is **centered** within the backdrop bounds.
3. **Render**: If `Style == Opaque && BackgroundColor != null`, fills entire bounds with background color using ANSI codes. Child renders on top via `context.RenderChild()`.
4. **IsFocusable**: Always `true` — captures all clicks
5. **HandleMouseClick**: Always returns `InputResult.Handled` — prevents click-through to lower layers
6. **Input bindings**: Escape triggers click-away if handler set; left-click on backdrop (not child) triggers click-away

## Usage Patterns

### Panel with solid background
```csharp
ctx.Backdrop(
    ctx.VStack(v => [
        v.Text("Panel Title").Bold(),
        v.Text("Panel content here")
    ])
).Opaque(Hex1bColor.FromRgb(22, 28, 38))
```

### For panel shading (non-modal — use with caution)
BackdropWidget is designed primarily for modals and overlays. For non-modal panel backgrounds, consider:
- **ThemePanel** with `GlobalTheme.BackgroundColor` for scoped theme backgrounds
- **EffectPanel** for post-processing cell backgrounds
- BackdropWidget with no click-away handler for a simple area fill (but note it intercepts ALL input)

### Click-away modal dismissal
```csharp
ctx.Backdrop(
    ctx.VStack(v => [
        v.Text("Modal content")
    ])
).Opaque(Hex1bColor.FromRgb(0, 0, 0))
 .OnClickAway(() => DismissModal())
```

## Key Points
- BackdropWidget fills the ENTIRE allocated area — not just around the child
- The child is CENTERED within the backdrop
- `.Opaque(color)` and `.WithBackground(color)` are equivalent
- BackdropWidget is ALWAYS focusable and ALWAYS consumes mouse clicks
- Escape key triggers click-away handler if set
- Best used for modals/overlays, NOT for simple panel background shading
- For panel backgrounds, ThemePanel with BackgroundColor is usually the better choice
