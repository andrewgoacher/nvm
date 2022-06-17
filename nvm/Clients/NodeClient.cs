using nvm.Configuration;
using nvm.Exceptions;
using nvm.Serialization.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace nvm.Clients
{
    internal class NodeClient
    {
        readonly HttpClient _httpClient;

        public NodeClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<NodeVersion>> GetAllNodeVersionsAsync()
        {
            var url = $"{Config.NodeDistUrl}index.json";

            var content = await _httpClient.GetStringAsync(url);

            var versions = JsonSerializer.Deserialize<Version[]>(content);

            if (versions == null)
            {
                throw new VersionsNotFoundException();
            }

            return versions
                .Select(Create)
                .Where(version => version != null)
                .OrderByDescending(version => version!.IsLatest)
                .ThenByDescending(version => version!.Major)
                .ThenByDescending(version => version!.Minor)
                .ThenByDescending(version => version!.Patch)
                .ToList()!;

            static NodeVersion? Create(Version version, int index)
            {
                if (version == null) { return null; }
                if (string.IsNullOrEmpty(version.VersionString)) { return null; }
                if (string.IsNullOrEmpty(version.Date)) { return null; }

                return NodeVersion.Parse(
                    version.VersionString,
                    version.Date,
                    isLatest: (index == 0),
                    isLts: version.Lts);
            }
        }

        public async Task<Stream> DownloadZipAsync(string version)
        {
            var name = $"node-{version}-win-x64";
            var url = $"{Config.NodeDistUrl}{version}/{name}.zip";

            var data = await _httpClient.GetStreamAsync(url);
            return data;
        }

        private class Version
        {
            [JsonPropertyName("version")]
            public string? VersionString { get; set; }

            [JsonPropertyName("date")]
            public string? Date { get; set; }

            [JsonPropertyName("files")]
            public string[]? Files { get; set; }

            [JsonPropertyName("npm")]
            public string? Npm { get; set; }

            [JsonPropertyName("lts"), JsonConverter(typeof(StringBoolConverter))]
            public string Lts { get; set; }
        }
    }
}
