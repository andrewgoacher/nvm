using System.CommandLine;

namespace nvm.Commands
{
    internal class InstallCommand : Command
    {
        public InstallCommand() : base("install", "Installs the specified version of node")
        {
            var versionArgument = new Argument<string>(name: "", 
                description: "The version to install.\nAllowed values are:\nlatest\ncurrent\nlts\nOr a specific version");
            var useOption = new Option<bool>("--use", "Tells nvm to set this as the current version to use as default");

            AddArgument(versionArgument);
            AddOption(useOption); 

            this.SetHandler((string version, bool use) =>
            {
                Console.WriteLine("Installing version {0}", version);
                Console.WriteLine("Using this version: {0}", use);
            }, versionArgument, useOption);
        }
    }
}
