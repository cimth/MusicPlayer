using System.IO;
using System.Text;
using System.Text.Json;
using Model.DataType;

namespace Model.Service;

public class AppConfigurator
{
    // ==============
    // FIELDS
    // ==============

    // The app's config file is placed under "<App base directory>/config.json" (if existing)
    private static readonly string AppBaseDirectoryPath = Path.GetFullPath(AppContext.BaseDirectory);
    private static readonly string AppConfigFilePath = Path.GetFullPath(Path.Combine(AppBaseDirectoryPath, "config.json"));
    
    private readonly AppConfig _appConfig;
    
    // ==============
    // INITIALIZATION
    // ==============

    public AppConfigurator()
    {
        this._appConfig = this.LoadAppConfig();
    }

    private AppConfig LoadAppConfig()
    {
        // Load app config if existing
        // => The config file (if existing) is placed inside the app's base directory
        AppConfig? appConfig = null;        
        if (File.Exists(AppConfigurator.AppConfigFilePath))
        {
            string configJson = File.ReadAllText(AppConfigurator.AppConfigFilePath);
            appConfig = JsonSerializer.Deserialize<AppConfig>(configJson) ?? throw new InvalidOperationException();
        }

        // Return the loaded config or a new (empty) config if no config is existing yet
        return appConfig ?? new AppConfig();
    }
    
    // ==============
    // SAVING
    // ==============

    private void SaveConfig()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            // pretty print
            WriteIndented = true
        };
        string configJson = JsonSerializer.Serialize(this._appConfig, options);
        File.WriteAllText(AppConfigurator.AppConfigFilePath, configJson, Encoding.UTF8);
    }
    
    // ==============
    // ACTIONS FOR MUSIC DIRECTORIES
    // ==============
    
    public void AddDirectory(string directoryPath)
    {
        if (!this._appConfig.MusicDirectories.Contains(directoryPath))
        {
            this._appConfig.MusicDirectories.Add(directoryPath);
            this.SaveConfig();
        }
    }

    public void RemoveDirectory(string directoryPath)
    {
        if (this._appConfig.MusicDirectories.Contains(directoryPath))
        {
            this._appConfig.MusicDirectories.Remove(directoryPath);
            this.SaveConfig();
        }
    }
}