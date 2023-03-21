using nvm.Configuration;
using System.Text.RegularExpressions;

namespace nvm.Handlers;

internal static class ListVersionsHandler
{
    private static readonly Regex _structureRegex = new Regex(@"node-(v\d+\.\d+\.\d+)-win-x64");

    public static Task Handle(ListOptions options, Config config)
    {
        var installPath = config.NodeInstallPath;
        var directories = Directory.GetDirectories(installPath);
        var installs = directories.Select(dir => _structureRegex.Match(dir));

        Console.WriteLine("Listing installed versions of node");
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
                    Console.WriteLine(install.Groups[1].Value);
                }
            }
        }

        return Task.CompletedTask;
    }
}