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
            var provider = RegisterServices(new ServiceCollection());
            var rootCommand = CreateRootCommand(provider);

            return await rootCommand.InvokeAsync(args);
        }

        private static RootCommand CreateRootCommand(IServiceProvider provider)
        {
            var rootCommand = new RootCommand(
                "nvm allows the user to run and manage multiple versions of node.");

            foreach (var command in provider.GetServices<Command>())
            {
                rootCommand.AddCommand(command);
            }

            rootCommand.TreatUnmatchedTokensAsErrors = false;

            return rootCommand;
        }

        private static IServiceProvider RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<FetchNodeVersions>();
            serviceCollection.AddScoped<LocalVersionService>();
            serviceCollection.AddScoped<DownloadNodeService>();

            //serviceCollection.AddScoped<Command, ListCommand>();
            //serviceCollection.AddScoped<Command, InstallCommand>();
            //serviceCollection.AddScoped<Command, UninstallCommand>();
            //serviceCollection.AddScoped<Command, LocationCommand>();
            //serviceCollection.AddScoped<Command, EnvCommand>();
            //serviceCollection.AddScoped<Command, RcCommand>();
            //serviceCollection.AddScoped<Command, CurrentCommand>();
            //serviceCollection.AddScoped<Command, UseCommand>();
            //serviceCollection.AddScoped<Command, RunCommand>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}