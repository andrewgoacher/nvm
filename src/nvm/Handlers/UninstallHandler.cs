using nvm.ApplicationServices;
using nvm.Configuration;
using nvm.Logging;

namespace nvm.Handlers;

internal class UninstallHandler : HandlerBase<UninstallOptions>
{
    protected override Task OnHandleAsync(Config config, ILogger logger, UninstallOptions options)
    {
        var version = options.Version;
        if (!version.StartsWith("v"))
        {
            version = $"v{version}";
        }

        var isCurrent = NodeVersionInstaller.Uninstall(config, logger, version);

        if (isCurrent)
        {
            logger.LogWarning("This is the current version.  Uninstalling this will unset the current version");
            config.CurrentNodeVersion = "";
        }

        return Task.CompletedTask;
    }
}
