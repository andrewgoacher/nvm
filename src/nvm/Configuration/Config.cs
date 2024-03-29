﻿using System.Text.Json;

namespace nvm.Configuration;

internal class Config
{
    private static readonly string AppSettingPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "nvm-dotnet");

    private static readonly string ConfigFile = "config.json";

    private string _nodeDistUrl = "https://nodejs.org/dist/";
    private string _nodeInstallPath = GetDefaultInstallFolder();
    private string _currentNodeVersion = "";

    public Config()
    {

    }

    public string NodeDistUrl
    {
        get => _nodeDistUrl;
        set
        {
            _nodeDistUrl = value;
        }
    }
    public string NodeInstallPath
    {
        get => _nodeInstallPath;
        set
        {
            _nodeInstallPath = value;
        }
    }
    public string CurrentNodeVersion
    {
        get => _currentNodeVersion;
        set
        {
            _currentNodeVersion = value;
        }
    }

    public bool IsNewConfig { get; private set; }

    public static Config Load()
    {
        try
        {
            var path = Path.Combine(AppSettingPath, ConfigFile);
            var data = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Config>(data)!;
        }
        catch
        {
            return new Config()
            {
                IsNewConfig = true,
            };
        }
    }

    public void Save()
    {
        var path = Path.Combine(AppSettingPath, ConfigFile);
        var data = JsonSerializer.Serialize(this);
        File.WriteAllText(path, data);
    }

    private static string SetInstallFolder(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path!);
        }

        return path!;
    }

    private static string GetDefaultInstallFolder()
    {
        return SetInstallFolder(AppSettingPath);
    }
}