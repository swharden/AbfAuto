namespace AbfAutoTests;

public class SampleAbfTests
{
    [Test]
    public void Test_Analyze_AllABFs()
    {
        SampleAbfs.DeleteAutoAnalysisFolder();
        SampleAbfs.Paths.ToList().ForEach(x => AbfAuto.Analyze.AbfFile(x));
        HtmlReport.AutoAnalysisImages();
    }
}
