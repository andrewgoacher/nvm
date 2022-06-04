using nvm.Configuration;

namespace nvm
{
    internal class LocalVersionService
    {
        readonly string _installPath;

        public LocalVersionService(Config config)
        {
            _installPath = config.NodeInstallPath;
        }

        public IEnumerable<string> GetLocalVersions()
        {
            var dirs = Directory.GetDirectories(_installPath);
            return dirs
                .Select(dir => Path.GetFileName(dir) ?? "")
                .Where(dir => dir.StartsWith("v"));
        }
    }
}
