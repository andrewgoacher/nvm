using nvm.Configuration;
using nvm.Console;
using nvm.Logging;
using nvm.Node;
using System.IO.Compression;

namespace nvm.ApplicationServices;

internal class NodeVersionInstaller
{
    public async Task<string?> ExecuteAsync(Config config, ILogger logger, string version)
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
            Directory.CreateDirectory(path);
            entry.ExtractToFile(filename);
        }
        return versionPath;
    }
}
