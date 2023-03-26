using CommandLine;

namespace nvm.Configuration;

[Verb("install", HelpText = "Install the specified node version")]
internal class InstallOptions : ProgramOptions
{
    [Value(0, Required = true)]
    public string Version { get; set; }
}
