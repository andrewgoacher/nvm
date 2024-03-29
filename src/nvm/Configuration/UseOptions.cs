﻿using CommandLine;

namespace nvm.Configuration;

[Verb("use", HelpText = "Set the specified node version")]
internal class UseOptions : ProgramOptions
{
    [Value(0, Required = true)]
    public string Version { get; set; } = "";
}