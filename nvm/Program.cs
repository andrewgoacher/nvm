using Microsoft.Extensions.DependencyInjection;
using nvm.Commands;
using nvm.Node;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<FetchNodeVersions>();
            serviceCollection.AddScoped<LocalVersionService>();
            serviceCollection.AddScoped<DownloadNodeService>();

            serviceCollection.AddScoped<Command, ListCommand>();
            serviceCollection.AddScoped<Command, InstallCommand>();
            serviceCollection.AddScoped<Command, UninstallCommand>();
            serviceCollection.AddScoped<Command, LocationCommand>();
            serviceCollection.AddScoped<Command, EnvCommand>();
            serviceCollection.AddScoped<Command, RcCommand>();
            serviceCollection.AddScoped<Command, CurrentCommand>();
            serviceCollection.AddScoped<Command, UseCommand>();
            serviceCollection.AddScoped<Command, RunCommand>();

            var provider = serviceCollection.BuildServiceProvider();

            var rootCommand = new RootCommand(
                "nvm allows the user to run and manage multiple versions of node.");

            foreach(var command in provider.GetServices<Command>())
            {
                rootCommand.AddCommand(command);
            }


            rootCommand.TreatUnmatchedTokensAsErrors = false;


            return await rootCommand.InvokeAsync(args);
        }
    }
}