using SquadTUI.Models;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Rendering;

/// <summary>
/// Shared status badge formatting for member status and task status.
/// Previously duplicated across DashboardScreen, RosterScreen, and MemberDetailScreen.
/// </summary>
public static class StatusBadges
{
    public static string Member(MemberStatus status, bool showEmoji) => status switch
    {
        MemberStatus.Active => Icon("✅", "[+]", showEmoji),
        MemberStatus.Idle => Icon("🟡", "[~]", showEmoji),
        MemberStatus.Working => Icon("🔵", "[>]", showEmoji),
        MemberStatus.Offline => Icon("⚫", "[-]", showEmoji),
        _ => Icon("⚪", "[ ]", showEmoji)
    };

    public static string Task(SquadTaskStatus status, bool showEmoji) => status switch
    {
        SquadTaskStatus.InProgress => Icon("🔄", ">", showEmoji),
        SquadTaskStatus.Done => Icon("✅", "+", showEmoji),
        SquadTaskStatus.Pending => Icon("⏳", "~", showEmoji),
        SquadTaskStatus.Blocked => Icon("🚫", "-", showEmoji),
        _ => Icon("⚪", "[ ]", showEmoji)
    };
}
