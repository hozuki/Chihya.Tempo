using System;
using System.IO;

namespace Chihya.Tempo.Test {
    internal class Program {

        private static int Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Usage: Tempo <wav file>");
                return 0;
            }

            var fileName = args[0];
            if (!File.Exists(fileName)) {
                Console.WriteLine("File '{0}' doesn't exist.", fileName);
                return 1;
            }


            Console.WriteLine($"File: {fileName}");
            var wav = WaveReader.ReadWaveFile(fileName);
            var config = EnergyTempoDetectorConfig.For44KHz;
            var filter = new ButterworthFilter(5000, 44100);
            //AudioFilter filter = null;
            var detector = new EnergyTempoDetector(wav.data, wav.properties, config, filter);
            var tempo = detector.Detect();
            Console.WriteLine($"Starts from {tempo.BeatStart}, BPM is {tempo.BeatsPerMinute:0.00}");

            Console.ReadKey();

            return 0;
        }

    }
}
