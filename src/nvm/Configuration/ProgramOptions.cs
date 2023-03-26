using CommandLine;
using nvm.Logging;

namespace nvm.Configuration;

public abstract class ProgramOptions
{
    [Option("verbose", Default = false, Required = false)]
    public bool Verbose { get; set; }

    [Option("verbosity", Default = "Information", Required = false)]
    public string Verbosity { get; set; }

    public LogLevel LogLevel => Enum.Parse<LogLevel>(Verbosity);
}
