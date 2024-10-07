namespace AbfAutoTests;

internal class AbfSharpTests
{
    [Test]
    public void Test_SampleRate()
    {
        string path = @"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\2024-10-04\2024_10_04_EEG_0000.abf";
        AbfSharp.ABF abf = new(path);
        abf.SampleRate.Should().Be(1000);
    }
}
