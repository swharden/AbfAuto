using AbfAuto.DeveloperTools;

namespace AbfAuto;

/// <summary>
/// Logic here determines which analyzer to apply to an ABF according to its protocol.
/// </summary>
public static class ProtocolTable
{
    /// <summary>
    /// The recommended analyzer to use based on the starting characters of the protocol file filename.
    /// After creating new analysis classes, add them to this table to enable them to be used by the auto-analyzer.
    /// </summary>
    private readonly static Dictionary<string, Type> AnalyzerTable = new()
    {
        { "0110", typeof(Analyzers.RMP) },
        { "0111", typeof(Analyzers.APFirst) },
        { "0112", typeof(Analyzers.APGainDual) },
        { "0113", typeof(Analyzers.APGainDual) },
        { "0114", typeof(Analyzers.APGainDual) },

        { "0201", typeof(Analyzers.Memtest) },
        { "0202", typeof(Analyzers.IVWithTail) },
        { "0208", typeof(Analyzers.IVStepEnd) },

        { "0301", typeof(Analyzers.APFreqOverTime) },

        { "0405", typeof(Analyzers.MemtestRepeated) },
        { "0406", typeof(Analyzers.MemtestRepeated) },
        { "0426", typeof(Analyzers.NmdaOverTime) },

        { "0501", typeof(Analyzers.OptoMeanEpoch2) },
        { "0503", typeof(Analyzers.OptoMeanEpoch1) },
        { "0509", typeof(Analyzers.OptoMeanEpoch1) },

        { "0804", typeof(Analyzers.BAP) },

        { "EEG-3", typeof(Analyzers.InVivo3) },
    };

    /// <summary>
    /// Return the recommended analyzer for the given ABF file
    /// </summary>
    public static IAnalyzer GetAnalyzer(AbfSharp.ABF abf)
    {
        string protocol = Path.GetFileNameWithoutExtension(abf.Header.AbfFileHeader.sProtocolPath);

        foreach (string key in AnalyzerTable.Keys)
        {
            if (protocol.StartsWith(key))
            {
                object? instance = Activator.CreateInstance(AnalyzerTable[key]);
                if (instance is IAnalyzer analyzer)
                    return analyzer;
                else
                    throw new InvalidOperationException($"{instance} is does not inherit {nameof(IAnalyzer)}");
            }
        };

        using TemporaryConsoleColor c = new(ConsoleColor.White, ConsoleColor.Magenta);
        Console.WriteLine($"WARNING: Protocol '{protocol}' has no matching analyzer.");
        Console.WriteLine($"Edit {nameof(ProtocolTable)}.cs to assign this protocol to an analyzer.");
        return new Analyzers.Unknown();
    }
}
