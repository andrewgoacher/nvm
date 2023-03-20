﻿using CommandLine;
using nvm.Configuration;
using nvm.Node;
using System.IO.Compression;

namespace nvm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = Config.Load();

        var result = Parser.Default
            .ParseArguments<InstallOptions, UseOptions>(args);

        await result
            .WithParsedAsync<InstallOptions>(async options => await HandleInstall(options, config));

        await result
            .WithParsedAsync<UseOptions>(options => HandleUse(options, config));

        config.Save();
    }

    private static async Task HandleInstall(InstallOptions options, Config config)
    {
        using var client = new NodeClient(config);

        if (options.Version.Equals("node", StringComparison.OrdinalIgnoreCase) || options.Version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            var versions = await client.GetAllNodeVersionsAsync();
            var version = versions.First(version => version.IsLatest);
            Console.WriteLine($"Installing latest version of node ({version.Version})");
            await InstallVersion(client, config, version.Version);
        } else
        {
            await InstallVersion(client, config, $"v{options.Version}");
        }
    }

    private static async Task InstallVersion(NodeClient client, Config config, string version)
    {
        var versionPath = Path.Combine(config.NodeInstallPath, version);
        // todo: Handle path existing?
        if (Directory.Exists(versionPath)) 
        {
            Console.WriteLine("Version already downloaded");
            return;
        }

        Directory.CreateDirectory(versionPath);
        Console.WriteLine($"Creating directory {versionPath}");
        Console.WriteLine("Downloading zip");
        using var stream = await client.DownloadZipAsync(version);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (entry.CompressedLength == 0) { continue; }

            Console.WriteLine($"Extracting {entry.Name}");
            var filename = Path.Combine(versionPath, entry.FullName);
            var path = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(path);
            entry.ExtractToFile(filename);
        }
    }

    private static async Task HandleUse(UseOptions options, Config config)
    {

    }
}