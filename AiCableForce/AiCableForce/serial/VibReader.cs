using System;
using System.Collections.Generic;
using System.IO;

namespace AiCableForce.serial
{
    public class VibReader
    {
        private readonly string _path;

        public VibReader(string path)
        {
            _path = path;
        }

        public VibData Read()
        {
            var res = new VibData();

            using (var fs = new FileStream(Path.Combine(Path.GetDirectoryName(_path) ?? "",
                Path.GetFileNameWithoutExtension(_path) ?? "") + ".sdb", FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    res.Sdb = (sdb)Converter.BytesToStruct(br.ReadBytes((int)fs.Length), typeof(sdb));
                }
            }

            using (var fs = new FileStream(Path.Combine(Path.GetDirectoryName(_path) ?? "",
                Path.GetFileNameWithoutExtension(_path) ?? "") + ".odb", FileMode.Open, FileAccess.Read))
            {
                var n = fs.Length / sizeof(double);
                res.Odb = new double[n];
                var br = new BinaryReader(fs);
                for (var i = 0; i < n; i++)
                {
                    res.Odb[i] = br.ReadDouble();
                }
            }
            return res;
        }

        public double[] ReadAsciiData()
        {
            var rls = new List<double>();
            using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                            rls.Add(double.Parse(line));
                    }
                }
            }
            return rls.ToArray();
        }
    }
}
