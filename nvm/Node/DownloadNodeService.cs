using nvm.Configuration;
using System.IO.Compression;

namespace nvm.Node
{
    internal class DownloadNodeService
    {
        readonly HttpClient _httpClient;

        public DownloadNodeService()
        {
            _httpClient = new HttpClient();
        }

        public void RemoveNodeVersion(string version)
        {
            var path = Path.Combine(Config.NodeInstallPath!, version);
            Directory.Delete(path);
        }

        public async Task DownloadNodeVersion(string version)
        {
            var name = $"node-{version}-win-x64";
            var url = $"{Config.NodeDistUrl}{version}/{name}.zip";

            var data = await _httpClient.GetStreamAsync(url);

            using var archive = new ZipArchive(data, ZipArchiveMode.Read);

            Directory.CreateDirectory(Path.Combine(Config.NodeInstallPath!, version));

            foreach(var entry in archive.Entries)
            {
                if(!string.IsNullOrEmpty(entry.Name))
                {
                    var target = entry.FullName.Replace(name, version);
                    var path = Path.Combine(Config.NodeInstallPath!, target);
                    Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                    entry.ExtractToFile(path);
                }
            }
        }
    }
}
