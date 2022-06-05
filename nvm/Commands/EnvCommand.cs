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
                Console.WriteLine("{0}: {1}",
                    EnvironmentVariables.CURRENT_VERSION_KEY,
                    Environment.GetEnvironmentVariable(EnvironmentVariables.CURRENT_VERSION_KEY, EnvironmentVariableTarget.User));
            });
        }
    }
}
