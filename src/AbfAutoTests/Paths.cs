namespace AbfAutoTests;

internal class Paths
{
    public static readonly string SampleAbfFolder = @"X:\Software\ABF protocol tests\abfs";
    public static readonly string SampleAutoAnalysisFolder = Path.Combine(SampleAbfFolder, "_autoanalysis");

    public static string[] SampleAbfs = Directory.GetFiles(SampleAbfFolder, "*.abf");

    public static void DeleteAutoAnalysisFolder()
    {
        if (Directory.Exists(SampleAutoAnalysisFolder))
            Directory.Delete(SampleAutoAnalysisFolder, true);
    }

    [Test]
    public void Test_SampleAbfs_Located()
    {
        SampleAbfs.Should().NotBeNullOrEmpty();
    }
}
