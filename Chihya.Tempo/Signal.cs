using System;
using System.Linq;

namespace Chihya.Tempo {
    internal sealed class Signal {

        public Signal(byte[] data, int channels, int length, int sampleRate, SignalSampleFormat format, int selectedChannel, AudioFilter filter = null) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }
            if (selectedChannel < 0) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel must be non-negative.");
            }
            var properties = new SignalProperties(channels, length, sampleRate, format);
            if (selectedChannel >= properties.Channels) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel does not exist.");
            }
            Properties = properties;
            _rawData = CheckAndConvert(data);
            SelectedChannel = selectedChannel;
            Filter = filter;
        }

        public Signal(byte[] data, SignalProperties properties, int selectedChannel, AudioFilter filter = null) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }
            if (properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }
            if (selectedChannel < 0) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel must be non-negative.");
            }
            Properties = properties;
            _rawData = CheckAndConvert(data);
            SelectedChannel = selectedChannel;
            Filter = filter;
        }

        public int SelectedChannel { get; }

        public SignalProperties Properties { get; }

        public float[] RawData => _rawData;

        public AudioFilter Filter { get; }

        public float GetEnergy() {
            return _rawData.Sum(f => f * f);
        }

        public float[] GetEnergies(int groupSize) {
            if (groupSize <= 0) {
                throw new ArgumentException("Group size must be greater than 0.", nameof(groupSize));
            }
            var length = _rawData.Length;
            if (groupSize > length) {
                throw new ArgumentException("Group size must be smaller or equal to data size.", nameof(groupSize));
            }
            var waveCount = length / groupSize;
            if (waveCount * groupSize < length) {
                ++waveCount;
            }
            var energies = new float[waveCount];
            var left = length;
            var rawData = _rawData;
            var filter = Filter;
            for (var i = 0; i < waveCount; ++i) {
                var n = left < groupSize ? left : groupSize;
                var energy = 0f;
                for (var j = 0; j < n; ++j) {
                    var f = rawData[i * groupSize + j];
                    if (filter != null) {
                        f = filter.NewSample(f);
                    }
                    energy += f * f;
                }
                left -= n;
                energies[i] = energy;
            }
            return energies;
        }

        public static int NumberOfSamples(TimeSpan duration, int samplingRate) {
            return NumberOfSamples(duration.TotalSeconds, samplingRate);
        }

        public static int NumberOfSamples(TimeSpan duration, int samplingRate, int channels) {
            return NumberOfSamples(duration.TotalSeconds, samplingRate, channels);
        }

        public static int NumberOfSamples(double durationInSeconds, int samplingRate) {
            return NumberOfSamples(durationInSeconds, samplingRate, 1);
        }

        public static int NumberOfSamples(double durationInSeconds, int samplingRate, int channels) {
            return (int)(durationInSeconds * channels * samplingRate);
        }

        public static TimeSpan DurationFromSamples(long numberOfSamples, int samplingRate) {
            return DurationFromSamples(numberOfSamples, samplingRate, 1);
        }

        public static TimeSpan DurationFromSamples(long numberOfSamples, int samplingRate, int channels) {
            var seconds = numberOfSamples / (double)samplingRate / channels;
            return TimeSpan.FromSeconds(seconds);
        }

        public static int GetSampleSize(SignalSampleFormat format) {
            return GetSampleSize(format, 1);
        }

        public static int GetSampleSize(SignalSampleFormat format, int channels) {
            int size;
            switch (format) {
                case SignalSampleFormat.Unsigned8Bit:
                case SignalSampleFormat.Signed8Bit:
                    size = 8;
                    break;
                case SignalSampleFormat.Signed16Bit:
                    size = 16;
                    break;
                case SignalSampleFormat.Signed32Bit:
                case SignalSampleFormat.Float32Bit:
                    size = 32;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
            size *= channels;
            return size;
        }

        private float[] CheckAndConvert(byte[] data) {
            float[] result;
            var properties = Properties;
            switch (properties.Format) {
                case SignalSampleFormat.Unsigned8Bit: {
                        var expectedLength = properties.SamplesPerChannel * properties.Channels * 1;
                        if (data.Length < expectedLength) {
                            throw new ArgumentException("Length of data is shorter than expected.", nameof(data));
                        }
                        var step = 1 * properties.Channels;
                        result = new float[properties.SamplesPerChannel];
                        for (int i = 0, p = 0; i < expectedLength; i += step, ++p) {
                            result[p] = (float)(data[i] - 128) / 128;
                        }
                        break;
                    }
                case SignalSampleFormat.Signed8Bit: {
                        var expectedLength = properties.SamplesPerChannel * properties.Channels * 1;
                        if (data.Length < expectedLength) {
                            throw new ArgumentException("Length of data is shorter than expected.", nameof(data));
                        }
                        var step = 1 * properties.Channels;
                        result = new float[properties.SamplesPerChannel];
                        for (int i = 0, p = 0; i < expectedLength; i += step, ++p) {
                            result[p] = (float)data[i] / 128;
                        }
                        break;
                    }
                case SignalSampleFormat.Signed16Bit: {
                        var expectedLength = properties.SamplesPerChannel * properties.Channels * 2;
                        if (data.Length < expectedLength) {
                            throw new ArgumentException("Length of data is shorter than expected.", nameof(data));
                        }
                        var step = 2 * properties.Channels;
                        result = new float[properties.SamplesPerChannel];
                        for (int i = 0, p = 0; i < expectedLength; i += step, ++p) {
                            var s = BitConverter.ToInt16(data, i);
                            result[p] = (float)s / 65536;
                        }
                        break;
                    }
                case SignalSampleFormat.Signed32Bit: {
                        var expectedLength = properties.SamplesPerChannel * properties.Channels * 4;
                        if (data.Length < expectedLength) {
                            throw new ArgumentException("Length of data is shorter than expected.", nameof(data));
                        }
                        var step = 4 * properties.Channels;
                        result = new float[properties.SamplesPerChannel];
                        for (int i = 0, p = 0; i < expectedLength; i += step, ++p) {
                            var f = BitConverter.ToInt32(data, i);
                            result[p] = (float)f / 2147483648;
                        }
                        break;
                    }
                case SignalSampleFormat.Float32Bit: {
                        var expectedLength = properties.SamplesPerChannel * properties.Channels * 4;
                        if (data.Length < expectedLength) {
                            throw new ArgumentException("Length of data is shorter than expected.", nameof(data));
                        }
                        var step = 4 * properties.Channels;
                        result = new float[properties.SamplesPerChannel];
                        for (int i = 0, p = 0; i < expectedLength; i += step, ++p) {
                            var f = BitConverter.ToSingle(data, i);
                            result[p] = MathUtils.Clamp(f, 0, 1);
                        }
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        private readonly float[] _rawData;

    }
}
