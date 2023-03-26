using CommandLine;
using nvm.Configuration;
using nvm.Handlers;
using nvm.Logging;

namespace nvm;

internal static class ParserExtensions
{
    public static async Task Handle<TOptions>(this ParserResult<object> parser, Config config, IUseCaseHandler<TOptions> handler)
    {
        await parser.WithParsedAsync<TOptions>(async options => await handler.HandleAsync(config, options));
    }

    public static LogLevel GetLogLevel(this ProgramOptions options)
    {
        if (options.Verbose)
        {
            return LogLevel.Diagnostic;
        }

        return options.LogLevel;
    }
}