using nvm.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace nvm.Node
{
    internal class FetchNodeVersions : IDisposable
    {
        readonly HttpClient _httpClient;

        public FetchNodeVersions()
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

                return NodeVersion.Parse(version.VersionString, version.Date, index == 0);
            }

        }

        public void Dispose()
        {
            _httpClient.Dispose();
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
        }
    }
}
