using nvm.Configuration;

namespace nvm.Handlers;

using nvm.ApplicationServices;
using nvm.Logging;
using System;

internal class ListVersionsHandler : HandlerBase<ListOptions>
{

    protected override Task OnHandleAsync(Config config, ILogger logger, ListOptions options)
    {
        logger.LogInformation("Listing installed versions of node");
        var enumerator = new InstalledVersionEnumerator();

        var installs = enumerator.GetInstalledVersions(config.NodeInstallPath);

        if (!installs.Any())
        {
            Console.WriteLine("No installed versions found");
            return Task.CompletedTask;
        }

        foreach(var install in installs )
        {
            if (install.Equals(config.CurrentNodeVersion))
            {
                Console.Write(install);
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" *");
                Console.ForegroundColor = currentColor;
            }
            else
            {
                Console.WriteLine(install);
            }
        }

        return Task.CompletedTask;
    }
}