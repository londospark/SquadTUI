using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class RosterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members ?? SampleData.Members;
        var listItems = members.Select(m => $"{GetStatusBadge(m.Status)} {m.Name} — {m.Role}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
        var selected = members[selectedIdx];

        return v.HStack(h =>
        [
            h.Border(b =>
            [
                b.List(listItems)
                    .OnSelectionChanged(e => { state.RosterSelectedIndex = e.SelectedIndex; })
                    .OnItemActivated(e =>
                    {
                        state.SelectedMemberName = members[e.ActivatedIndex].Name;
                        state.CurrentScreen = Screen.MemberDetail;
                    })
                    .Fill()
            ]).Title("👥 Team Roster").FillWidth(2).FillHeight(),

            h.Border(b =>
            [
                b.Text($"  Name:   {selected.Name}"),
                b.Text($"  Role:   {selected.Role}"),
                b.Text($"  Status: {GetStatusBadge(selected.Status)} {selected.Status}"),
                b.Text($"  Task:   {selected.CurrentTask ?? "None"}"),
                b.Text(""),
                b.Text("  Press Enter to view details"),
            ]).Title("Preview").Fill(),
        ]).Fill();
    }

    private static string GetStatusBadge(Models.MemberStatus status) => status switch
    {
        Models.MemberStatus.Active => "✅",
        Models.MemberStatus.Idle => "🟡",
        Models.MemberStatus.Working => "🔵",
        Models.MemberStatus.Offline => "⚫",
        _ => "⚪"
    };
}
