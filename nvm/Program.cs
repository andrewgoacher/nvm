using Microsoft.Extensions.DependencyInjection;
using nvm.Clients;
using nvm.Commands;
using nvm.Configuration;
using System.CommandLine;


// lts: gallium = latest-gallium/
// current is first

/**
 * This will be the front runner info for nvm.  
 * Each task that this program can perform will be listed here with most interactions.
 * Each task will be represented as a specific application service responding to a command
 * Tasks should come with accompanying tests to ensure behavior is correct.
 * 
 * init:
 * Initializes the tool.  The first time nvm is used it should run this regardless.
 * the output of this command will be a settings file stored in app data for the tool
 * Init will be able to determine where the node versions are installed and add this to the path
 * 
 * Default will put all data in appsettings 
 * Options to specify otherwise (use tab complete if possible here to find the path)
 * 
 * install:
 * 
 * Will install a specific version into the designated location.
 * Optional command arg to set this version as the "current"
 * Current means the version of node used when no specific version is provided
 * or an nvmrc file isn't used.
 * not providing a version will provide a prompt
 * 
 * uninstall: 
 * Will remove a version from the system
 * if this version if current then prompt to choose new default if any are available will be provided
 * 
 * not providing a version will show prompt
 * 
 * use: 
 * Sets the version as current
 * if the version doesn't exist get the choice to install it or set a different one
 * Not providing a version will provide a prompt
 * 
 * list:
 * Will show all locally installed versions
 * optional argument to show 12 months of recent versions
 * 
 * run;
 * 
 * Will run the provided command in the specified verison
 * 
 * nvm run node -v -> runs "node -v" against currently set version OR against nvmrc version
 * nvm run node -v --12.13.14 will run "node -v" against the installed 12.13.14 version
 * 
 * not all versions might have a tool.
 * 
 * Special verions
 * 
 * latest/
 * lts/
 * 
 */

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