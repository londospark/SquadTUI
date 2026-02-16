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
        var listItems = skills.Select(s => $"  🔧 {s.Name} — {s.Description}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.SkillSelectedIndex, 0, skills.Count - 1);
        var selected = skills[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;

        var members = state.Members ?? SampleData.Members;
        var relatedMembers = GetRelatedMembers(selected.Name, members);
        var confidence = GetConfidenceLevel(selected.Name);

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"  {B}{acc}🔧 Installed Skills{R}"),
                left.Text($"  {D}{sec}{new string('━', 30)}{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.SkillSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {B}{acc}🔧 {selected.Name}{R}"),
                    detail.Text($"  {D}{sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {D}Description:{R}  {selected.Description}{R}"),
                    detail.Text($"  {D}Confidence:{R}   {confidence.Bar} {B}{confidence.Level}{R}"),
                    detail.Text(""),
                    detail.Text($"  {D}{sec}{new string('━', 36)}{R}"),
                    detail.Text($"  {B}{acc}👥 Related Members{R}"),
                };

                if (relatedMembers.Count > 0)
                    foreach (var m in relatedMembers)
                        widgets.Add(detail.Text($"    • {m}{R}"));
                else
                    widgets.Add(detail.Text($"  {D}No members directly associated{R}"));

                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"  {D}{sec}{new string('━', 36)}{R}"));
                widgets.Add(detail.Text($"  {B}{acc}📊 Usage{R}"));
                widgets.Add(detail.Text($"  {D}This skill is available to all squad members{R}"));
                widgets.Add(detail.Text($"  {D}and can be invoked during task execution.{R}"));

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
            "code-review" => ("\x1b[32m████████\x1b[90m░░\x1b[0m", "High"),
            "testing" => ("\x1b[32m███████\x1b[90m░░░\x1b[0m", "High"),
            "documentation" => ("\x1b[33m██████\x1b[90m░░░░\x1b[0m", "Medium"),
            "refactoring" => ("\x1b[33m█████\x1b[90m░░░░░\x1b[0m", "Medium"),
            "debugging" => ("\x1b[32m████████\x1b[90m░░\x1b[0m", "High"),
            _ => ("\x1b[33m████\x1b[90m░░░░░░\x1b[0m", "Medium")
        };
    }
}
