using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AiCableForce.calc;
using AiCableForce.graphic;
using AiCableForce.serial;

namespace AiCableForce
{
    class Program
    {
        private static List<GraphData> _graphDatum = new List<GraphData>();

        private static List<VibData> _datum = new List<VibData>();

        static void Main(string[] args)
        {
            while (true)
            {
                var line = Console.ReadLine() ?? "".Trim().ToLower();
                var cmd = line.Split()[0];
                var para = line.Split().Skip(1).ToList();
                switch (cmd)
                {
                    case "load":
                        Load(para);
                        break;
                    case "clr":
                        Clr();
                        break;
                    case "fft":
                        Fft();
                        break;
                    case "muti":
                        Multi();
                        break;
                    case "q":
                    case "quit":
                    case "exit":
                        break;
                    default:
                        Console.WriteLine("DONE -- nothing to do.");
                        break;
                }
                Console.WriteLine("DONE");
                Thread.Sleep(500);
            }
        }

        static void Load(IEnumerable<string> paths)
        {
            _datum.Clear();
            foreach (var path in paths)
            {
                var data = new VibReader(path).Read();
                new Graphic("ORG").ShowOrg(data.Odb, data.Sdb.diSampleFreq);
                _datum.Add(data);
            }
        }

        private static void Clr()
        {
            _datum.Clear();
        }

        static void Fft()
        {
            var d = _datum.Last();
            double[] f, y;
            FourierTransform.Forward(d.Odb, d.Sdb.diSampleFreq, null, out y, out f);
            new Graphic("FFT").Show(f, y, TGraphic.FreqSpectrum, d.Sdb.diSampleFreq, d.Odb.Length);
        }

        static void Multi()
        {
            var d = _datum.Last();
            double[] f, y;
            FourierTransform.Forward(d.Odb, d.Sdb.diSampleFreq, null, out y, out f);
            FourierTransform.SelfPower(y, 5, out y);
            new Graphic("SELF POWER").Show(f, y, TGraphic.FreqSpectrum, d.Sdb.diSampleFreq, d.Odb.Length);

        }
    }
}
