using System.Text;

namespace SWHarden.CsvBuilder;

public struct Column
{
    public readonly string Title;
    public readonly string Units;
    public readonly string Comments;
    public readonly double[] Data;

    public Column(string title, string units, string comments, double[] data)
    {
        Title = title;
        Units = units;
        Comments = comments;
        Data = data;
    }
}

public class CsvBuilder
{
    private readonly List<string> HeaderLines = [];
    private readonly List<Column> Columns = new();

    public CsvBuilder()
    {
    }

    public void AddHeaderLine(string line)
    {
        HeaderLines.Add(line);
    }

    public void Add(string title, string units, string comments, double[] data)
    {
        title = string.IsNullOrWhiteSpace(title) ? "---" : title;
        units = string.IsNullOrWhiteSpace(units) ? "---" : units;
        comments = string.IsNullOrWhiteSpace(comments) ? "---" : comments;
        Columns.Add(new Column(title, units, comments, data));
    }

    public void SaveAs(string filePath, bool titles = true, bool units = true, bool comments = true)
    {
        StringBuilder sb = new();

        foreach(string line in HeaderLines)
            sb.AppendLine($"# {line}");

        if (titles)
            sb.AppendLine(string.Join(", ", Columns.Select(x => x.Title)));

        if (units)
            sb.AppendLine(string.Join(", ", Columns.Select(x => x.Units)));

        if (comments)
            sb.AppendLine(string.Join(", ", Columns.Select(x => x.Comments)));

        int maxDataLength = Columns.Select(x => x.Data.Length).Max();
        for (int i = 0; i < maxDataLength; i++)
        {
            for (int j = 0; j < Columns.Count; j++)
            {
                if (i < Columns[j].Data.Length)
                {
                    double value = Columns[j].Data[i];
                    if (double.IsNaN(value))
                    {
                        sb.Append(string.Empty);
                    }
                    else
                    {
                        sb.Append(value.ToString());
                    }
                }

                if (j < Columns.Count - 1)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.AppendLine();
                }
            }
        }

        System.IO.File.WriteAllText(filePath, sb.ToString());
    }
}