using nvm.Configuration;
using nvm.Console;
using nvm.Logging;

namespace nvm.Handlers;

internal class UninstallHandler : IUseCaseHandler<UninstallOptions>
{
    public Task HandleAsync(Config config, UninstallOptions options)
    {
        var loglevel = options.GetLogLevel();
        var logger = new ConsoleLogger(loglevel);

        var version = options.Version;
        if (!version.StartsWith("v"))
        {
            version = $"v{version}";
        }

        var dir = Path.Combine(config.NodeInstallPath, version);

        if (!Directory.Exists(dir))
        {
            logger.LogWarning("The version {0} is not installed", version);
            return Task.CompletedTask;
        }

        if (config.CurrentNodeVersion.Equals(version, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("This is the current version.  Uninstalling this will unset the current version");
            config.CurrentNodeVersion = "";
        }
        Directory.Delete(dir, true);

        return Task.CompletedTask;
    }
}
