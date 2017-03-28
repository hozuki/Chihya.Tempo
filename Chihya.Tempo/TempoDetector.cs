using System;

namespace Chihya.Tempo {
    /// <summary>
    /// The base class of tempo detectors.
    /// </summary>
    public abstract class TempoDetector {

        /// <summary>
        /// Creates a new instance of <see cref="TempoDetector"/>.
        /// </summary>
        /// <param name="data">The raw wave audio data.</param>
        /// <param name="properties">Wave signal properties.</param>
        /// <param name="selectedChannel">Selected channel index. Tempo detection will be performed on this channel.</param>
        protected TempoDetector(byte[] data, SignalProperties properties, int selectedChannel) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }
            if (properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }
            if (selectedChannel < 0) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel must be 0 or above 0.");
            }
            if (selectedChannel >= properties.Channels) {
                throw new ArgumentOutOfRangeException(nameof(selectedChannel), selectedChannel, "Selected channel must be within given channels.");
            }
            RawData = data;
            SignalProperties = properties;
            SelectedChannel = selectedChannel;
        }

        /// <summary>
        /// Gets raw wave audio data. The array stores audio data in its original form,
        /// instead of converting it into a <see cref="float"/> array. For example,
        /// the byte array representing S16 audio data stores <see cref="short"/> data.
        /// This property is readonly.
        /// </summary>
        public byte[] RawData { get; }

        /// <summary>
        /// Gets properties of the given signal. This property is readonly.
        /// </summary>
        public SignalProperties SignalProperties { get; }

        /// <summary>
        /// Gets the selected channel. This property is readonly.
        /// </summary>
        public int SelectedChannel { get; }

        /// <summary>
        /// Detect tempo and returns detection result.
        /// </summary>
        /// <returns>The detecting result.</returns>
        public abstract TempoDetectionResult Detect();

    }
}
