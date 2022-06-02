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
                Console.WriteLine("The currently set version");
            });
        }
    }
}
