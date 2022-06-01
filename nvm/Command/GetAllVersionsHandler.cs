//using HtmlAgilityPack;
//using nvm.Config;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace nvm.Command
//{
//    internal sealed class GetAllVersionsHandler : IHandler<CommandLineOptions>
//    {
//        const string NodeDistPath = "https://nodejs.org/dist/";
//        const string NPMDistPath = "";

//        const string VersionRegex = @".*v(\d+)\.(\d+)\.(\d+)\/.*";
//        const string Latest = "latest/";

//        readonly Regex versionRegex = new Regex(VersionRegex);
//        public async Task Handle(CommandLineOptions option)
//        {
//            if (option.ListAll)
//            {
//                Console.WriteLine("Getting all versions of node.");

//                var raw = await GetRawHtml();
//                var html = new HtmlDocument();
//                html.LoadHtml(raw);

//                var pre = html.DocumentNode.SelectNodes("//pre");
//                var nodes = pre.Nodes().ToArray();

//                for (var i = 0; i < nodes.Length - 2;)
//                {
//                    var anchor = nodes[i];
//                    var text = nodes[i + 1];

//                    var version = GetVersion(anchor.InnerText);

//                    if (version.Length > 0)
//                    {


//                        Console.WriteLine($"version: {WriteVersion(version)}");
//                        Console.WriteLine($"Text: - {text.InnerText}");
//                        Console.WriteLine();
//                    }

//                    i += 2;
//                }
//            }
//        }

//        private string WriteVersion(string[] version)
//        {
//            if (version.Length == 1) { return version[0]; }

//            return $"v{version[0]}.{version[1]}.{version[2]}";
//        }

//        private string[] GetVersion(string versionString)
//        {
//            if (versionString.Contains(Latest))
//            {
//                return new[] { "latest" };
//            }

//            var matches = versionRegex.Match(versionString);

//            if (matches.Success)
//            {
//                return new[] { matches.Groups[1].Value, matches.Groups[2].Value, matches.Groups[3].Value };
//            }

//            return new string[0];
//        }

//        private string GetReleaseDate(string date)
//        {
//            return "";
//        }


//        private static async Task<string?> GetRawHtml()
//        {
//            using var client = new HttpClient();
//            var response = await client.GetStringAsync(NodeDistPath);

//            return response;
//        }
//    }
//}
