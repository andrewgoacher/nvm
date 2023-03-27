using nvm.Configuration;
using nvm.Node;

namespace nvm.Handlers;

using nvm.ApplicationServices;
using nvm.Logging;

internal class InstallVersionHandler : HandlerBase<InstallOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, InstallOptions options)
    {
        using var client = new NodeClient(config, logger);
        var version = await client.GetVersionFromVersion(options.Version);

        var versionPath = await NodeVersionInstaller.InstallAsync(config, logger, version);
        if (string.IsNullOrEmpty(versionPath))
        {
            logger.LogWarning("No version path returned.  Installer exited early");
            return;
        }

        var nodeToolsInstaller = new NodeToolsInstaller();
        await nodeToolsInstaller.InstallAsync(config, logger, versionPath);
    }
}