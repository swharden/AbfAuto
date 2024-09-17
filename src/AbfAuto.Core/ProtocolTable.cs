namespace AbfAuto.Core;

public static class ProtocolTable
{
    private readonly static Dictionary<string, Type> AnalysesByProtocol = new()
    {
        { "0110", typeof(Analyzers.P0110_RMP) },
        { "0111", typeof(Analyzers.P0111_AP) },
        { "0112", typeof(Analyzers.P0113_APGain) },
        { "0113", typeof(Analyzers.P0113_APGain) },
        { "0114", typeof(Analyzers.P0113_APGain) },

        { "0201", typeof(Analyzers.P0201_Memtest) },
        { "0202", typeof(Analyzers.P0202_IV) },

        { "0301", typeof(Analyzers.P0301_APFreqOverTime) },

        { "0405", typeof(Analyzers.P0405_RepeatedMemtest) },
        { "0406", typeof(Analyzers.P0405_RepeatedMemtest) },

        { "0804", typeof(Analyzers.P0804_bAP) },

        { "EEG-3", typeof(Analyzers.PEEG_3Ch) },
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

        using (TemporaryConsoleColor c = new(ConsoleColor.Magenta))
        {
            Console.WriteLine($"WARNING: Protocol '{protocol}' is unknown.");
            Console.WriteLine($"Edit {nameof(ProtocolTable)}.cs to assign it to an existing analysis.");
        }

        object? unknownProtocolInstance = Activator.CreateInstance(typeof(AbfAuto.Core.Analyzers.Unknown));

        if (unknownProtocolInstance is IAnalyzer upi)
            return upi;
        else
            throw new InvalidOperationException();

    }
}
