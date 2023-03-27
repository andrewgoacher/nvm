using nvm.Configuration;
using nvm.Node;

namespace nvm.Handlers;

using nvm.ApplicationServices;
using nvm.Logging;
using System;

internal class UseVersionHandler : HandlerBase<UseOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, UseOptions options)
    {
        using var client = new NodeClient(config, logger);
        var version = await client.GetVersionFromVersion(options.Version);

        if (config.CurrentNodeVersion.Equals(version, StringComparison.OrdinalIgnoreCase) == false)
        {
            logger.LogInformation("Current version is already set.");
            return;
        }

        if (!await NodeVersionInstaller.CheckInstallAsync(config, logger, version))
        {
            return;
        }

        config.CurrentNodeVersion = version;
    }
}