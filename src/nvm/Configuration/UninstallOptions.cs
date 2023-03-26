using CommandLine;

namespace nvm.Configuration;

internal class UninstallOptions : ProgramOptions
{
    [Value(0, Required = true)]
    public string Version { get; set; }
}
