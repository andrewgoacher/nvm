using nvm.Configuration;
using nvm.Node;
using System.IO.Compression;
using System.Text;

namespace nvm.Handlers;

internal class InstallVersionHandler : IUseCaseHandler<InstallOptions>
{
    private HashSet<string> _availableScripts = new HashSet<string>();
    private readonly string[] validExtensions = new[] { ".cmd", ".bat", ".exe" };

    public InstallVersionHandler(Config config)
    {
        var installDir = config.NodeInstallPath;
        foreach (var file in Directory.GetFiles(installDir))
        {
            if (validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (!_availableScripts.Contains(filename))
                {
                    _availableScripts.Add(filename);
                }
            }
        }
    }

    public async Task HandleAsync(Config config, InstallOptions options)
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

    private async Task InstallVersion(NodeClient client, Config config, string version)
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

        // create the missing scripts
        foreach (var file in Directory.GetFiles(versionPath))
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if (validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                if (!_availableScripts.Contains(filename))
                {
                    _availableScripts.Add(filename);
                    Console.WriteLine($"Need to create file {Path.Combine(Directory.GetCurrentDirectory(), filename)}.ps1");
                    await CreateFile(filename, Directory.GetCurrentDirectory());
                }
            }
        }
    }

    private static async Task CreateFile(string command, string dir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("$str = \"\"");
        sb.AppendLine("foreach($item in $args)");
        sb.AppendLine("{");
        sb.AppendLine("$str += $item + \" \"");
        sb.AppendLine("}");

        sb.AppendLine($"nvm run {command} $str");

        await File.WriteAllTextAsync(Path.Combine(dir, $"{command}.ps1"), sb.ToString());
    }
}