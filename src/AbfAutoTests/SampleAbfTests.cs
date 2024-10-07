namespace AbfAutoTests;

public class SampleAbfTests
{
    [Test]
    public void Test_Analyze_ABF()
    {
        foreach(string path in Directory.GetFiles(@"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\2024-10-04", "*.abf"))
        {
            string[] savedFiles = AbfAuto.Analyze.AbfFile(path);
            foreach (string savedFile in savedFiles)
            {
                Console.WriteLine(savedFile);
            }
        }
    }

    [Test]
    public void Test_Analyze_AllABFs()
    {
        SampleAbfs.DeleteAutoAnalysisFolder();
        SampleAbfs.Paths.ToList().ForEach(x => AbfAuto.Analyze.AbfFile(x));
        HtmlReport.AutoAnalysisImages();
    }
}
