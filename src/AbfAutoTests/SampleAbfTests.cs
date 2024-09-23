using System.Text;

namespace AbfAutoTests;

public class SampleAbfTests
{
    [Test]
    public void Test_Analyze_AllABFs()
    {
        Paths.DeleteAutoAnalysisFolder();
        foreach (string path in Paths.SampleAbfs)
        {
            AbfAuto.Analyze.AbfFile(path);
        }

        StringBuilder sb = new();
        foreach (string pngPath in Directory.GetFiles(Paths.SampleAutoAnalysisFolder, "*.png"))
        {
            string pngFilename = Path.GetFileName(pngPath);
            string abfFilename = Path.GetFileNameWithoutExtension(pngPath).Split("_AbfAuto_")[0];
            string abfPath = Path.Combine(Paths.SampleAbfFolder, abfFilename) + ".abf";

            sb.AppendLine($"<h3 class='mt-5'>{abfFilename}</h3>");
            sb.AppendLine($"<div><code>{abfPath}</code></div>");
            sb.AppendLine($"<a href='{pngFilename}'><img src='{pngFilename}' style='max-width: 100%'></a>");
            sb.AppendLine($"<hr class='my-5 invisible'>");
        }

        string html = """
            <!doctype html>
            <html lang="en">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>AbfAuto Test</title>
                <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
              </head>
              <body>
                <div class='container' style='max-width: 800px;'>
                CONTENT
                </div>
                <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
              </body>
            </html>
            """.Replace("CONTENT", sb.ToString());

        string saveAs = Path.Combine(Paths.SampleAutoAnalysisFolder, "index.html");
        File.WriteAllText(saveAs, html);
        Console.WriteLine(saveAs);
    }
}
