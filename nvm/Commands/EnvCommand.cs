using nvm.Configuration;
using System.CommandLine;

namespace nvm.Commands
{
    internal class EnvCommand : Command
    {
        public EnvCommand() : base("env", 
            "Gets the environment variables that are currently used")
        {
            this.SetHandler(() =>
            {
                foreach(var kvp in Config.EnumerateValues())
                {
                    Console.WriteLine("{0}: {1}",
                        kvp.Key, kvp.Value);
                }
            });
        }
    }
}
