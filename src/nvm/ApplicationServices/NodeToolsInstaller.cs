using nvm.Configuration;
using nvm.Logging;
using System.Text;

namespace nvm.ApplicationServices;

internal class NodeToolsInstaller
{
    private readonly string[] validExtensions = new[] { ".cmd", ".bat", ".exe" };

    public NodeToolsInstaller()
    {

    }

    public async Task ExecuteAsync(Config config, ILogger logger, string versionPath)
    {
        logger.LogInformation("Checking to see if any node tools need to be installed");
        var availableScripts = GetAvailableScripts(config);

        var generateFiles = false;

        // create the missing scripts
        foreach (var file in Directory.GetFiles(versionPath))
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if (validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
            {
                if (!availableScripts.Contains(filename))
                {
                    availableScripts.Add(filename);
                    logger.LogDiagnostic($"Need to create file {Path.Combine(config.NodeInstallPath, filename)}.ps1");
                    await CreateFile(filename, config.NodeInstallPath);
                    generateFiles = true;
                }
            }
        }

        if (generateFiles)
        {
            logger.LogInformation("Additional tools files have been generated");
        }

        logger.LogInformation("Checking to see if the users PATH needs updating");

        var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;
        if (!currentPath?.Contains(config.NodeInstallPath) == false)
        {
            currentPath += $";{config.NodeInstallPath}";
            Environment.SetEnvironmentVariable("Path", currentPath, EnvironmentVariableTarget.User);

            logger.LogInformation("Added the tools path into the environment");
        }
    }

    private HashSet<string> GetAvailableScripts(Config config)
    {
        var scripts = new HashSet<string>();
        var installDir = config.NodeInstallPath;
        foreach (var file in Directory.GetFiles(installDir))
        {
            if (validExtensions.Any(ext => ext.Equals(Path.GetExtension(file))))
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
}
