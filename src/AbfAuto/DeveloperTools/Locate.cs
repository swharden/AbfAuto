namespace AbfAuto.DeveloperTools;

public class Locate
{
    public static string[] FindAbfsWithProtocol(string directory, string match)
    {
        List<string> matchingPaths = [];

        string[] abfPaths = Directory.GetFiles(directory, "*.abf", SearchOption.AllDirectories);
        foreach (string abfPath in abfPaths)
        {
            AbfSharp.ABF abf = new(abfPath, preloadSweepData: false);
            if (abf.Header.Protocol.Contains(match))
            {
                matchingPaths.Add(abfPath);
            }
        }

        return [.. matchingPaths];
    }
}
