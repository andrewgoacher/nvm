using nvm.Configuration;
using nvm.Console;
using nvm.Events;
using nvm.Logging;
using System.IO.Compression;
using System.Text;

namespace nvm.Node;

internal class Installer
{
    private readonly string[] _validExtensions = new[] { ".cmd", ".bat", ".exe" };

    private Config _config;
    private ILogger _logger;

    public event EventHandler<UninstalledEventArgs> Uninstalled;
    public event EventHandler<InstalledEventArgs> Installed;

    public Installer(Config config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<string?> InstallNodeAsync(string version)
    {
        using var client = new NodeClient(_config, _logger);
        var parsedVersion = await client.GetVersionFromVersionAsync(version);

        var path = Path.Combine(_config.NodeInstallPath, parsedVersion);

        if (Directory.Exists(path))
        {
            _logger.LogDiagnostic("Version already downloaded. Skipping.");
            return null;
        }

        _logger.LogDiagnostic("Creating directory {0}", parsedVersion);
        Directory.CreateDirectory(path);
        _logger.LogInformation("Downloading zip contents and extracting");

        using var ms = new MemoryStream();
        using var progress = new ConsoleProgress();
        await client.DownloadZipAsync(parsedVersion, ms, progress);

        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (entry.CompressedLength == 0) { continue; }

            _logger.LogDiagnostic($"Extracting {entry.Name}");
            var entryName = entry.FullName;
            var root = entryName.Substring(0, entryName.IndexOf('/'));
            entryName = entryName.Replace(root, parsedVersion);
            var filename = Path.Combine(_config.NodeInstallPath, entryName);
            var rootPath = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(rootPath)) { continue; }

            Directory.CreateDirectory(rootPath);
            entry.ExtractToFile(filename);
        }

        Installed?.Invoke(this, new InstalledEventArgs
        {
            Version = parsedVersion,
        });

        return parsedVersion;
    }

    public async Task InstallToolsAsync(string versionPath)
    {
        _logger.LogInformation("Checking to see if any node tools need to be installed");
        var availableScripts = GetAvailableScripts(_config);

        var generateFiles = false;

        // create the missing scripts
        foreach (var file in Directory.GetFiles(versionPath))
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if (_validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                if (!availableScripts.Contains(filename))
                {
                    availableScripts.Add(filename);
                    _logger.LogDiagnostic($"Need to create file {Path.Combine(_config.NodeInstallPath, filename)}.ps1");
                    await CreateFile(filename, _config.NodeInstallPath);
                    generateFiles = true;
                }
            }
        }

        if (generateFiles)
        {
            _logger.LogInformation("Additional tools files have been generated");
        }

        _logger.LogInformation("Checking to see if the users PATH needs updating");

        var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;

        if (string.IsNullOrEmpty(currentPath))
        {
            _logger.LogError("PATH is empty.");
        }

        var pathParts = currentPath
            .Split(";", StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (pathParts.Any(p => p.Equals(_config.NodeInstallPath, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogDiagnostic("PATH contains install location.  Nothing more to do here");
            return;
        }

        _logger.LogInformation("Adding {0} into the PATH for the current user", _config.NodeInstallPath);

        pathParts.Add(_config.NodeInstallPath);
        Environment.SetEnvironmentVariable(
            "Path", 
            string.Join(";", pathParts),
            EnvironmentVariableTarget.User);
    }

    public async Task<bool> CheckInstallAsync(string version)
    {
        var versionEnumerator = new InstalledVersionEnumerator();
        var installedVersions = versionEnumerator.GetInstalledVersions(_config.NodeInstallPath);

        if (!installedVersions.Any(ver => ver.Equals(version, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("This selected version is not installed.  Prompting user to install.");

            if (TryGetAnswerForInstall(_logger, out var shouldInstall) && shouldInstall)
            {
                await InstallNodeAsync(version);
            }
            else
            {
                _logger.LogWarning("This version is not installed.");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> UninstallAsync(string version)
    {
        using var client = new NodeClient(_config, _logger);
        var parsedVersion = await client.GetVersionFromVersionAsync(version);

        var directory = Path.Combine(_config.NodeInstallPath, parsedVersion);

        if (!Directory.Exists(directory))
        {
            _logger.LogWarning("The version {0} is not installed", parsedVersion);
            return false;
        }

        _logger.LogInformation("Uninstalled successfully");

        Directory.Delete(directory, true );

        Uninstalled?.Invoke(this, new UninstalledEventArgs
        {
            CurrentVersion = _config.CurrentNodeVersion.Equals(parsedVersion, StringComparison.OrdinalIgnoreCase)
        });

        return true;
    }

    private HashSet<string> GetAvailableScripts(Config config)
    {
        var scripts = new HashSet<string>();
        var installDir = config.NodeInstallPath;
        foreach (var file in Directory.GetFiles(installDir))
        {
            if (_validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (!scripts.Contains(filename))
                {
                    scripts.Add(filename);
                }
            }
        }

        return scripts;
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
