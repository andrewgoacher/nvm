namespace nvm.Node;

using nvm.Configuration;
using nvm.Json.Converters;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class NodeClient : IDisposable
{
    readonly HttpClient _httpClient;
    readonly Config _config;

    public NodeClient(Config config)
    {
        _httpClient = new HttpClient();
        _config = config;
    }

    ~NodeClient()
    {
        Dispose(false);
    }

    public async Task<IEnumerable<NodeVersion>> GetAllNodeVersionsAsync()
    {
        var url = $"{_config.NodeDistUrl}index.json";

        var content = await _httpClient.GetStringAsync(url);

        var versions = JsonSerializer.Deserialize<Version[]>(content);

        if (versions == null)
        {
            throw new InvalidOperationException("No versions found");
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

    public async Task DownloadZipAsync(string version, Stream destination, IProgress<float> progress)
    {
        var name = $"node-{version}-win-x64";
        var url = $"{_config.NodeDistUrl}{version}/{name}.zip";

        await _httpClient.DownloadAsync(url, destination, progress);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient.Dispose();
        }
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
