using nvm.Configuration;
using System.CommandLine;

namespace nvm.Commands
{
    internal class CurrentCommand : Command
    {
        public CurrentCommand() : 
            base("current", "Gets the currently set version")
        {
            this.SetHandler(() =>
            {
                var currentVersion = Config.CurrentNodeVersion;

                if (string.IsNullOrEmpty(currentVersion))
                {
                    Console.WriteLine("Current version not set");
                }
                else
                {
                    Console.WriteLine("Current version: {0}", currentVersion);
                }

            });
        }
    }
}
