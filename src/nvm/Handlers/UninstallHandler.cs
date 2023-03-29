using nvm.Configuration;
using nvm.Logging;
using nvm.Node;

namespace nvm.Handlers;

internal class UninstallHandler : HandlerBase<UninstallOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, UninstallOptions options)
    {
        var installer = new Installer(config, logger);
        installer.Uninstalled += Installer_Uninstalled;

        await installer.UninstallAsync(options.Version);

        void Installer_Uninstalled(object? sender, Events.UninstalledEventArgs e)
        {
            if (e.CurrentVersion)
            {
                logger.LogWarning("This is the current version.  Uninstalling this will unset the current version");
                config.CurrentNodeVersion = "";
            }
        }
    }
}
