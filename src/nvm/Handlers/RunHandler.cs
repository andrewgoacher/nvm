using nvm.ApplicationServices;
using nvm.Configuration;
using nvm.Logging;
using nvm.Node;
using System.Diagnostics;

namespace nvm.Handlers;

internal class RunHandler : HandlerBase<RunOptions>
{
    protected override async Task OnHandleAsync(Config config, ILogger logger, RunOptions options)
    {
        using var client = new NodeClient(config, logger);
        var version = options.Version;

        if (string.IsNullOrEmpty(version))
        {
            logger.LogInformation("Version not provided, looking for nvmrc file");

            if (!NVMRcParser.TryGetRcVersion(out version))
            {
                logger.LogInformation("RC file does not exist.  Defaulting to default version");

                version = config.CurrentNodeVersion;
            }
        }

        version = await client.GetVersionFromVersion(options.Version);

        if (!await NodeVersionInstaller.CheckInstallAsync(config, logger, version))
        {
            logger.LogError("The expected version is not installed and user opted not to install it.");
            return;
        }

        var info = new ProcessStartInfo(version);
        info.UseShellExecute = false;
        info.RedirectStandardOutput = false;

        // hack: todo: Need to handle this way better than I am right now
        var file = options.Command.Substring(0, options.Command.IndexOf(" "));
        var args = options.Command.Substring(options.Command.IndexOf(" "));

        var ext = file.Equals("node") ? ".exe" : ".cmd";

        // todo: sort this so it's not .cmd
        info.FileName = Path.Combine(config.NodeInstallPath, version, $"{file}{ext}");
        info.Arguments = args;

        Process.Start(info);
    }
}