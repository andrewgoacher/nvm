
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
            var localVersionService = new LocalVersionService(config);
            var downloadNodeService = new DownloadNodeService(config);

            var rootCommand = new RootCommand()
            {
                new ListCommand(nodeService, localVersionService),
                new InstallCommand(nodeService, localVersionService, downloadNodeService),
                new UninstallCommand(),
                new LocationCommand(),
                new EnvCommand(),
                new RcCommand(),
                new CurrentCommand(),
                new UseCommand(localVersionService)
            };


            return await rootCommand.InvokeAsync(args);
        }
    }
}