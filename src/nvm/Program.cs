using CommandLine;
using nvm.Configuration;

namespace nvm;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<InstallOptions, UseOptions>(args)
            .WithParsed<InstallOptions>(HandleInstall)
            .WithParsed<UseOptions>(HandleUse);
    }

    private static void HandleInstall(InstallOptions options)
    {

    }

    private static void HandleUse(UseOptions options)
    {

    }
}