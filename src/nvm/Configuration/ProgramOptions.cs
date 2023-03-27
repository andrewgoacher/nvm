using CommandLine;

namespace nvm.Configuration;

public abstract class ProgramOptions
{
    [Option("verbose", Default = false, Required = false)]
    public bool Verbose { get; set; }
}
