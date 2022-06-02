using HtmlAgilityPack;

namespace nvm
{
    internal class NodeService : IDisposable
    {
        
        const string NODE_DIST_PATH = "https://nodejs.org/dist/";

        readonly HttpClient _httpClient;
     
        public NodeService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<NodeVersion>> GetAllVersionsAsync()
        {
            var rawHtml = await _httpClient.GetStringAsync(NODE_DIST_PATH);
            var html = new HtmlDocument();
            html.LoadHtml(rawHtml);

            var preTag = html.DocumentNode.SelectNodes("//pre");
            var nodes = preTag.Nodes().ToArray();
            var versions = new List<NodeVersion>();

            for (var i = 0; i < nodes.Length - 2;)
            {
                var anchor = nodes[i];
                var text = nodes[i + 1];

                var version = NodeVersion.Parse(anchor.InnerText, text.InnerText);

                if (version != null)
                {
                    versions.Add(version);
                }

                i += 2;
            }

            return versions
                .OrderByDescending(version => version.IsLatest)
                .ThenByDescending(version => version.Major)
                .ThenByDescending(version => version.Minor)
                .ThenByDescending(version => version.Patch);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }
    }
}
