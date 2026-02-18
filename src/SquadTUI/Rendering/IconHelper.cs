using SquadTUI.Models;

namespace SquadTUI.Rendering;

/// <summary>
/// Conditionally returns emoji or ASCII symbol based on user preference.
/// </summary>
public static class IconHelper
{
    /// <summary>Returns emoji when showEmoji is true, otherwise returns the ASCII alternative.</summary>
    public static string Icon(string emoji, string ascii, bool showEmoji) =>
        showEmoji ? emoji : ascii;
}
