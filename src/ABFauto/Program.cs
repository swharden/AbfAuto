using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABFauto
{
    class Program
    {
        static void Main(string[] args)
        {
            DevStartup();
        }

        static void DevStartup()
        {
            string abfFolder = System.IO.Path.GetFullPath("../../../../dev/abfs");

            Debug.WriteLine($"DEV ABF folder: {abfFolder}");
            string[] abfPaths = AbfsInFolder(abfFolder);
            foreach (string abfPath in abfPaths)
                AutoAnalyze(abfPath);
        }

        static void AutoAnalyze(string abfFilePath)
        {
            Debug.WriteLine("");
            Debug.WriteLine($"Auto-analyzing: {System.IO.Path.GetFileName(abfFilePath)}");
            var abf = new ABFsharp.ABF(abfFilePath, ABFsharp.ABF.Preload.AllSweeps);
            Debug.WriteLine(abf);

            // protocol code is the thing before the first space
            Debug.WriteLine($"Protocol: '{abf.info.protocol}'");
            string protocolCode = abf.info.protocol;
            if (protocolCode.Contains(" "))
                protocolCode = protocolCode.Split(' ')[0];
            Debug.WriteLine($"Protocol code: '{protocolCode}'");

            switch (protocolCode)
            {
                case "0111":
                    Analyze.ApFirst(abf);
                    break;

                case "0112":
                case "0113":
                case "0114":
                    Analyze.ApGain(abf);
                    break;

                case "0201":
                    Analyze.Memtest(abf);
                    break;

                case "0202":
                    Analyze.IvStep(abf);
                    break;

                case "0203":
                    Analyze.IvRamp(abf);
                    break;

                case "0401":
                case "0402":
                    Analyze.MemtestOverTime(abf);
                    break;

                case "p0601":
                    break;

                default:
                    throw new NotImplementedException();
            }

        }

        static string[] AbfsInFolder(string abfFolderPath)
        {
            if (System.IO.Directory.Exists(abfFolderPath))
            {
                string[] abfPaths = System.IO.Directory.GetFiles(abfFolderPath, "*.abf");
                for (int i = 0; i < abfPaths.Length; i++)
                    abfPaths[i] = System.IO.Path.GetFullPath(abfPaths[i]);
                return abfPaths;
            }
            else
            {
                return new string[] { };
            }
        }

    }
}
