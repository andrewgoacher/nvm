using nvm.Configuration;
using nvm.Node;
using System.CommandLine;

namespace nvm.Commands
{
    internal class ListCommand : Command
    {
        private readonly FetchNodeVersions _fetchNodeVersions;
        public ListCommand(Config config, FetchNodeVersions fetchNodeVersions) : base("list",
            "Lists all versions of node installed currently")
        {
            _fetchNodeVersions = fetchNodeVersions;

            var allOption = new Option<bool>("--all", "list all versions of node available");
            AddOption(allOption);

            var dateOption = new Option<bool>("--date", "display the date along with the version");
            AddOption(dateOption);

            this.AddValidator(validator =>
            {
                var all = validator.GetValueForOption(allOption);
                var date = validator.GetValueForOption(dateOption);

                if (!all && date)
                {
                    validator.ErrorMessage = "Cannot list dates for local installed versions";
                }
            });

            this.SetHandler(async (bool all, bool date) =>
            {
                var localInstalls = GetLocalVersions(config.NodeInstallPath);

                if (!all)
                {
                    foreach(var install in localInstalls)
                    {
                        Console.WriteLine(install);
                    }
                    return;
                }

                var maxYear = all ? (int?)null : DateTime.Now.AddYears(-1).Year;
                var versions = await (date ?
                    GetVersionsAndDatesAsync(maxYear) :
                    GetVersionsAsync(maxYear));

                foreach(var version in versions)
                {
                    var marker = localInstalls.Any(install => version.StartsWith(install)) ? "* " : "";

                    Console.WriteLine("{0}{1}", marker, version);
                }

            }, allOption, dateOption);
        }

        private IEnumerable<string> GetLocalVersions(string installPath)
        {
            var dirs = Directory.GetDirectories(installPath);
            return dirs
                .Select(dir => Path.GetFileName(dir)?? "")
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
