using nvm.Configuration;
using System.Text.RegularExpressions;

namespace nvm.Handlers;

internal static class UseVersionHandler
{
    private static readonly Regex _structureRegex = new Regex(@"node-(v?\d+\.\d+\.\d+)-win-x64");

    public static Task Handle(UseOptions options, Config config)
    {
        if (!VersionIsInstalled(options.Version, config))
        {
            Console.WriteLine("Specified version is not installed");
        }

        var version = options.Version;

        if (options.Version.StartsWith("v") == false)
        {
            version = $"v{version}";
        }
        config.CurrentNodeVersion = version;

        Console.WriteLine($"Default version set to {version}");

        return Task.CompletedTask;
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