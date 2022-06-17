using System.Collections;

namespace nvm.Configuration
{
    /// <summary>
    /// A wrapper around <see cref="Environment.GetEnvironmentVariable(string)"/> and <see cref="Environment.SetEnvironmentVariable(string, string?)" />
    /// This wrapper allows for the getting and setting of application variables that are stored in the users environment.
    /// </summary>
    internal static class Config 
    {
        private const string CURRENT_VERSION_KEY = "NVM_CURRENT_VERSION";
        private const string INSTALL_FOLDER_KEY = "NVM_INSTALL_LOCATION";
        private const string NODE_URL_KEY = "NVM_URL";
        public static string? NodeDistUrl
        {
            get => ReadEnvironmentVariable(NODE_URL_KEY, "https://nodejs.org/dist/");
            set => SetEnvironmentVariable(NODE_URL_KEY, value);
        }
        public static string NodeInstallPath
        {
            get => ReadEnvironmentVariable(INSTALL_FOLDER_KEY, GetDefaultInstallFolder())!;
            set => SetEnvironmentVariable(INSTALL_FOLDER_KEY, SetInstallFolder(value));
        }


        public static string? CurrentNodeVersion
        {
            get => ReadEnvironmentVariable(CURRENT_VERSION_KEY, "");
            set => SetEnvironmentVariable(CURRENT_VERSION_KEY, value);
        }

        public static IEnumerable<KeyValuePair<string, string?>> EnumerateValues()
        {
            yield return new KeyValuePair<string, string?>(CURRENT_VERSION_KEY, CurrentNodeVersion);
            yield return new KeyValuePair<string, string?>(INSTALL_FOLDER_KEY, NodeInstallPath);
            yield return new KeyValuePair<string, string?>(NODE_URL_KEY, NodeDistUrl);
        }

        private static string GetDefaultInstallFolder()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullPath = Path.Combine(appdata, "nvm");

            return SetInstallFolder(fullPath);
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

        private static string? ReadEnvironmentVariable(string variable, string? @default)
        {
            var val = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(val))
            {
                SetEnvironmentVariable(variable, @default);
                return @default;
            }

            return val;
        }

        private static void SetEnvironmentVariable(string variable, string? value)
        {
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
        }
    }
}
