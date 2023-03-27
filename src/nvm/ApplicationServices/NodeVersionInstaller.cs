using nvm.Configuration;
using nvm.Console;
using nvm.Logging;
using nvm.Node;
using System.IO.Compression;

namespace nvm.ApplicationServices;

internal class NodeVersionInstaller
{
    public static async Task<string?> InstallAsync(Config config, ILogger logger, string version)
    {
        using var client = new NodeClient(config, logger);

        // todo: this naming logic is duplicated in DownloadZipAsync
        // todo: Handle other OS versions
        var versionPath = Path.Combine(config.NodeInstallPath, $"{version}");

        if (Directory.Exists(versionPath))
        {
            logger.LogDiagnostic("Version already downloaded nothing to do here.");
            return null;
        }

        Directory.CreateDirectory(versionPath);
        logger.LogDiagnostic($"Creating directory {versionPath}");
        logger.LogInformation("Downloading zip");

        using var ms = new MemoryStream();
        using var progress = new ConsoleProgress();
        await client.DownloadZipAsync(version, ms, progress);

        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (entry.CompressedLength == 0) { continue; }

            logger.LogDiagnostic($"Extracting {entry.Name}");
            var filename = Path.Combine(config.NodeInstallPath, entry.FullName);
            var path = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(path)) { continue; }

            Directory.CreateDirectory(path);
            entry.ExtractToFile(filename);
        }
        return versionPath;
    }

    public static async Task<bool> CheckInstallAsync(Config config, ILogger logger, string version)
    {
        var versionEnumerator = new InstalledVersionEnumerator();
        var installedVersions = versionEnumerator.GetInstalledVersions(config.NodeInstallPath);

        if (!installedVersions.Any(ver => ver.Equals(version, StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogWarning("This selected version is not installed.  Prompting user to install.");

            if (TryGetAnswerForInstall(logger, out var shouldInstall) && shouldInstall)
            {
                await InstallAsync(config, logger, version);
            }
            else
            {
                logger.LogWarning("This version is not installed.");
                return false;
            }
        }

        return true;
    }

    public static bool Uninstall(Config config, ILogger logger, string version)
    {
        var dir = Path.Combine(config.NodeInstallPath, version);

        if (!Directory.Exists(dir))
        {
            logger.LogWarning("The version {0} is not installed", version);
            return false;
        }

        Directory.Delete(dir, true);

        return config.CurrentNodeVersion.Equals(version, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetAnswerForInstall(ILogger logger, out bool shouldInstall)
    {
        System.Console.WriteLine("The version you have selected is not installe.");
        System.Console.WriteLine("Would you like to install this version?");
        System.Console.WriteLine("[Y]es or [N]o");

        var response = System.Console.ReadLine()?.ToLower() ?? "";

        switch (response)
        {
            case "yes" or "y":
                {
                    logger.LogInformation("User has opted to install this version");
                    shouldInstall = true;
                    return true;
                }
            case "no" or "n":
                {
                    logger.LogInformation("User has opted not to install this version");
                    shouldInstall = false;
                    return true;
                }
        }

        shouldInstall = false;
        return false;
    }
}