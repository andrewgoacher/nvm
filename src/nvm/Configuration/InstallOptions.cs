using CommandLine;

namespace nvm.Configuration;

[Verb("install", HelpText = "Install the specified node version")]
internal class InstallOptions : ProgramOptions
{
    [Value(0, Required = true)]
    public string Version { get; set; }

    [Option("use", Default = false, Required = false, HelpText = "Set this version to be the default version")]
    public bool Use { get; set; }
}
