using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class NoSquadScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);

        return v.HStack(outer =>
        [
            outer.Text("").Fill(),
            new BackgroundPanelWidget(t.PanelBg, outer.VStack(center =>
            [
                center.Text("").Fill(),

                center.Text($"    {t.HlBg}{t.HlFg}  {Icon("☀️", "▸", t.Em)}  Welcome to SquadTUI  {t.R}"),
                center.Text(""),
                center.Text(""),

                center.Text($"    {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}What is SquadTUI?{t.R}"),
                center.Text(""),
                center.Text($"    {t.D}SquadTUI is a terminal dashboard for managing AI agent{t.R}"),
                center.Text($"    {t.D}squads. View activity, inspect members, track decisions,{t.R}"),
                center.Text($"    {t.D}and monitor your team — all from the terminal.{t.R}"),
                center.Text(""),
                center.Text(""),

                center.Text($"    {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}What is a Squad?{t.R}"),
                center.Text(""),
                center.Text($"    {t.D}A squad is a team of AI agents defined in an{t.R}"),
                center.Text($"    {t.D}{t.B}.ai-team/{t.R}{t.D} directory in your repo. Each agent has{t.R}"),
                center.Text($"    {t.D}a charter, history, and role.{t.R}"),
                center.Text(""),
                center.Text(""),

                center.Text($"    \x1b[93m{Icon("⚠", "!", t.Em)}  No squad detected in this directory.{t.R}"),
                center.Text(""),
                center.Text(""),

                center.Text($"    {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}Getting Started{t.R}"),
                center.Text(""),
                center.Text($"    {t.D}Create a squad using the CLI:{t.R}"),
                center.Text(""),
                center.Text($"      {t.Acc}npx github:bradygaster/squad{t.R}"),
                center.Text(""),
                center.Text($"    {t.D}Or press{t.R} {t.B}C{t.R} {t.D}to create a basic squad structure.{t.R}"),
                center.Text(""),
                center.Text($"    {t.D}Learn more:{t.R}  \x1b[4m{t.Acc}https://github.com/bradygaster/squad{t.R}"),
                center.Text(""),
                center.Text(""),

                center.Text($"    {t.Sec}{new string('━', 44)}{t.R}"),
                center.Text(""),
                center.Text($"    {t.B}C{t.R}  {t.D}Create basic squad structure{t.R}        {t.B}Q{t.R}  {t.D}Quit{t.R}"),

                center.Text("").Fill(),
            ])),
            outer.Text("").Fill(),
        ]).Fill();
    }
}
