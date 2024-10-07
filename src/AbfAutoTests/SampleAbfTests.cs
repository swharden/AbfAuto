namespace AbfAutoTests;

public class SampleAbfTests
{
    [Test]
    public void Test_Analyze_ABF()
    {
        string path = @"X:\Software\ABF protocol tests\abfs\0208 VC IV extended.abf";
        string[] savedFiles = AbfAuto.Analyze.AbfFile(path);
        foreach(string savedFile in savedFiles)
        {
            Console.WriteLine(savedFile);
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
