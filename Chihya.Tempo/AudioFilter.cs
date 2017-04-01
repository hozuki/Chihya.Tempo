namespace Chihya.Tempo {
    public abstract class AudioFilter {

        public abstract float NewSample(float sampleValue);

        public abstract float Value { get; }

    }
}
