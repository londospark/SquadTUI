using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class SkillsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var skills = state.Skills.GetOrEmpty();
        if (skills.Count == 0)
            return ScreenHelper.EmptyState(v, "No skills found. Ensure your .squad/ directory contains skill definitions.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var listItems = skills.Select(s => $"  {Icon("🔧", "◇", t.Em)} {s.Name} — {s.Description}").ToList() as IReadOnlyList<string>;
        var selectedIdx = Math.Clamp(state.SkillSelectedIndex, 0, skills.Count - 1);
        var selected = skills[selectedIdx];

        var members = state.Members.GetOrEmpty();
        var relatedMembers = GetRelatedMembers(selected.Name, members);
        var confidenceValue = ProgressBarRenderer.ConfidenceToFloat(selected.Confidence);
        var confidence = ProgressBarRenderer.Render(confidenceValue);

        return ScreenHelper.ListDetailLayout(v, t,
            listContent: left =>
            [
                left.Text(t.SectionHeader("🔧", "◇", " Installed Skills ")),
                left.Text($"  {t.D}Available capabilities for your squad{t.R}"),
                left.Text(""),
                left.Text(t.Separator(30)),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.SkillSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ],
            detailContent: detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text(""),
                    detail.Text(t.SectionHeader("🔧", "◇", $" {selected.Name} ")),
                    detail.Text(""),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text($"    {t.D}Description:{t.R}  {selected.Description}{t.R}"),
                    detail.Text($"    {t.D}Confidence:{t.R}   {confidence.Bar} {t.B}{confidence.Label}{t.R}"),
                    detail.Text(""),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text(t.SectionHeader("👥", "◆", "Related Members")),
                    detail.Text(""),
                };

                if (relatedMembers.Count > 0)
                    foreach (var m in relatedMembers)
                        widgets.Add(detail.Text($"    • {m}{t.R}"));
                else
                    widgets.Add(detail.Text($"    {t.D}No members directly associated{t.R}"));

                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.Separator()));
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.SectionHeader("📊", "▪", "Usage")));
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"    {t.D}This skill is available to all squad members{t.R}"));
                widgets.Add(detail.Text($"    {t.D}and can be invoked during task execution.{t.R}"));

                return widgets.ToArray();
            });
    }

    private static List<string> GetRelatedMembers(string skillName, IReadOnlyList<Models.SquadMember> members) =>
        skillName switch
        {
            "code-review" => (from m in members where m.Role.Contains("Dev") || m.Role.Contains("Lead") select m.Name).ToList(),
            "testing" => (from m in members where m.Role.Contains("Test") select m.Name).ToList(),
            "documentation" => (from m in members where m.Role.Contains("Scribe") select m.Name).ToList(),
            "refactoring" => (from m in members where m.Role.Contains("Dev") select m.Name).ToList(),
            "debugging" => (from m in members where m.Role.Contains("Dev") || m.Role.Contains("Test") select m.Name).ToList(),
            _ => []
        };
}
