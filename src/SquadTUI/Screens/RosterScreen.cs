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
                b.Text($"  \x1b[90mName:\x1b[0m   \x1b[1m{selected.Name}\x1b[0m"),
                b.Text($"  \x1b[90mRole:\x1b[0m   {selected.Role}"),
                b.Text($"  \x1b[90mStatus:\x1b[0m {GetStatusBadge(selected.Status)} {selected.Status}"),
                b.Text($"  \x1b[90mTask:\x1b[0m   {selected.CurrentTask ?? "\x1b[90mNone\x1b[0m"}"),
                b.Text(""),
                b.Text("\x1b[90m  Press Enter to view details\x1b[0m"),
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
