using System;

namespace Chihya.Tempo {
    public sealed class SignalProperties {

        public SignalProperties(int channels, int samplesPerChannel, int sampleRate, SignalSampleFormat format) {
            if (channels <= 0) {
                throw new ArgumentOutOfRangeException(nameof(channels), channels, "Channels must be positive.");
            }
            if (samplesPerChannel <= 0) {
                throw new ArgumentOutOfRangeException(nameof(samplesPerChannel), samplesPerChannel, "Length must be positive.");
            }
            if (sampleRate <= 0) {
                throw new ArgumentOutOfRangeException(nameof(sampleRate), SampleRate, "Sample rate must be positive.");
            }
            Channels = channels;
            SamplesPerChannel = samplesPerChannel;
            SampleRate = sampleRate;
            Format = format;
        }

        public int Channels { get; }

        public int SamplesPerChannel { get; }

        public int SampleRate { get; }

        public SignalSampleFormat Format { get; }

        public int SampleCount => Channels * SamplesPerChannel;

    }
}
