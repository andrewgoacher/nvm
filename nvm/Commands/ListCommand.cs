using System.CommandLine;

namespace nvm.Commands
{
    internal class ListCommand : Command
    {
        private readonly NodeService _nodeService;
        public ListCommand(NodeService service) : base("list",
            "Lists all versions of node installed currently")
        {
            _nodeService = service;

            var allOption = new Option<bool>("--all", "list all versions of node available");
            AddOption(allOption);

            var dateOption = new Option<bool>("--date", "display the date along with the version");
            AddOption(dateOption);

            this.SetHandler(async (bool all, bool date) =>
            {
                var maxYear = all ? (int?)null : DateTime.Now.AddYears(-1).Year;
                var versions = await (date ?
                    GetVersionsAndDatesAsync(maxYear) :
                    GetVersionsAsync(maxYear));

                foreach(var version in versions)
                {
                    Console.WriteLine(version);
                }

            }, allOption, dateOption);
        }

        private async Task<IEnumerable<string>> GetVersionsAsync(int? maxYear)
        {
            var versions = await _nodeService.GetAllVersionsAsync();
            return versions
                .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
                .Select(version => $"{version.Version}");
        }

        private async Task<IEnumerable<string>> GetVersionsAndDatesAsync(int? maxYear)
        {
            var versions = await _nodeService.GetAllVersionsAsync();
            return versions
                .Where(version => maxYear == null || version.ReleaseDate.Year >= maxYear)
                .Select(version => $"{version.Version} - {version.ReleaseDate.ToString("dd-MMM-yyyy")}");
        }
    }
}
