using System;

namespace Chihya.Tempo {
    public sealed class TempoDetectionResult {

        public TempoDetectionResult(float bpm, TimeSpan beatStart) {
            if (bpm < 0) {
                throw new ArgumentOutOfRangeException(nameof(bpm), bpm, "BPM must be no less than 0.");
            }
            if (beatStart < TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(beatStart), beatStart, "Beat must start after a song begins.");
            }
            BeatStart = beatStart;
            BeatsPerMinute = bpm;
        }

        public float BeatsPerMinute { get; }

        public TimeSpan BeatStart { get; }

    }
}
