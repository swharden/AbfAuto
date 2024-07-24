using ScottPlot.Control;

namespace AbfAuto.Core;

public static class AnalyzerLookup
{
    private readonly static Dictionary<string, Type> AnalysesByProtocol = new()
    {
        { "0201", typeof(Analyzers.P0201_Memtest) },
        { "0202", typeof(Analyzers.P0202_IV) },
        { "0110", typeof(Analyzers.P0110_RMP) },
        { "0111", typeof(Analyzers.P0111_AP) },
    };

    public static IAnalyzer GetAnalysis(AbfSharp.ABF abf)
    {
        string protocol = Path.GetFileNameWithoutExtension(abf.Header.AbfFileHeader.sProtocolPath);

        foreach (string key in AnalysesByProtocol.Keys)
        {
            if (protocol.StartsWith(key))
            {
                object? inst = Activator.CreateInstance(AnalysesByProtocol[key]);

                if (inst is IAnalyzer ian)
                    return ian;
                else
                    throw new InvalidOperationException($"{inst} is does not inherit {nameof(IAnalyzer)}");
            }
        };

        object? unknownProtocolInstance = Activator.CreateInstance(typeof(AbfAuto.Core.Analyzers.Unknown));

        if (unknownProtocolInstance is IAnalyzer upi)
            return upi;
        else
            throw new InvalidOperationException();

    }
}
