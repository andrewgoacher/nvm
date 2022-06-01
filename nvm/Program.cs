
using nvm.Commands;
using System.CommandLine;

namespace nvm
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            /*
             * nvm list - show 2 years worth
             * nvm list --all show all 
             * nvm list --date show date
             * 
             * nvm install 12.13.13 - install node version 12.13.13
             * nvm install latest - installs latest
             * nvm install --use 12.13.13 - install version 12.13.13 and set as current
             * 
             * 
             * nvm use 12.13.13 - set 12.13.13 as current
             * 
             * install and use can also use 
             * latest, newest, lts
             * 
             * nvm current - gets current version
             * 
             * nvm uninstall 12.13.13 uninstalls 12.13.13
             * 
             * nvm version - gets current program version
             * 
             * nvm location - gets the path where files are stored
             * nvm localtion --set sets the path
             * nvm location --move moves the current ones
             * 
             * nvm env - gets the environment variables currently used
             * 
             * nvm 12.13.14 --npm [commands]
             * runs npm using 12.13.14 with specified commands
             * 
             
             * nvm rc use - tells nvm to use the nvm rc in the current directory (default)
             * nvm rc ignore - tells nvm to ignore the nvmrc in the current directory
             * 
             * npm/npx and node will use the current version unless nvmrc exists
             */
            var envCommand = new Command("env");
            var rcCommand = new Command("rc");
            var versionCommand = new Command("version");
            var currentCommand = new Command("current");
            var useCommand = new Command("use");

            var rootCommand = new RootCommand()
            {
                new ListCommand(),
                new InstallCommand(),
                new UninstallCommand(),
                new LocationCommand(),
                envCommand,
                rcCommand,
                versionCommand,
                currentCommand,
                useCommand
            };


            return await rootCommand.InvokeAsync(args);
        }
    }
}