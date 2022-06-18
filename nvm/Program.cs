using Microsoft.Extensions.DependencyInjection;
using nvm.Clients;
using nvm.Commands;
using nvm.Configuration;
using System.CommandLine;


// lts: gallium = latest-gallium/
// current is first

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var config = Config.Load();
            var provider = RegisterServices(new ServiceCollection(), config);
            var rootCommand = CreateRootCommand(provider);
            
            var response = await rootCommand.InvokeAsync(args);
            config.Save();

            return response;
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

        private static IServiceProvider RegisterServices(IServiceCollection serviceCollection, Config config)
        {
            serviceCollection.AddSingleton(config);

            serviceCollection.AddScoped<NodeClient>();
            serviceCollection.AddScoped<FileSystemClient>();


            serviceCollection.AddScoped<Command, ListCommand>();
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