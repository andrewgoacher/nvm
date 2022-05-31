using CommandLine;
using nvm.Config;

await Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs())
    .WithParsedAsync<CommandLineOptions>(async options => Console.WriteLine($"All set: {options.ListAll}"));