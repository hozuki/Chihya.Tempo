using System;
using System.Collections;
using System.Collections.Generic;
// Change this to System.Single if you want to.
using ElementType = System.Double;

namespace Chihya.Tempo {
    /// <summary>
    /// A fuzzy bin, grouping floating point numbers (float/double) by nearest integers.
    /// For each record, the key is those numbers' integer values, and the value is its showing frequency.
    /// </summary>
    internal sealed class FuzzyBin : IEnumerable<KeyValuePair<int, int>> {

        /// <summary>
        /// Adds a value into this bin.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(ElementType value) {
            var v = (int)Math.Round(value);
            if (_frequencies.ContainsKey(v)) {
                ++_frequencies[v];
            } else {
                _frequencies[v] = 1;
            }
        }

        /// <summary>
        /// Removes a value from the bin.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(ElementType value) {
            var v = (int)Math.Round(value);
            if (_frequencies.ContainsKey(v)) {
                --_frequencies[v];
                if (_frequencies[v] == 0) {
                    _frequencies.Remove(v);
                }
            }
        }

        /// <summary>
        /// Gets the frequency of a integer value. This indexer is readonly.
        /// </summary>
        /// <param name="key">The integer value.</param>
        /// <returns>The frequency of that integer value.</returns>
        public int this[int key] {
            get {
                return _frequencies[key];
            }
        }

        /// <summary>
        /// Gets the frequency of the integer value of a floating point number. This indexer is readonly.
        /// </summary>
        /// <param name="key">The floating point number.</param>
        /// <returns>The frequency of that number.</returns>

        public int this[ElementType key] {
            get {
                var k = (int)Math.Round(key);
                return this[k];
            }
        }

        /// <summary>
        /// Clear all items.
        /// </summary>
        public void Clear() {
            _frequencies.Clear();
        }

        /// <summary>
        /// Returns a enumerator for this bin.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<int, int>> GetEnumerator() {
            return _frequencies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private Dictionary<int, int> _frequencies = new Dictionary<int, int>();

    }
}
