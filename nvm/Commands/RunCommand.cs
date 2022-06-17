using nvm.Configuration;
using System.CommandLine;
using System.Diagnostics;

namespace nvm.Commands
{
    internal class RunCommand : Command
    {
        public RunCommand() : base(
            "run", 
            "Runs npm, npx or node")
        {
            var versionOption = new Option<string>("--target", "the version to run, when absent, uses the default version");
            var commandArgument = new Argument<string>();
            commandArgument.FromAmong("npm", "node", "npx");

            AddArgument(commandArgument);
            AddOption(versionOption);

            this.TreatUnmatchedTokensAsErrors = false;

            this.SetHandler((string command, string version) =>
            {
                
                var runVersion = string.IsNullOrEmpty(version) ?
                    Config.CurrentNodeVersion :
                    SanitiseVersionName(version);

                var path = Path.Combine(Config.NodeInstallPath!, runVersion!, GetCommand(command));

                var args = Environment.GetCommandLineArgs();

                var argsToTake = string.IsNullOrEmpty(version) ?
                    args.Skip(3) :
                    args.Skip(4);

                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = string.Join(" ", argsToTake),
                    UseShellExecute = false,
                });

                proc!.WaitForExit();

            }, commandArgument, versionOption);
        }

        private static string GetCommand(string command)
        {
            return command switch
            {
                "node" => "node.exe",
                _ => $"{command}.cmd"
            };
        }

        private static string SanitiseVersionName(string version)
        {
            return version.StartsWith("v") ? version : $"v{version}";
        }
    }
}
