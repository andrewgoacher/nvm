namespace nvm.Configuration
{
    internal class Config
    {
        public string NodeDistUrl { get; }
        public string NodeInstallPath { get; }

        public Config(string url, string installPath)
        {
            NodeDistUrl = url;
            NodeInstallPath = installPath;
        }
    }
}
