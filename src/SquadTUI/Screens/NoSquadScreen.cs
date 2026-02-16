using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

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

        var w = 56; // dialog inner width
        var pad = new string(' ', 2);
        var bar = new string('─', w);
        var innerBar = new string('━', w - 4);

        return v.HStack(outer =>
        [
            outer.Text("").Fill(),
            outer.VStack(center =>
            [
                center.Text("").Fill(),

                // Top border
                center.Text($"  {D}{sec}┌{bar}┐{R}"),

                // Title
                center.Text($"  {D}{sec}│{R}  {B}{acc}☀️  Welcome to SquadTUI{R}{new string(' ', w - 24)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}  {D}{sec}{innerBar}{R}  {D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // What is SquadTUI
                center.Text($"  {D}{sec}│{R}{pad}{B}{acc}What is SquadTUI?{R}{new string(' ', w - 19)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}SquadTUI is a terminal dashboard for managing{R}{new string(' ', w - 48)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}AI agent squads. View activity, inspect members,{R}{new string(' ', w - 51)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}track decisions, and monitor your team — all from{R}{new string(' ', w - 52)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}the terminal.{R}{new string(' ', w - 15)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // What is a Squad
                center.Text($"  {D}{sec}│{R}{pad}{B}{acc}What is a Squad?{R}{new string(' ', w - 18)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}A squad is a team of AI agents defined in an{R}{new string(' ', w - 47)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}{B}.ai-team/{R}{D} directory in your repo. Each agent has{R}{new string(' ', w - 49)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}a charter, history, and role.{R}{new string(' ', w - 30)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // Warning
                center.Text($"  {D}{sec}│{R}{pad}\x1b[93m⚠  No squad detected in this directory.{R}{new string(' ', w - 42)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // Getting started
                center.Text($"  {D}{sec}│{R}{pad}{B}{acc}Getting Started{R}{new string(' ', w - 17)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}Create a squad using the CLI:{R}{new string(' ', w - 30)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}  {acc}npx github:bradygaster/squad{R}{new string(' ', w - 32)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{pad}{D}Or press{R} {B}C{R} {D}below to create a basic structure.{R}{new string(' ', w - 48)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // Link
                center.Text($"  {D}{sec}│{R}{pad}{D}Learn more:{R} \x1b[4m{acc}https://github.com/bradygaster/squad{R}{new string(' ', w - 50)}{D}{sec}│{R}"),
                center.Text($"  {D}{sec}│{R}{new string(' ', w)}{D}{sec}│{R}"),

                // Divider before keys
                center.Text($"  {D}{sec}│{R}  {D}{sec}{innerBar}{R}  {D}{sec}│{R}"),

                // Key bindings
                center.Text($"  {D}{sec}│{R}{pad}{B}C{R} {D}Create basic squad structure{R}      {B}Q{R} {D}Quit{R}{new string(' ', w - 42)}{D}{sec}│{R}"),

                // Bottom border
                center.Text($"  {D}{sec}└{bar}┘{R}"),

                center.Text("").Fill(),
            ]),
            outer.Text("").Fill(),
        ]).Fill();
    }
}
