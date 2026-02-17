using SquadTUI.Rendering;

namespace SquadTUI.Tests.Unit;

public class IconHelperTests
{
    [Fact]
    public void Icon_ReturnsEmoji_WhenShowEmojiTrue()
    {
        var result = IconHelper.Icon("✅", "[+]", showEmoji: true);
        Assert.Equal("✅", result);
    }

    [Fact]
    public void Icon_ReturnsAscii_WhenShowEmojiFalse()
    {
        var result = IconHelper.Icon("✅", "[+]", showEmoji: false);
        Assert.Equal("[+]", result);
    }

    [Theory]
    [InlineData("👥", "◆")]
    [InlineData("👤", "◆")]
    [InlineData("📋", "▪")]
    [InlineData("📜", "▪")]
    [InlineData("📊", "▪")]
    [InlineData("✅", "[+]")]
    [InlineData("🟡", "[~]")]
    [InlineData("🔵", "[>]")]
    [InlineData("⚫", "[-]")]
    [InlineData("⚪", "[ ]")]
    [InlineData("🔄", ">")]
    [InlineData("⏳", "~")]
    [InlineData("🚫", "-")]
    public void Icon_AllMappings_ReturnCorrectValue(string emoji, string ascii)
    {
        Assert.Equal(emoji, IconHelper.Icon(emoji, ascii, showEmoji: true));
        Assert.Equal(ascii, IconHelper.Icon(emoji, ascii, showEmoji: false));
    }

    [Fact]
    public void Icon_EmptyStrings_HandledCorrectly()
    {
        Assert.Equal("", IconHelper.Icon("", "alt", showEmoji: true));
        Assert.Equal("alt", IconHelper.Icon("", "alt", showEmoji: false));
    }
}
