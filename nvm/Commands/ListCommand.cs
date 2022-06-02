using nvm.Configuration;
using nvm.Node;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace nvm.Commands
{
    internal class ListCommand : Command
    {
        private readonly FetchNodeVersions _fetchNodeVersions;
        readonly Config _config;
        private readonly Option<bool> _allOption;
        private readonly Option<bool> _dateOption;

        public ListCommand(Config config, FetchNodeVersions fetchNodeVersions) : base("list",
            "Lists all versions of node installed currently")
        {
            _config = config;
            _fetchNodeVersions = fetchNodeVersions;

            _allOption = new Option<bool>("--all", "list all versions of node available");
            AddOption(_allOption);

            _dateOption = new Option<bool>("--date", "display the date along with the version");
            AddOption(_dateOption);

            this.AddValidator(Validate);

            this.SetHandler(async (bool all, bool date) =>
            {
                await Handle(all, date);
            }, _allOption, _dateOption);
        }

        private async Task Handle(bool all, bool date)
        {
            var localInstalls = GetLocalVersions(_config.NodeInstallPath);

            if (!all)
            {
                foreach (var install in localInstalls)
                {
                    Console.WriteLine(install);
                }
                return;
            }

            var maxYear = all ? (int?)null : DateTime.Now.AddYears(-1).Year;
            var versions = await (date ?
                GetVersionsAndDatesAsync(maxYear) :
                GetVersionsAsync(maxYear));

            foreach (var version in versions)
            {
                var marker = localInstalls.Any(install => version.StartsWith(install)) ? "* " : "";

                Console.WriteLine("{0}{1}", marker, version);
            }
        }

        private void Validate(CommandResult result)
        {
            var all = result.GetValueForOption(_allOption);
            var date = result.GetValueForOption(_dateOption);

            if (!all && date)
            {
                result.ErrorMessage = "Cannot list dates for local installed versions";
            }
        }

        private static IEnumerable<string> GetLocalVersions(string installPath)
        {
            var dirs = Directory.GetDirectories(installPath);
            return dirs
                .Select(dir => Path.GetFileName(dir) ?? "")
                .Where(dir => dir.StartsWith("v"));
        }

        private async Task<IEnumerable<string>> GetVersionsAsync(int? maxYear)
        {
            var versions = await _fetchNodeVersions.GetAllNodeVersionsAsync();
            return versions
                .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
                .Select(version => $"{version.Version}");
        }

        private async Task<IEnumerable<string>> GetVersionsAndDatesAsync(int? maxYear)
        {
            var versions = await _fetchNodeVersions.GetAllNodeVersionsAsync();
            return versions
                .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
                .Select(version => $"{version.Version} - {version.ReleaseDate.ToString("dd-MMM-yyyy")}");
        }
    }
}
