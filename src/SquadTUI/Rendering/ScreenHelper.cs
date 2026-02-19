using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Rendering;

/// <summary>
/// Reusable widget patterns shared across multiple screens.
/// </summary>
public static class ScreenHelper
{
    /// <summary>
    /// Renders the standard empty-state message used when a data collection is empty.
    /// </summary>
    public static Hex1bWidget EmptyState<T>(WidgetContext<T> ctx, string message) where T : Hex1bWidget =>
        ctx.VStack(empty =>
        [
            empty.Text(""),
            empty.Text($"  {PanelRenderer.Dim}{message}{PanelRenderer.Reset}"),
        ]).Fill();

    /// <summary>
    /// Renders a standard list-detail layout with BackgroundPanelWidget wrapping.
    /// Left panel gets FillWidth(listWeight), right panel gets FillWidth(detailWeight).
    /// </summary>
    public static Hex1bWidget ListDetailLayout<T>(
        WidgetContext<T> ctx,
        ThemeContext theme,
        Func<WidgetContext<VStackWidget>, Hex1bWidget[]> listContent,
        Func<WidgetContext<VStackWidget>, Hex1bWidget[]> detailContent,
        int listWeight = 1,
        int detailWeight = 2) where T : Hex1bWidget =>
        ctx.HStack(h =>
        [
            new BackgroundPanelWidget(theme.PanelBg, h.VStack(left =>
                listContent(left)
            ).FillWidth(listWeight).FillHeight()),
            new BackgroundPanelWidget(theme.DetailBg, h.VStack(right =>
                detailContent(right)
            ).FillWidth(detailWeight).FillHeight()),
        ]).Fill();
}
