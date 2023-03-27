using CommandLine;

namespace nvm.Configuration;

[Verb("run", HelpText = "Runs the specified version of node, or the default version")]
internal class RunOptions : ProgramOptions
{
    [Value(0, Required = true)]
    public string Command { get; set; } = "";

    [Option("version", HelpText = "Specifies the version to run")]
    public string Version { get; set; } = "";
}