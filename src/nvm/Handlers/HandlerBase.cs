using nvm.Configuration;
using nvm.Logging;

namespace nvm.Handlers;

internal abstract class HandlerBase<TOptions> : IUseCaseHandler<TOptions>
    where TOptions : ProgramOptions
{
    public async Task HandleAsync(Config config, TOptions options)
    {
        var logLevel = options.GetLogLevel();
        var logger = new Console.ConsoleLogger(logLevel);

        OnCheckForNewConfig(config, logger);
        await OnHandleAsync(config, logger, options);
    }

    protected abstract Task OnHandleAsync(Config config, ILogger logger, TOptions options);

    private static void OnCheckForNewConfig(Config config, ILogger logger)
    {
        if (!config.IsNewConfig)
        {
            return;
        }

        logger.LogInformation("This is a new nvm install. Checking for previous NVM installations");

        var nvmHomeEnv = Environment.GetEnvironmentVariable(Node.Constants.NvmHomeEnvironmentFlag);
        if (string.IsNullOrEmpty(nvmHomeEnv))
        {
            logger.LogInformation("No NVM install found.");
            return;
        }

        if (Directory.Exists(nvmHomeEnv))
        {
            logger.LogInformation("Previous version of NVM found at {0}", nvmHomeEnv);
        }

        if (TryGetAnswerForNVM(logger, out var adopt))
        {
            if (adopt)
            {
                logger.LogInformation("Setting install directory to {0}", nvmHomeEnv);
                logger.LogInformation("Config will still be found at {0}", config.NodeInstallPath);
                config.NodeInstallPath = nvmHomeEnv;
            }
        } 
        else
        {
            System.Console.WriteLine("You gave an invalid answer.  Defaulting to current install path");
        }
    }

    private static bool TryGetAnswerForNVM(ILogger logger, out bool adoptOldNVM)
    {
        System.Console.WriteLine("It looks like there is a previous install containing managed node versions.");
        System.Console.WriteLine("Would you like to use this install directory?");
        System.Console.WriteLine("[Y]es or [N]o");

        var response = System.Console.ReadLine()?.ToLower() ?? "";

        switch (response)
        {
            case "yes" or "y":
                {
                    logger.LogInformation("User has opted to adopt the current install directory");
                    adoptOldNVM = true;
                    return true;
                }
            case "no" or "n":
                {
                    logger.LogInformation("User has opted not to adopt the current install directory");
                    adoptOldNVM = false;
                    return true;
                }
        }

        adoptOldNVM = false;
        return false;
    }
}
