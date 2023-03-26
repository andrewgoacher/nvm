using nvm.Configuration;
using nvm.Node;
using System.IO.Compression;
using System.Text;
using nvm.Console;

namespace nvm.Handlers;

using nvm.Logging;
using System;

internal class InstallVersionHandler : IUseCaseHandler<InstallOptions>
{
    private readonly HashSet<string> _availableScripts = new HashSet<string>();
    private readonly string[] validExtensions = new[] { ".cmd", ".bat", ".exe" };

    public InstallVersionHandler(Config config)
    {
        GetAvailableScripts(config);
    }

    public async Task HandleAsync(Config config, InstallOptions options)
    {
        var loglevel = options.GetLogLevel();
        var logger = new ConsoleLogger(loglevel);

        if (config.IsNewConfig)
        {
            var directory = Environment.GetEnvironmentVariable(Node.Constants.NvmHomeEnvironmentFlag);
        interaction:
            if (Directory.Exists(directory))
            {
                Console.WriteLine("It looks like there is a previous install containing managed node versions.");
                Console.WriteLine("Would you like to use this install directory?");
                Console.WriteLine("[Y]es or [N]o");

                var response = Console.ReadLine()?.ToLower() ?? "";
                switch (response)
                {
                    case "y" or "yes":
                        {
                            logger.LogInformation("Setting install directory to {0}", directory);
                            config.NodeInstallPath = directory;
                            GetAvailableScripts(config);
                            break;
                        }
                    case "n" or "no":
                        {
                            logger.LogDiagnostic("Opted to not use currently installed node installations.");
                            logger.LogDiagnostic("These will not be managed by nvm-dotnet");
                            break;
                        }
                    default: goto interaction;
                }
            }
        }

        // Check if version already exists?
        // does? Then don't do anything further
        // does not? Then install it

        // Post installation: Check the tools that need to be installed
        // Check if we want to set this version as the current version

        using var client = new NodeClient(config);

        var versionToInstall = options.Version;

        if (Constants.SpecialVersions.Any(ver => options.Version.Equals(ver, StringComparison.OrdinalIgnoreCase)))
        {
            var versions = await client.GetAllNodeVersionsAsync();
            var version = versions.First(version => version.IsLatest);
            logger.LogInformation("Installing latest version of node ({0})", version.Version);
            versionToInstall = version.Version;
        }
        else
        {
            versionToInstall = $"v{options.Version}";
        }

        await InstallVersion(logger, client, config, versionToInstall);

        if (options.Use)
        {
            logger.LogInformation("Setting the current not version to this version");
            config.CurrentNodeVersion = versionToInstall;
        }

        logger.LogInformation("Done");
    }

    private async Task InstallVersion(ILogger logger, NodeClient client, Config config, string version)
    {
        // todo: this naming logic is duplicated in DownloadZipAsync
        // todo: Handle other OS versions
        var versionPath = Path.Combine(config.NodeInstallPath, $"node-{version}-win-x64");
        // todo: Handle path existing?
        if (Directory.Exists(versionPath))
        {
            logger.LogDiagnostic("Version already downloaded nothing to do here.");
            return;
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

        // create the missing scripts
        foreach (var file in Directory.GetFiles(versionPath))
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if (validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                if (!_availableScripts.Contains(filename))
                {
                    _availableScripts.Add(filename);
                    logger.LogDiagnostic($"Need to create file {Path.Combine(config.NodeInstallPath, filename)}.ps1");
                    await CreateFile(filename, config.NodeInstallPath);
                }
            }
        }

        var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
        if (!currentPath.Contains(config.NodeInstallPath))
        {
            currentPath += $";{config.NodeInstallPath}";
            Environment.SetEnvironmentVariable("Path", currentPath, EnvironmentVariableTarget.User);

            currentPath += $";{config.NodeInstallPath}";
            Environment.SetEnvironmentVariable("Path", currentPath, EnvironmentVariableTarget.Process);
        }
    }

    private void GetAvailableScripts(Config config)
    {
        _availableScripts.Clear();
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

    private static async Task CreateFile(string command, string dir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("$str = \"\"");
        sb.AppendLine("foreach($item in $args)");
        sb.AppendLine("{");
        sb.AppendLine("$str += $item + \" \"");
        sb.AppendLine("}");

        sb.AppendLine($"nvm run \"{command} $str\"");

        await File.WriteAllTextAsync(Path.Combine(dir, $"{command}.ps1"), sb.ToString());
    }
}