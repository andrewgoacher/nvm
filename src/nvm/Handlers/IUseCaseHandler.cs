using nvm.Configuration;

namespace nvm.Handlers;

internal interface IUseCaseHandler<TOptions>
{
    Task HandleAsync(Config config, TOptions options);
}