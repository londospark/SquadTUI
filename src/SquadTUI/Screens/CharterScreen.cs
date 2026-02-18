using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    private const int VisibleLines = 20;

    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");

        // Render all markdown lines, then slice for scrolling
        var allWidgets = MarkdownRenderer.Render(v, charter);
        var totalLines = allWidgets.Length;
        var maxScroll = Math.Max(0, totalLines - VisibleLines);
        state.CharterScrollOffset = Math.Clamp(state.CharterScrollOffset, 0, maxScroll);

        var visibleWidgets = allWidgets
            .Skip(state.CharterScrollOffset)
            .Take(VisibleLines)
            .ToArray();

        var scrollInfo = totalLines > VisibleLines
            ? $"  {t.D}[{state.CharterScrollOffset + 1}–{Math.Min(state.CharterScrollOffset + VisibleLines, totalLines)}/{totalLines}] j/k to scroll{t.R}"
            : "";

        return new BackgroundPanelWidget(t.PanelBg, v.VStack(inner =>
        [
            inner.Text(t.SectionHeader("📜", "▪", $"Charter — {memberName}")),
            inner.Text(t.Separator(44)),
            inner.Text(""),
            inner.VStack(scroll =>
            [
                ..visibleWidgets,
            ]).Fill(),
            inner.Text(""),
            inner.Text($"  {t.D}Esc Back{t.R}{scrollInfo}"),
        ]).Fill());
    }
}