using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    private const int VisibleLines = 20;

    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var em = state.Settings.ShowEmoji;

        // Render all markdown lines, then slice for scrolling
        var allWidgets = MarkdownRenderer.Render(v, charter);
        var totalLines = allWidgets.Length;
        var maxScroll = Math.Max(0, totalLines - VisibleLines);
        state.CharterScrollOffset = Math.Clamp(state.CharterScrollOffset, 0, maxScroll);

        var visibleWidgets = allWidgets
            .Skip(state.CharterScrollOffset)
            .Take(VisibleLines)
            .ToArray();

        // Scroll indicator
        var scrollInfo = totalLines > VisibleLines
            ? $"  {D}[{state.CharterScrollOffset + 1}–{Math.Min(state.CharterScrollOffset + VisibleLines, totalLines)}/{totalLines}] j/k to scroll{R}"
            : "";

        return new BackgroundPanelWidget(panelBg, v.VStack(inner =>
        [
            inner.Text($"  {B}{acc}{Icon("📜", "▪", em)} Charter — {memberName}{R}"),
            inner.Text($"  {D}{sec}{new string('━', 44)}{R}"),
            inner.Text(""),
            inner.VStack(scroll =>
            [
                ..visibleWidgets,
            ]).Fill(),
            inner.Text(""),
            inner.Text($"  {D}Esc Back{R}{scrollInfo}"),
        ]).Fill());
    }
}
