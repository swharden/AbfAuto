namespace AbfAutoTests;

internal class SampleAbfs
{
    public static readonly string Folder = @"X:\Software\ABF protocol tests\abfs";
    public static readonly string AutoAnalysisFolder = Path.Combine(Folder, "_autoanalysis");

    public static string[] Paths = Directory.GetFiles(Folder, "*.abf");

    public static void DeleteAutoAnalysisFolder()
    {
        if (Directory.Exists(AutoAnalysisFolder))
            Directory.Delete(AutoAnalysisFolder, true);
    }

    [Test]
    public void Test_SampleAbfs_Located()
    {
        Paths.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Test_EveryProtocol_HasSampleABF()
    {
        string[] protocols = Directory
            .GetFiles(@"L:\Protocols\permanent", "*.pro")
            .Select(x => Path.GetFileName(x))
            .Where(x=>!x.Contains("no ephys"))
            .Select(x => x.Split(" ")[0])
            .Select(NumbersOnly)
            .Distinct()
            .Order()
            .ToArray();

        string[] sampleAbfProtocols = Directory
            .GetFiles(Folder, "*.abf")
            .Select(x => new AbfSharp.ABF(x).Header.Protocol)
            .Select(x => x.Split(" ")[0])
            .Select(NumbersOnly)
            .Distinct()
            .Order()
            .ToArray();

        string[] protocolsNeedingSampleAbfs = protocols
            .Where(x => !sampleAbfProtocols.Contains(x))
            .ToArray();

        if (protocolsNeedingSampleAbfs.Length > 0)
        {
            Assert.Fail($"Every protocol file on X drive needs a sample ABF file in '{Folder}'. " +
                $"Missing protocols: {string.Join(", ", protocolsNeedingSampleAbfs)}");
        }

        Console.WriteLine(string.Join("\n", protocols));
    }

    static string NumbersOnly(string input)
    {
        return new string(input.ToCharArray().Where(char.IsNumber).ToArray());
    }
}
