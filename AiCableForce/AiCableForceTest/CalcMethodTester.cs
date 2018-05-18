using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AiCableForce.calc;
using AiCableForce.graphic;
using AiCableForce.serial;
using NUnit.Framework;

namespace AiCableForceTest
{
    [TestFixture]
    class CalcMethodTester
    {
        [Test]
        public void TestFft()
        {
            new Graphic("TEST").Show(new[] { 1.0, 2, 3, 4 }, new[] { 3.0, 4, 5, 6 },TGraphic.Original);
        }

        [Test]
        public void TestPeriodgram()
        {
            var d = new VibReader("Test_941B_20170214135049918_TRI.dat").ReadAsciiData();
            var fs = 1024;
            var nfft = 1024;
            double[] y, f;
            FourierTransform.Periodogram(d, fs, nfft, out y, out f);
            new Graphic("TEST").Show(f, y, TGraphic.FreqSpectrum, fs, nfft);
        }
    }
}
