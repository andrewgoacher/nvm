using System.Text.RegularExpressions;

namespace nvm.ApplicationServices;

internal class InstalledVersionEnumerator
{
    private static readonly Regex _structureRegex = new Regex(@"v(\d+\.\d+\.\d+)");

    public IEnumerable<string> GetInstalledVersions(string installerpath)
    {
        var directories = Directory.GetDirectories(installerpath);
        var installs = directories
            .Select(dir => _structureRegex.Match(dir))
            .Where(regex => regex.Success)
            .Select(regex => GetParts(regex.Groups[1].Value));

        return installs.OrderByDescending(x => x.Item1)
            .ThenByDescending(x => x.Item2)
            .ThenByDescending(x => x.Item3)
            .Select(x => $"v{x.Item1}.{x.Item2}.{x.Item3}");
    }

    private static (int,int,int) GetParts(string version)
    {
        var parts = version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        return (Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
    }
}
