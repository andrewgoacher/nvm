using CommandLine;
using nvm.Configuration;
using nvm.Handlers;

namespace nvm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = Config.Load();
        var result = Parser.Default
            .ParseArguments<InstallOptions, ListOptions, UseOptions, RunOptions>(args);

        await result.Handle(config, new InstallVersionHandler(config));
        await result.Handle(config, new ListVersionsHandler());
        await result.Handle(config, new UseVersionHandler());
        await result.Handle(config, new RunHandler());

        config.Save();
    }
}