using nvm.Configuration;
using nvm.Node;
using System.IO.Compression;

namespace nvm.Handlers;

internal static class InstallVersionHandler
{
    public static async Task Handle(InstallOptions options, Config config)
    {
        using var client = new NodeClient(config);

        if (options.Version.Equals("node", StringComparison.OrdinalIgnoreCase) || options.Version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            var versions = await client.GetAllNodeVersionsAsync();
            var version = versions.First(version => version.IsLatest);
            Console.WriteLine($"Installing latest version of node ({version.Version})");
            await InstallVersion(client, config, version.Version);
        }
        else
        {
            await InstallVersion(client, config, $"v{options.Version}");
        }
    }

    private static async Task InstallVersion(NodeClient client, Config config, string version)
    {
        // todo: this naming logic is duplicated in DownloadZipAsync
        // todo: Handle other OS versions
        var versionPath = Path.Combine(config.NodeInstallPath, $"node-{version}-win-x64");
        // todo: Handle path existing?
        if (Directory.Exists(versionPath))
        {
            Console.WriteLine("Version already downloaded");
            return;
        }

        Directory.CreateDirectory(versionPath);
        Console.WriteLine($"Creating directory {versionPath}");
        Console.Write("Downloading zip");

        using var ms = new MemoryStream();
        using var progress = new ConsoleProgress();
        await client.DownloadZipAsync(version, ms, progress);

        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (entry.CompressedLength == 0) { continue; }

            Console.WriteLine($"Extracting {entry.Name}");
            var filename = Path.Combine(config.NodeInstallPath, entry.FullName);
            var path = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(path);
            entry.ExtractToFile(filename);
        }
    }
}