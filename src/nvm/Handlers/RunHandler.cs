using nvm.Configuration;
using System.Diagnostics;

namespace nvm.Handlers
{
    internal static class RunHandler
    {
        public static async Task Handle(RunOptions options, Config config)
        {
            var version = options.Version;

            if (string.IsNullOrEmpty(version))
            {
                version = config.CurrentNodeVersion;
            }

            if (version.StartsWith("v") == false)
            {
                version = $"v{version}";
            }

            version = $"node-{version}-win-x64";

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
}