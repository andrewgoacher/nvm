
using nvm.Commands;
using nvm.Node;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var config = new Configuration.Config(
                url: "https://nodejs.org/dist/",
                installPath: @"C:\Users\andre\AppData\Roaming\nvm");

            var nodeService = new FetchNodeVersions(config);

            var rootCommand = new RootCommand()
            {
                new ListCommand(config, nodeService),
                new InstallCommand(),
                new UninstallCommand(),
                new LocationCommand(),
                new EnvCommand(),
                new RcCommand(),
                new CurrentCommand(),
                new UseCommand()
            };


            return await rootCommand.InvokeAsync(args);
        }
    }
}