using nvm.Configuration;
using nvm.Node;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace nvm.Commands
{
    internal class InstallCommand : Command
    {
        readonly LocalVersionService _localVersionService;
        readonly FetchNodeVersions _fetchNodeVersions;
        readonly DownloadNodeService _downloadNodeService;

        readonly Option<bool> _useOption;
        readonly Argument<string> _versionArgument;

        public InstallCommand(FetchNodeVersions fetchNodeVersions, 
            LocalVersionService localVersions,
            DownloadNodeService downloadNodeService) : base("install", "Installs the specified version of node")
        {
            _localVersionService = localVersions;
            _fetchNodeVersions = fetchNodeVersions;
            _downloadNodeService = downloadNodeService;

            _versionArgument = new Argument<string>(name: "",
                description: "The version to install.\nAllowed values are:\nlatest\ncurrent\nlts\nOr a specific version");
            _useOption = new Option<bool>("--use", "Tells nvm to set this as the current version to use as default");

            AddArgument(_versionArgument);
            AddOption(_useOption);

            AddValidator(async validator => await Validate(validator));

            this.SetHandler(async (string version, bool use) =>
            {
                var sanitisedVersion = SanitiseVersionName(version);

                await _downloadNodeService.DownloadNodeVersion(sanitisedVersion);

                if (use)
                {
                    Environment.SetEnvironmentVariable(EnvironmentVariables.CURRENT_VERSION_KEY, sanitisedVersion, EnvironmentVariableTarget.User);
                }


            }, _versionArgument, _useOption);
            _downloadNodeService = downloadNodeService;
        }

        private static string SanitiseVersionName(string version)
        {
            return version.StartsWith("v") ? version : $"v{version}";
        }

        private async Task Validate(CommandResult result)
        {
            var versionArg = SanitiseVersionName(result.GetValueForArgument(_versionArgument));

            var localVersions = _localVersionService.GetLocalVersions();

            if (localVersions.Contains(versionArg))
            {
                result.ErrorMessage = "Requested version is already installed.\nRun nvm list to get the current installed versions";
            }

            var fetchVersions = await _fetchNodeVersions.GetAllNodeVersionsAsync();
            if (fetchVersions == null || !fetchVersions.Any(version => version.Version.Equals(versionArg)))
            {
                result.ErrorMessage = $"Node version {versionArg} not found";
            }
        }
    }
}
