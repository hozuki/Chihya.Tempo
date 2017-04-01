using System;

namespace Chihya.Tempo {
    /// <summary>
    /// A Butterworth signal filter.
    /// Found at http://stackoverflow.com/questions/8079526/lowpass-and-high-pass-filter-in-c-sharp, which points to
    /// http://www.musicdsp.org/archive.php?classid=3#38.
    /// </summary>
    public sealed class ButterworthFilter : AudioFilter {

        public ButterworthFilter(float cutoffFrequency, int sampleRate, float resonance = 1, BandPass bandPass = BandPass.LowPass) {
            if (cutoffFrequency <= 0) {
                throw new ArgumentOutOfRangeException(nameof(cutoffFrequency), "Cutoff frequency should be greater than zero.");
            }
            if (sampleRate < 0) {
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "Sample rate should be greater than zero.");
            }
            if (resonance < 0.1f || resonance > Sqrt2) {
                throw new ArgumentOutOfRangeException(nameof(resonance), $"Resonance should be between 0.1 and {Sqrt2}.");
            }
            CutoffFrequency = cutoffFrequency;
            SampleRate = sampleRate;
            Resonance = resonance;
            BandPass = bandPass;

            float a1, a2, a3, b1, b2, c;
            switch (bandPass) {
                case BandPass.LowPass:
                    c = 1.0f / (float)Math.Tan(Math.PI * cutoffFrequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = 2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (1.0f - c * c) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                case BandPass.HighPass:
                    c = (float)Math.Tan(Math.PI * cutoffFrequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = -2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (c * c - 1.0f) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bandPass), bandPass, null);
            }
            _a1 = a1;
            _a2 = a2;
            _a3 = a3;
            _b1 = b1;
            _b2 = b2;
            _c = c;
        }

        public float CutoffFrequency { get; }

        public int SampleRate { get; }

        public float Resonance { get; }

        public BandPass BandPass { get; }

        public override float NewSample(float sampleValue) {
            var newOutput = _a1 * sampleValue + _a2 * _inputHistory[0] + _a3 * _inputHistory[1] - _b1 * _outputHistory[0] - _b2 * _outputHistory[1];

            _inputHistory[1] = _inputHistory[0];
            _inputHistory[0] = sampleValue;

            _outputHistory[2] = _outputHistory[1];
            _outputHistory[1] = _outputHistory[0];
            _outputHistory[0] = newOutput;

            return newOutput;
        }

        public override float Value => _outputHistory[0];

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        private readonly float[] _inputHistory = new float[2];
        private readonly float[] _outputHistory = new float[3];
        private readonly float _a1, _a2, _a3, _b1, _b2, _c;

    }
}
