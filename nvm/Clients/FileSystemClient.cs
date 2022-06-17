using nvm.Configuration;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm.Clients
{
    internal class FileSystemClient
    {
        public FileSystemClient()
        {

        }

        public void RemoveVersion(string version)
        {
            var path = Path.Combine(Config.NodeInstallPath!, version);
            Directory.Delete(path, true);
        }

        public void SaveVersion(string version, Stream data)
        {
            Directory.CreateDirectory(Path.Combine(Config.NodeInstallPath!, version));

            using var archive = new ZipArchive(data, ZipArchiveMode.Read);

            var name = $"node-{version}-win-x64";

            foreach (var entry in archive.Entries)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    var target = entry.FullName.Replace(name, version);
                    var path = Path.Combine(Config.NodeInstallPath!, target);
                    Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                    entry.ExtractToFile(path);
                }
            }
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
