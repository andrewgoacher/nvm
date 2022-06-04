using nvm.Configuration;
using System.IO.Compression;

namespace nvm.Node
{
    internal class DownloadNodeService
    {
        readonly Config _config;
        readonly HttpClient _httpClient;

        public DownloadNodeService(Config config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task DownloadNodeVersion(string version)
        {
            var name = $"node-{version}-win-x64";
            var url = $"{_config.NodeDistUrl}{version}/{name}.zip";

            var data = await _httpClient.GetStreamAsync(url);

            using var archive = new ZipArchive(data, ZipArchiveMode.Read);

            Directory.CreateDirectory(Path.Combine(_config.NodeInstallPath, version));

            foreach(var entry in archive.Entries)
            {
                if(!string.IsNullOrEmpty(entry.Name))
                {
                    var target = entry.FullName.Replace(name, version);
                    var path = Path.Combine(_config.NodeInstallPath, target);
                    Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                    entry.ExtractToFile(path);
                }
            }
        }
    }
}
