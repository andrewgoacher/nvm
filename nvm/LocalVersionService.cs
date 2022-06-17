using nvm.Configuration;

namespace nvm
{
    internal class LocalVersionService
    {
        public LocalVersionService()
        {
        }

        public IEnumerable<string> GetLocalVersions()
        {
            var dirs = Directory.GetDirectories(Config.NodeInstallPath!);
            return dirs
                .Select(dir => Path.GetFileName(dir) ?? "")
                .Where(dir => dir.StartsWith("v"));
        }
    }
}
