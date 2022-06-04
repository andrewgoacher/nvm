using nvm.Configuration;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace nvm.Commands
{
    internal class UseCommand : Command
    {
        readonly LocalVersionService _localVersionService;

        readonly Argument<string> _versionArg;

        public UseCommand(LocalVersionService localVversionService) :
            base("use", "Sets the version to use")
        {
            _localVersionService = localVversionService;
            _versionArg = new Argument<string>(
                name: null,
                description: "The version to use");

            AddArgument(_versionArg);

            AddValidator(Validate);

            this.SetHandler((string version) =>
            {
                var installVersion = version.StartsWith("v") ? version : $"v{version}";
                Environment.SetEnvironmentVariable(EnvironmentVariables.CURRENT_VERSION_KEY, installVersion);

                Console.WriteLine("Current version set to {0}", installVersion);
            }, _versionArg);
        }

        private void Validate(CommandResult result)
        {
            var versionArg = result.GetValueForArgument(_versionArg);

            var localVersions = _localVersionService.GetLocalVersions();

            if (!localVersions.Any(version => version.Equals(versionArg) || version.Equals($"v{versionArg}")))
            {
                result.ErrorMessage = "Requested version not installed.\nRun nvm list to get the current installed versions";
            }
        }
    }
}
