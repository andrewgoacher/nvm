using CommandLine;

namespace nvm.Config
{
    public sealed class CommandLineOptions
    {
        [Option("all", Default = false, HelpText = "Lists all available node packages", Min = 0, Max = 1, Required = false)]
        public bool ListAll { get; set; }
    }
}
