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
                Console.WriteLine("Gets the env value");
            });
        }
    }
}
