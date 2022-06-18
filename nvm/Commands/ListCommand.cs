using nvm.Clients;
using nvm.Configuration;
using System.CommandLine;

namespace nvm.Commands
{
    internal class ListCommand : Command
    {
        private readonly NodeClient _nodeClient;
        private readonly FileSystemClient _fileSystem;
        private readonly Config _config;
        private readonly Option<bool> _allOption;
        private readonly Option<bool> _dateOption;

        public ListCommand(
            Config config,
            NodeClient nodeClient,
            FileSystemClient fileSystem) : base("list",
            "Lists versions of node use --all to list all versions of node available")
        {
            _config = config;
            _nodeClient = nodeClient;
            _fileSystem = fileSystem;

            _allOption = new Option<bool>("--all", "list all versions of node available");
            AddOption(_allOption);

            _dateOption = new Option<bool>("--date", "display the date along with the version");
            AddOption(_dateOption);

            this.SetHandler(async (bool all, bool date) =>
            {
                await Handle(all, date);
            }, _allOption, _dateOption);
        }

        /// <summary>
        /// Should output the necessary node versions 
        /// 
        /// When all is specified it will output the versions matching the nodejs json file in
        /// the root of dist 
        /// When all is not specified it will show local only
        /// It should show which one is the current node version
        /// It should show which one is the LTS not version
        /// It should show which one is the currently used version.
        /// </summary>
        /// <param name="all"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task Handle(bool all, bool date)
        {
            // todo: Move all this into a usecase that can be tested
            var maxYear = DateTime.Now.AddYears(-1).Year;

            var nodeVersions = (await GetNodeVersionsAsync(maxYear)).ToList();
            var localInstalls = _fileSystem.GetLocalVersions();

            var mappedVersions = EnumerateVersions(nodeVersions, localInstalls, all).ToList();
            var formatString = "{0} {1}{2}{3}";


            foreach (var version in mappedVersions)
            {
                var (firstArg, secondArg, thirdArg, fourthArg) = GetStringProps(version, all, date);

                Console.WriteLine(formatString, firstArg, secondArg, thirdArg, fourthArg);
            }

            Console.WriteLine("This list shows the releases over the last year. For more information, visit https://nodejs.org/en/download/releases");
        }

        private (string, string, string, string) GetStringProps(MappedNodeVersion version, bool showAll, bool showDate)
        {
            var firstArg = version.IsLocalInstalled ?
                   (showAll ?
                       (version.IsCurrentSet ? "**" : "*") :
                       "") :
                   "";

            var secondArg = version.NodeVersion.Version;
            var thirdArg = version.IsLatest ?
                " (latest)" :
                (version.IsLts ?
                    " (lts)" : "");

            // todo: Date should be current culture
            var fourthArg = showDate ?
                $" {version.NodeVersion.ReleaseDate.ToShortDateString()}" :
                "";

            return (firstArg, secondArg, thirdArg, fourthArg);  
        }

        private IEnumerable<MappedNodeVersion> EnumerateVersions(IEnumerable<NodeVersion> allVersions, IEnumerable<string> localVersions, bool showAll)
        {
            if (!showAll && !localVersions.Any())
            {
                return Enumerable.Empty<MappedNodeVersion>();
            }

            var currentNodeVersion = _config.CurrentNodeVersion;

            var isLts = false;
            var returnItems = new List<MappedNodeVersion>();
            foreach (var nodeVersion in allVersions)
            {
                var isLocalVersion = localVersions.Contains(nodeVersion.Version);
                var isCurrentSet = isLocalVersion && nodeVersion.Version.Equals(currentNodeVersion);
                var isLtsVersion = !isLts && !nodeVersion.LtsValue.Equals("false");
                if (isLtsVersion) { isLts = true; }

                if (isLocalVersion || showAll)
                {

                    returnItems.Add(new MappedNodeVersion
                    {
                        NodeVersion = nodeVersion,
                        IsLatest = nodeVersion.IsLatest,
                        IsLts = isLtsVersion,
                        IsLocalInstalled = isLocalVersion,
                        IsCurrentSet = isCurrentSet,
                    });
                }
            }

            return returnItems;
        }

        class MappedNodeVersion
        {
            public NodeVersion NodeVersion { get; set; }
            public bool IsLatest { get; set; }
            public bool IsLts { get; set; }
            public bool IsLocalInstalled { get; set; }
            public bool IsCurrentSet { get; set; }
        }

        private async Task<IEnumerable<NodeVersion>> GetNodeVersionsAsync(int? maxYear)
        {
            var versions = await _nodeClient.GetAllNodeVersionsAsync();
            return versions
                .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear);
        }

        //private async Task<IEnumerable<string>> GetVersionsAsync(int? maxYear)
        //{
        //    var versions = await _nodeClient.GetAllNodeVersionsAsync();
        //    return versions
        //        .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
        //        .Select(version => $"{version.Version}");
        //}

        //private async Task<IEnumerable<string>> GetVersionsAndDatesAsync(int? maxYear)
        //{
        //    var versions = await _nodeClient.GetAllNodeVersionsAsync();
        //    return versions
        //        .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
        //        .Select(version => $"{version.Version} - {version.ReleaseDate.ToString("dd-MMM-yyyy")}");
        //}
    }
}
