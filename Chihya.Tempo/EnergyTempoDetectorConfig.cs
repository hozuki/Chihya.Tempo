using System;

namespace Chihya.Tempo {
    /// <summary>
    /// The configuration class for <see cref="EnergyTempoDetector"/>.
    /// </summary>
    public sealed class EnergyTempoDetectorConfig {

        /// <summary>
        /// Creates a new instance of <see cref="EnergyTempoDetectorConfig"/>.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        /// <param name="bufferSize">The energy buffer size.</param>
        /// <param name="selectedChannel">Selected channel index.</param>
        /// <param name="varCoeff">The coefficient of energy variance for sensitivity.</param>
        /// <param name="varConst">The constant for sensitivity.</param>
        /// <param name="inBeatThreshold">Time threshold in each beat.</param>
        /// <param name="betweenBeatsThreshold">Time threshold between beats.</param>
        /// <param name="bpmProximity"></param>
        public EnergyTempoDetectorConfig(int chunkSize, int bufferSize, int selectedChannel, double varCoeff, double varConst,
            TimeSpan inBeatThreshold, TimeSpan betweenBeatsThreshold, float bpmProximity) {
            if (chunkSize <= 0) {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, "Chunk size must be greater than 0.");
            }
            if (bufferSize <= 0) {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "Buffer size must be greater than 0.");
            }
            if (selectedChannel < 0) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel must be integers beginning from 0.");
            }
            if (inBeatThreshold <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(inBeatThreshold), inBeatThreshold, "Threshold in each beat must be greater than 0.");
            }
            if (betweenBeatsThreshold <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(betweenBeatsThreshold), betweenBeatsThreshold, "Threshold between beats must be greater than 0.");
            }
            if (bpmProximity <= 0 || bpmProximity > 1) {
                throw new ArgumentOutOfRangeException(nameof(bpmProximity), bpmProximity, "BPM proximity must be a value in (0, 1].");
            }
            ChunkSize = chunkSize;
            BufferSize = bufferSize;
            SelectedChannel = selectedChannel;
            SensitivityVarianceCoeff = varCoeff;
            SensitivityVarianceConst = varConst;
            InBeatThreshold = inBeatThreshold;
            BetweenBeatsThreshold = betweenBeatsThreshold;
            BpmProximity = bpmProximity;
        }

        /// <summary>
        /// The chunk size of signals, in number of samples. During detection, audio data is splitted into chunks.
        /// For each chunk, the energy of the whole chunk is calculated and considered as a whole.
        /// It is affected by audio sample rate. The value suggested in various sources is 1024.
        /// However, after some testing, 512 should be the best value for 44kHz audio data.
        /// </summary>
        public int ChunkSize { get; }

        /// <summary>
        /// The size of the energy buffer (a circular linked list).
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Selected channel of audio data. For example, for a stereo audio, 0 is for left channel and 1 is for right channel.
        /// </summary>
        public int SelectedChannel { get; }

        /// <summary>
        /// The coefficient of energy variance, used in auto sensitivity adjustment.
        /// </summary>
        public double SensitivityVarianceCoeff { get; }

        /// <summary>
        /// The constant, used in auto sensitivity adjustment.
        /// </summary>
        public double SensitivityVarianceConst { get; }

        /// <summary>
        /// A <see cref="TimeSpan"/> used to filter peak energy bursts that are too close to their previous ones.
        /// It is affected by the actual tempo of audio data. 0.04s is sufficient to handle music with BPM below 200.
        /// </summary>
        public TimeSpan InBeatThreshold { get; }

        /// <summary>
        /// A <see cref="TimeSpan"/> used to filter possible beats.
        /// It is affected by the actual tempo of audio data. 0.3s (60 / 200) is sufficient to handle music with BPM below 200.
        /// </summary>
        public TimeSpan BetweenBeatsThreshold { get; }

        /// <summary>
        /// The proximity of a filtered BPM to the most possible BPM, between 0 and 1. It should be a value greater than 0.6 to exclude invalid BPM values after the first filtering.
        /// The suggested value is 0.7.
        /// </summary>
        public float BpmProximity { get; }

        /// <summary>
        /// Gets a clone of this <see cref="EnergyTempoDetectorConfig"/>.
        /// </summary>
        /// <returns>The clone of this <see cref="EnergyTempoDetectorConfig"/>.</returns>
        public EnergyTempoDetectorConfig Clone() {
            return new EnergyTempoDetectorConfig(ChunkSize, BufferSize, SelectedChannel, SensitivityVarianceCoeff, SensitivityVarianceConst,
                InBeatThreshold, BetweenBeatsThreshold, BpmProximity);
        }

        /// <summary>
        /// A set of configuration values which is useful for 44kHz audio data.
        /// </summary>
        public static readonly EnergyTempoDetectorConfig For44KHz = new EnergyTempoDetectorConfig(512, 512, 0, -0.0025714, 1.5142857,
            TimeSpan.FromSeconds(0.04), TimeSpan.FromSeconds(60d / 200), 0.8f);

    }
}
