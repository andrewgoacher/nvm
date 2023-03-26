using nvm.Configuration;
using nvm.Node;
using System.Text.RegularExpressions;

namespace nvm.Handlers;

using System;
internal class UseVersionHandler : IUseCaseHandler<UseOptions>
{
    private static readonly Regex _structureRegex = new Regex(@"(v?\d+\.\d+\.\d+)");

    public async Task HandleAsync(Config config, UseOptions options)
    {

        var version = options.Version;

        if (options.Version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            using var client = new NodeClient(config);
            var versions = await client.GetAllNodeVersionsAsync();
            var latestVersion = versions.First(version => version.IsLatest);
            Console.WriteLine($"Using latest version ({latestVersion.Version})");

            version = latestVersion.Version;
        }
        else if (options.Version.StartsWith("v") == false)
        {
            version = $"v{version}";
        }

        if (!VersionIsInstalled(version, config))
        {
            Console.WriteLine("Specified version is not installed");
            return;
        }

        config.CurrentNodeVersion = version;

        Console.WriteLine($"Default version set to {version}");
    }

    // todo: This is repeated from list version handler.
    private static bool VersionIsInstalled(string version, Config config)
    {
        var installPath = config.NodeInstallPath;
        var directories = Directory.GetDirectories(installPath);
        var installs = directories.Select(dir => _structureRegex.Match(dir));

        if (installs.Any() == false)
        {
            return false;
        }
        else
        {
            foreach (var install in installs)
            {
                if (install.Success)
                {
                    var ver = install.Groups[1].Value;
                    if (version.Equals(ver))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}