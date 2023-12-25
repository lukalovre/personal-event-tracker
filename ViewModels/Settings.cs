using System.IO;
using Newtonsoft.Json;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class Settings
{
    private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Include,
        Formatting = Formatting.Indented,
    };

    private static Settings? _instance;
    public string DatasourcePath { get; set; } = string.Empty;

    public static Settings Instance
    {
        get
        {
            _instance ??= Load();

            return _instance;
        }
    }

    private static Settings Load()
    {
        if (!File.Exists(Paths.SettingsFilePath))
        {
            File.WriteAllText(Paths.SettingsFilePath, JsonConvert.SerializeObject(new Settings(), _jsonSettings));
        }

        var settingsText = File.ReadAllText(Paths.SettingsFilePath);
        return JsonConvert.DeserializeObject<Settings>(settingsText) ?? new Settings();
    }
}