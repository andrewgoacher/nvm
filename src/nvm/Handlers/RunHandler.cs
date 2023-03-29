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
        var version = options.Version;

        if (string.IsNullOrEmpty(version))
        {
            logger.LogDiagnostic("Version not provided, looking for nvmrc file");

            if (!NVMRcParser.TryGetRcVersion(out version))
            {
                logger.LogDiagnostic("RC file does not exist.  Defaulting to default version");

                version = config.CurrentNodeVersion;
            }
        }

        var installer = new Installer(config, logger);
        installer.Installed += Installer_Installed;

        if (!await installer.CheckInstallAsync(version))
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

        void Installer_Installed(object? sender, Events.InstalledEventArgs e)
        {
            version = e.Version;
        }
    }
}