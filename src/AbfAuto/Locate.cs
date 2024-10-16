using AbfSharp;

namespace AbfAuto;
public static class Locate
{
    public static string[] AbfsWithProtocol(string folder, string protocol)
    {
        List<string> paths = [];

        foreach (string path in Directory.GetFiles(folder, "*.abf", SearchOption.AllDirectories))
        {
            ABF abf = new(path, preloadSweepData: false);
            if (abf.Header.Protocol.Contains(protocol, StringComparison.OrdinalIgnoreCase))
            {
                paths.Add(path);
            }
        }

        return [.. paths];
    }
}
