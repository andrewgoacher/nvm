
using nvm.Commands;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
                new ListCommand(),
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