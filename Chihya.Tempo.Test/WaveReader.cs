using System;
using System.IO;

namespace Chihya.Tempo.Test {
    public static class WaveReader {

        public static (byte[] data, SignalProperties properties) ReadWaveFile(string fileName) {
            using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read)) {
                return ReadWaveFileStream(fileStream);
            }
        }

        // https://msdn.microsoft.com/en-us/library/ff827591.aspx
        // http://soundfile.sapp.org/doc/WaveFormat/
        public static (byte[] data, SignalProperties properties) ReadWaveFileStream(Stream stream) {
            using (var reader = new BinaryReader(stream)) {
                var chunkID = reader.ReadInt32();
                var fileSize = reader.ReadInt32();
                var riffType = reader.ReadInt32();
                var fmtID = reader.ReadInt32();
                var fmtSize = reader.ReadInt32();
                int fmtCode = reader.ReadInt16();
                int channels = reader.ReadInt16();
                var sampleRate = reader.ReadInt32();
                var fmtAverageBytesPerSecond = reader.ReadInt32();
                int fmtBlockAlign = reader.ReadInt16();
                int bitDepth = reader.ReadInt16();

                if (fmtSize == 18) {
                    // Read any extra values
                    int fmtExtraSize = reader.ReadInt16();
                    reader.ReadBytes(fmtExtraSize);
                }

                var dataID = reader.ReadInt32();
                var dataSize = reader.ReadInt32();

                var data = reader.ReadBytes(dataSize);

                SignalSampleFormat format;
                if (bitDepth == 8) {
                    format = SignalSampleFormat.Unsigned8Bit;
                } else if (bitDepth == 16) {
                    format = SignalSampleFormat.Signed16Bit;
                } else if (bitDepth == 32) {
                    if (fmtCode == 1) {
                        format = SignalSampleFormat.Signed32Bit;
                    } else if (fmtCode == 3) {
                        format = SignalSampleFormat.Float32Bit;
                    } else {
                        throw new NotSupportedException($"Format {fmtCode} is not supported while bit depth = {bitDepth}.");
                    }
                } else {
                    throw new NotSupportedException($"Bit depth is not supported: {bitDepth}.");
                }
                var samplesPerChannel = dataSize / channels / (bitDepth / 8);
                var prop = new SignalProperties(channels, samplesPerChannel, sampleRate, format);

                return (data, prop);
            }
        }

    }
}
