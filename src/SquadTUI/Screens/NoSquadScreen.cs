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

        return v.VStack(inner =>
        [
            inner.Text(""),
            inner.Text(""),
            inner.Text($"  {B}{acc}☀️  Welcome to SquadTUI{R}"),
            inner.Text($"  {D}{sec}{new string('━', 36)}{R}"),
            inner.Text(""),
            inner.Text($"  \x1b[93m⚠  No squad detected in this directory.{R}"),
            inner.Text(""),
            inner.Text($"  {D}SquadTUI needs an{R} {B}.ai-team/{R} {D}directory to work with.{R}"),
            inner.Text($"  {D}You can create one using the squad CLI:{R}"),
            inner.Text(""),
            inner.Text($"  {acc}  npx github:bradygaster/squad{R}"),
            inner.Text(""),
            inner.Text($"  {D}Or create the structure manually:{R}"),
            inner.Text(""),
            inner.Text($"  {acc}  mkdir .ai-team{R}"),
            inner.Text($"  {acc}  mkdir .ai-team/agents{R}"),
            inner.Text($"  {acc}  echo \"# Team\" > .ai-team/team.md{R}"),
            inner.Text(""),
            inner.Text($"  {D}See:{R} \x1b[4m{acc}https://github.com/bradygaster/squad{R}"),
            inner.Text(""),
            inner.Text($"  {B}C{R} {D}Create basic squad structure{R}    {B}Q{R} {D}Quit{R}"),
        ]).Fill();
    }
}
