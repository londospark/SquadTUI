using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class SkillsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var skills = state.Skills ?? SampleData.Skills;
        var listItems = skills.Select(s => $"🔧 {s.Name} — {s.Description}").ToList() as IReadOnlyList<string>;

        return v.Border(b =>
        [
            b.List(listItems)
                .OnSelectionChanged(e => { state.SkillSelectedIndex = e.SelectedIndex; })
                .Fill()
        ]).Title("🔧 Installed Skills").Fill();
    }
}
