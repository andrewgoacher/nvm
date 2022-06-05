using nvm.Node;
using System.CommandLine;

namespace nvm.Commands
{
    internal class UninstallCommand : Command
    {
        public UninstallCommand(DownloadNodeService downloadNodeService) : 
            base("uninstall", 
            "Uninstalls the specified version of node.\nUninstalling the current version will by default set the latest available.")
        {
            var versionArgument = new Argument<string>(name: "",
                description: "The version to uninstall.\nAllowed values are:\nlatest\ncurrent\nlts\nOr a specific version");

            AddArgument(versionArgument);

            this.SetHandler((string version) =>
            {
                var sanitisedVersion = SanitiseVersionName(version);
                downloadNodeService.RemoveNodeVersion(sanitisedVersion);
            }, versionArgument);
        }

        private static string SanitiseVersionName(string version)
        {
            return version.StartsWith("v") ? version : $"v{version}";
        }
    }
}
