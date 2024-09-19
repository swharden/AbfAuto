namespace AbfAuto;

public static class ProtocolTable
{
    private readonly static Dictionary<string, Type> AnalysesByProtocol = new()
    {
        { "0110", typeof(Analyzers.RMP) },
        { "0111", typeof(Analyzers.APFirst) },
        { "0112", typeof(Analyzers.APGainDual) },
        { "0113", typeof(Analyzers.APGainDual) },
        { "0114", typeof(Analyzers.APGainDual) },

        { "0201", typeof(Analyzers.Memtest) },
        { "0202", typeof(Analyzers.IVWithTail) },

        { "0301", typeof(Analyzers.APFreqOverTime) },

        { "0405", typeof(Analyzers.MemtestRepeated) },
        { "0406", typeof(Analyzers.MemtestRepeated) },

        { "0501", typeof(Analyzers.OptoMeanEpoch2) },
        { "0503", typeof(Analyzers.OptoMeanEpoch1) },
        { "0509", typeof(Analyzers.OptoMeanEpoch1) },

        { "0804", typeof(Analyzers.BAP) },

        { "EEG-3", typeof(Analyzers.InVivo3) },
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

        using (TemporaryConsoleColor c = new(ConsoleColor.White, ConsoleColor.Magenta))
        {
            Console.WriteLine($"WARNING: Protocol '{protocol}' has no matching analyzer.");
            Console.WriteLine($"Edit {nameof(ProtocolTable)}.cs to assign this protocol to an analyzer.");
        }

        object? unknownProtocolInstance = Activator.CreateInstance(typeof(Analyzers.Unknown));

        if (unknownProtocolInstance is IAnalyzer upi)
            return upi;
        else
            throw new InvalidOperationException();

    }
}
