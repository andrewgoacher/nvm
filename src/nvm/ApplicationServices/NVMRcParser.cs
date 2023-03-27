namespace nvm.ApplicationServices;

internal class NVMRcParser
{
    public static bool TryGetRcVersion(out string version)
    {
        var dir = Directory.GetCurrentDirectory();
        var file = Path.Combine(dir, ".nvmrc");

        if (File.Exists(file))
        {
            version = File.ReadAllText(file);
            return true;
        }

        version = "";
        return false;
    }
}
