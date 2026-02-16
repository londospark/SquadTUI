using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class SkillsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var skills = state.Skills ?? SampleData.Skills;
        var listItems = skills.Select(s => $"🔧 {s.Name} — {s.Description}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.SkillSelectedIndex, 0, skills.Count - 1);
        var selected = skills[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        var members = state.Members ?? SampleData.Members;
        var relatedMembers = GetRelatedMembers(selected.Name, members);
        var confidence = GetConfidenceLevel(selected.Name);

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"  {PanelRenderer.Bold}{acc}🔧 Installed Skills{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.SkillSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {PanelRenderer.Bold}{acc}🔧 {selected.Name}{R}"),
                    detail.Text(""),
                    detail.Text($"  \x1b[90mDescription:\x1b[0m {selected.Description}{R}"),
                    detail.Text($"  \x1b[90mConfidence:\x1b[0m  {confidence.Bar} \x1b[1m{confidence.Level}{R}"),
                    detail.Text(""),
                    detail.Text($"  {PanelRenderer.Bold}{acc}👥 Related Members{R}"),
                };

                if (relatedMembers.Count > 0)
                    foreach (var m in relatedMembers)
                        widgets.Add(detail.Text($"  • {m}{R}"));
                else
                    widgets.Add(detail.Text($"  \x1b[90mNo members directly associated{R}"));

                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"  {PanelRenderer.Bold}{acc}📊 Usage{R}"));
                widgets.Add(detail.Text($"  \x1b[90mThis skill is available to all squad members{R}"));
                widgets.Add(detail.Text($"  \x1b[90mand can be invoked during task execution.{R}"));

                return widgets.ToArray();
            }).FillWidth(2).FillHeight(),
        ]).Fill();
    }

    private static List<string> GetRelatedMembers(string skillName, IReadOnlyList<Models.SquadMember> members)
    {
        return skillName switch
        {
            "code-review" => members.Where(m => m.Role.Contains("Dev") || m.Role.Contains("Lead")).Select(m => m.Name).ToList(),
            "testing" => members.Where(m => m.Role.Contains("Test")).Select(m => m.Name).ToList(),
            "documentation" => members.Where(m => m.Role.Contains("Scribe")).Select(m => m.Name).ToList(),
            "refactoring" => members.Where(m => m.Role.Contains("Dev")).Select(m => m.Name).ToList(),
            "debugging" => members.Where(m => m.Role.Contains("Dev") || m.Role.Contains("Test")).Select(m => m.Name).ToList(),
            _ => []
        };
    }

    private static (string Bar, string Level) GetConfidenceLevel(string skillName)
    {
        return skillName switch
        {
            "code-review" => ("████████░░", "High"),
            "testing" => ("███████░░░", "High"),
            "documentation" => ("██████░░░░", "Medium"),
            "refactoring" => ("█████░░░░░", "Medium"),
            "debugging" => ("████████░░", "High"),
            _ => ("████░░░░░░", "Medium")
        };
    }
}
