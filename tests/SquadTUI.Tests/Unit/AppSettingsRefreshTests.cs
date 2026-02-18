using System.Text.Json;
using SquadTUI.Models;

namespace SquadTUI.Tests.Unit;

/// <summary>
/// Tests for the RefreshIntervalSeconds property on AppSettings —
/// verifying default value, serialization round-trip, and boundary values.
/// </summary>
public class AppSettingsRefreshTests
{
    [Fact]
    public void AppSettings_HasRefreshIntervalSeconds()
    {
        var settings = new AppSettings();
        Assert.Equal(30, settings.RefreshIntervalSeconds);
    }

    [Fact]
    public void AppSettings_RefreshIntervalSeconds_Serializes()
    {
        var settings = new AppSettings { RefreshIntervalSeconds = 60 };
        var json = JsonSerializer.Serialize(settings);
        var deserialized = JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(deserialized);
        Assert.Equal(60, deserialized.RefreshIntervalSeconds);
    }

    [Fact]
    public void AppSettings_RefreshIntervalSeconds_RoundTripsAllValidValues()
    {
        var validIntervals = new[] { 15, 30, 60, 120 };
        foreach (var interval in validIntervals)
        {
            var settings = new AppSettings { RefreshIntervalSeconds = interval };
            var json = JsonSerializer.Serialize(settings);
            var deserialized = JsonSerializer.Deserialize<AppSettings>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(interval, deserialized.RefreshIntervalSeconds);
        }
    }

    [Fact]
    public void AppSettings_RefreshIntervalSeconds_DefaultSerializesToJson()
    {
        var settings = new AppSettings();
        var json = JsonSerializer.Serialize(settings);

        Assert.Contains("RefreshIntervalSeconds", json);
        Assert.Contains("30", json);
    }

    [Fact]
    public void AppSettings_RefreshIntervalSeconds_DeserializesFromMissingProperty()
    {
        // When loading old settings files that predate RefreshIntervalSeconds,
        // the property should default to 30
        var json = """{"ThemeName":"Ocean","VimBindings":true}""";
        var settings = JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(settings);
        Assert.Equal(30, settings.RefreshIntervalSeconds);
    }

    [Fact]
    public void AppSettings_RefreshIntervalSeconds_IndependentOfOtherProperties()
    {
        var settings = new AppSettings
        {
            RefreshIntervalSeconds = 120,
            ThemeName = "Heist",
            VimBindings = false
        };

        Assert.Equal(120, settings.RefreshIntervalSeconds);
        Assert.Equal("Heist", settings.ThemeName);
        Assert.False(settings.VimBindings);
    }
}
