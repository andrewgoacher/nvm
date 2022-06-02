using System.CommandLine;

namespace nvm.Commands
{
    internal class UseCommand : Command
    {
        public UseCommand() :
            base("use", "Sets the version to use")
        {
            var versionArg = new Argument<string>(name: null, description: "The version to use");

            AddArgument(versionArg);

            this.SetHandler((string version) =>
            {
                Console.WriteLine("Using version: {0}", version);
            }, versionArg);
        }
    }
}
