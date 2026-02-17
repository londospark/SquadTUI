namespace SquadTUI.Models;

public class AppSettings
{
    public string ThemeName { get; set; } = "Ocean";
    public bool VimBindings { get; set; } = true;
    public bool MouseEnabled { get; set; } = true;
    public bool ShowEmoji { get; set; } = true;
    public bool MarkdownRendering { get; set; } = true;
    public string DefaultScreen { get; set; } = "Dashboard";
}
