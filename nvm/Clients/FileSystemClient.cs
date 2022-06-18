using nvm.Configuration;
using System.IO.Compression;

namespace nvm.Clients
{
    internal class FileSystemClient
    {
        readonly Config _config;

        public FileSystemClient(Config config)
        {
            _config = config;
        }

        public void RemoveVersion(string version)
        {
            var path = Path.Combine(_config.NodeInstallPath!, version);
            Directory.Delete(path, true);
        }

        public void SaveVersion(string version, Stream data)
        {
            Directory.CreateDirectory(Path.Combine(_config.NodeInstallPath!, version));

            using var archive = new ZipArchive(data, ZipArchiveMode.Read);

            var name = $"node-{version}-win-x64";

            foreach (var entry in archive.Entries)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    var target = entry.FullName.Replace(name, version);
                    var path = Path.Combine(_config.NodeInstallPath!, target);
                    Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                    entry.ExtractToFile(path);
                }
            }
        }

        public IEnumerable<string> GetLocalVersions()
        {
            var dirs = Directory.GetDirectories(_config.NodeInstallPath!);
            return dirs
                .Select(dir => Path.GetFileName(dir) ?? "")
                .Where(dir => dir.StartsWith("v"));
        }
    }
}
