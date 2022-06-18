using System.Collections;
using System.Text.Json;

namespace nvm.Configuration
{
    internal class Config
    {
        private static readonly string AppSettingPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "nvm");

        private static readonly string ConfigFile = "config.json";

        private string _nodeDistUrl = "https://nodejs.org/dist/";
        private string _nodeInstallPath = GetDefaultInstallFolder();
        private string _currentNodeVersion = "";
        private bool _changesDetected = false;

        public Config(bool forceChanges = false)
        {
            _changesDetected = forceChanges;
        }

        public Config()
        {

        }

        public string NodeDistUrl
        {
            get => _nodeDistUrl;
            set
            {
                _nodeDistUrl = value;
                _changesDetected = true;
            }
        }
        public string NodeInstallPath
        {
            get => _nodeInstallPath;
            set
            {
                _nodeInstallPath = value;
                _changesDetected = true;
            }
        }
        public string CurrentNodeVersion
        {
            get => _currentNodeVersion;
            set
            {
                _currentNodeVersion = value;
                _changesDetected = true;
            }
        }

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
                return new Config(true);
            }
        }

        public void Save()
        {
            if (!_changesDetected) { return; }

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
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullPath = Path.Combine(appdata, "nvm");

            return SetInstallFolder(fullPath);
        }
    }
}
