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

        await result
            .WithParsedAsync<InstallOptions>(async options => await InstallVersionHandler.Handle(options, config));

        await result
            .WithParsedAsync<ListOptions>(async options => await ListVersionsHandler.Handle(options, config));

        await result
            .WithParsedAsync<UseOptions>(async options => await UseVersionHandler.Handle(options, config));

        await result
            .WithParsedAsync<RunOptions>(async options => await RunHandler.Handle(options, config));

        config.Save();
    }
}