using CommandLine;
using nvm.Configuration;
using nvm.Handlers;

namespace nvm;

internal static class ParserExtensions
{
    public static async Task Handle<TOptions>(this ParserResult<object> parser, Config config, IUseCaseHandler<TOptions> handler)
    {
        await parser.WithParsedAsync<TOptions>(async options => await handler.HandleAsync(config, options));
    }
}