using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class NoSquadScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var hlBg = ThemeManager.GetHighlightBg(state.SelectedThemeIndex);
        var hlFg = ThemeManager.GetHighlightFg(state.SelectedThemeIndex);
        var em = state.Settings.ShowEmoji;

        return v.HStack(outer =>
        [
            outer.Text("").Fill(),
            new BackgroundPanelWidget(panelBg, outer.VStack(center =>
            [
                center.Text("").Fill(),

                // Title with themed highlight bar
                center.Text($"    {hlBg}{hlFg}  {Icon("☀️", "▸", em)}  Welcome to SquadTUI  {R}"),
                center.Text(""),
                center.Text(""),

                // What is SquadTUI
                center.Text($"    {B}{acc}▌{R} {B}{acc}What is SquadTUI?{R}"),
                center.Text(""),
                center.Text($"    {D}SquadTUI is a terminal dashboard for managing AI agent{R}"),
                center.Text($"    {D}squads. View activity, inspect members, track decisions,{R}"),
                center.Text($"    {D}and monitor your team — all from the terminal.{R}"),
                center.Text(""),
                center.Text(""),

                // What is a Squad
                center.Text($"    {B}{acc}▌{R} {B}{acc}What is a Squad?{R}"),
                center.Text(""),
                center.Text($"    {D}A squad is a team of AI agents defined in an{R}"),
                center.Text($"    {D}{B}.ai-team/{R}{D} directory in your repo. Each agent has{R}"),
                center.Text($"    {D}a charter, history, and role.{R}"),
                center.Text(""),
                center.Text(""),

                // Warning
                center.Text($"    \x1b[93m{Icon("⚠", "!", em)}  No squad detected in this directory.{R}"),
                center.Text(""),
                center.Text(""),

                // Getting started
                center.Text($"    {B}{acc}▌{R} {B}{acc}Getting Started{R}"),
                center.Text(""),
                center.Text($"    {D}Create a squad using the CLI:{R}"),
                center.Text(""),
                center.Text($"      {acc}npx github:bradygaster/squad{R}"),
                center.Text(""),
                center.Text($"    {D}Or press{R} {B}C{R} {D}to create a basic squad structure.{R}"),
                center.Text(""),
                center.Text($"    {D}Learn more:{R}  \x1b[4m{acc}https://github.com/bradygaster/squad{R}"),
                center.Text(""),
                center.Text(""),

                // Key bindings section
                center.Text($"    {sec}{new string('━', 44)}{R}"),
                center.Text(""),
                center.Text($"    {B}C{R}  {D}Create basic squad structure{R}        {B}Q{R}  {D}Quit{R}"),

                center.Text("").Fill(),
            ])),
            outer.Text("").Fill(),
        ]).Fill();
    }
}
