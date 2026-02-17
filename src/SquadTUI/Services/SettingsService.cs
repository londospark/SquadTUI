using System.Text.Json;
using SquadTUI.Models;

namespace SquadTUI.Services;

public class SettingsService
{
    private static readonly string DefaultConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "squadtui");
    private static readonly string DefaultConfigPath = Path.Combine(DefaultConfigDir, "settings.json");

    public static AppSettings Load(IFileLocationService? fileLocations = null)
    {
        var configPath = fileLocations?.GetSettingsFilePath() ?? DefaultConfigPath;
        try
        {
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { }
        return new AppSettings();
    }

    public static void Save(AppSettings settings, IFileLocationService? fileLocations = null)
    {
        var configDir = fileLocations?.GetSettingsDirectory() ?? DefaultConfigDir;
        var configPath = fileLocations?.GetSettingsFilePath() ?? DefaultConfigPath;
        try
        {
            Directory.CreateDirectory(configDir);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }
        catch { }
    }
}
