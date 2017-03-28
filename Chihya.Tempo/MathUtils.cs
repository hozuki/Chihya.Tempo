namespace Chihya.Tempo {
    /// <summary>
    /// Math utilities.
    /// </summary>
    internal static class MathUtils {

        /// <summary>
        /// Clamps a single value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum of the range.</param>
        /// <param name="max">The maximum of the range.</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp(float value, float min, float max) {
            return value < min ? min : (value > max ? max : value);
        }

    }
}
