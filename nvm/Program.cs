
using nvm.Commands;
using nvm.Node;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var nodeService = new FetchNodeVersions();
            var localVersionService = new LocalVersionService();
            var downloadNodeService = new DownloadNodeService();

            var rootCommand = new RootCommand()
            {
                new ListCommand(nodeService, localVersionService),
                new InstallCommand(nodeService, localVersionService, downloadNodeService),
                new UninstallCommand(downloadNodeService),
                new LocationCommand(),
                new EnvCommand(),
                //new RcCommand(),
                new CurrentCommand(),
                new UseCommand(localVersionService),
                new RunCommand()
            };

            rootCommand.TreatUnmatchedTokensAsErrors = false;


            return await rootCommand.InvokeAsync(args);
        }
    }
}