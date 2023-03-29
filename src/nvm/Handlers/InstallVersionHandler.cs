using nvm.Configuration;
using nvm.Node;

namespace nvm.Handlers;

using nvm.Logging;

internal class InstallVersionHandler : HandlerBase<InstallOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, InstallOptions options)
    {
        var installer = new Installer(config, logger);
        var path = await installer.InstallNodeAsync(options.Version);

        if (!string.IsNullOrEmpty(path))
        {
            await installer.InstallToolsAsync(Path.Combine(config.NodeInstallPath, path));

            if (options.Use)
            {
                config.CurrentNodeVersion = path;
            }
        }
    }
}