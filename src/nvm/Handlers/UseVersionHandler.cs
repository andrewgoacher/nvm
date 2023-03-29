using nvm.Configuration;
using nvm.Node;

namespace nvm.Handlers;

using nvm.Logging;
using System;

internal class UseVersionHandler : HandlerBase<UseOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, UseOptions options)
    {
        using var client = new NodeClient(config, logger);
        var version = await client.GetVersionFromVersionAsync(options.Version);

        if (config.CurrentNodeVersion.Equals(version, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Current version is already set.");
            return;
        }

        var installer = new Installer(config, logger);

        if (!await installer.CheckInstallAsync(version))
        {
            return;
        }

        config.CurrentNodeVersion = version;
    }
}