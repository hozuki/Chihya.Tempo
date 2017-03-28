// The design of AutoSensitivity and the values of its two coefficients originally come from Accord.NET:
// https://github.com/accord-net/framework/blob/master/Sources/Accord.Audition/Beat/EnergyBeatDetector.cs
// Copyright © César Souza, 2009-2017

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chihya.Tempo {
    public sealed class EnergyTempoDetector : TempoDetector {

        public EnergyTempoDetector(byte[] data, SignalProperties properties, EnergyTempoDetectorConfig config)
            : base(data, properties, config.SelectedChannel) {
            _energyBuffer = new EnergyBuffer(config.BufferSize);
            Config = config;
        }

        public EnergyTempoDetectorConfig Config { get; }

        /// <summary>
        /// Runs the detection procedure and gets the detection result.
        /// </summary>
        /// <remarks>This method is based on sound energy.
        /// See http://archive.gamedev.net/archive/reference/programming/features/beatdetection/index.html for the theoratical background.
        /// </remarks>
        /// <returns></returns>
        public override TempoDetectionResult Detect() {
            var config = Config;
            var signal = new Signal(RawData, SignalProperties, SelectedChannel);
            var energies = signal.GetEnergies(config.ChunkSize);    
            var energyBuffer = _energyBuffer;
            energyBuffer.Clear();

            var autoSensivity = AutoSensivity;
            // Sample index of each beat. (estimation!)
            var beatSampleIndices = new List<int>();

            var coeff1 = config.SensitivityVarianceCoeff;
            var coeff2 = config.SensitivityVarianceConst;
            for (var i = 0; i < energies.Length; i++) {
                var signalEnergy = energies[i];
                var averageEnergy = energyBuffer.Mean;
                if (autoSensivity) {
                    Sensivity = coeff1 * energyBuffer.Variance + coeff2;
                }
                energyBuffer.Add(signalEnergy);
                if (signalEnergy > Sensivity * averageEnergy) {
                    beatSampleIndices.Add(i * config.ChunkSize);
                }
            }

            if (beatSampleIndices.Count <= 1) {
                return new TempoDetectionResult(0, TimeSpan.Zero);
            }
            var sampleRate = SignalProperties.SampleRate;
            var beatLocations = beatSampleIndices.Select(sampleIndex => Signal.DurationFromSamples(sampleIndex, sampleRate)).ToArray();
            var deltas = new List<TimeSpan>();

            // Assuming BPM < N (see BetweenBeatsThreshold).
            // beatLocations[0] is always 0.
            var prevLocation = beatLocations[1];
            var threshold = config.BetweenBeatsThreshold;
            var threshold2 = config.InBeatThreshold;
            for (var i = 2; i < beatLocations.Length; ++i) {
                if (beatLocations[i] - beatLocations[i - 1] > threshold2 && beatLocations[i] - prevLocation > threshold) {
                    deltas.Add(beatLocations[i] - prevLocation);
                    prevLocation = beatLocations[i];
                }
            }

            var bpms = deltas.Select(time => (float)(60 / time.TotalSeconds)).ToArray();
            var beatStart = beatLocations[1];
            var bpm = GuessBpm(bpms, config.BpmProximity);
            var result = new TempoDetectionResult(bpm, beatStart);
            return result;
        }

        /// <summary>
        /// Sets/gets whether auto sensitivity is enabled.
        /// </summary>
        public bool AutoSensivity { get; set; } = true;

        public double Sensivity {
            get {
                return _sensivity;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentException("Sensivity must be greater than 0.", nameof(value));
                }
                _sensivity = value;
                AutoSensivity = false;
            }
        }

        private static float GuessBpm(float[] bpms, float proximity) {
            var bin = new FuzzyBin();
            foreach (var bpm in bpms) {
                bin.Add(bpm);
            }
            var mostPossibleKV = bin.OrderByDescending(kv => kv.Value).First();
            var possibleBPMs = bin.Where(kv => kv.Value > mostPossibleKV.Value * proximity).ToArray();
            var totalFrequencies = possibleBPMs.Sum(kv => kv.Value);
            var guessedBPM = possibleBPMs.Sum(kv => kv.Key * kv.Value / (float)totalFrequencies);
            return guessedBPM;
        }

        private readonly EnergyBuffer _energyBuffer;
        private double _sensivity;

    }
}
