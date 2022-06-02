
using nvm.Commands;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var nodeService = new NodeService();

            var rootCommand = new RootCommand()
            {
                new ListCommand(nodeService),
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