using nvm.Configuration;
using System.Text.RegularExpressions;

namespace nvm.Handlers;

using nvm.Console;
using nvm.Logging;
using System;

internal class ListVersionsHandler : IUseCaseHandler<ListOptions>
{
    private static readonly Regex _structureRegex = new Regex(@"(v\d+\.\d+\.\d+)");

    public Task HandleAsync(Config config, ListOptions options)
    {
        var loglevel = options.GetLogLevel();
        var logger = new ConsoleLogger(loglevel);

        var installPath = config.NodeInstallPath;
        var directories = Directory.GetDirectories(installPath);
        var installs = directories.Select(dir => _structureRegex.Match(dir));

        logger.LogInformation("Listing installed versions of node");

        if (installs.Any() == false)
        {
            Console.WriteLine("No installed versions found");
        }
        else
        {
            foreach (var install in installs)
            {
                if (install.Success)
                {
                    var dir = install.Groups[1].Value;
                    if (dir.Equals(config.CurrentNodeVersion, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"{dir} (*)");
                    }
                    else
                    {
                        Console.WriteLine(dir);
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}