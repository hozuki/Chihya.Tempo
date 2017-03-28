using System;
using System.Collections.Generic;
using System.Linq;

namespace Chihya.Tempo {
    internal sealed class EnergyBuffer {

        public EnergyBuffer(int maxSize) {
            if (maxSize <= 0) {
                throw new ArgumentOutOfRangeException(nameof(maxSize), maxSize, "maxSize should be greater than zero.");
            }
            MaxSize = maxSize;
        }

        public void Add(float value) {
            var values = _values;
            if (values.Count >= MaxSize) {
                values.Remove(0);
            }
            values.Add(value);
            _meanCalculated = false;
            _varianceCalculated = false;
        }

        public void Clear() {
            _values.Clear();
        }

        public int MaxSize { get; }

        public double Mean {
            get {
                if (_meanCalculated) {
                    return _mean;
                }
                var mean = 0d;
                var values = _values;
                if (values.Count > 0) {
                    var sum = 0d;
                    foreach (var value in values) {
                        sum += value;
                    }
                    mean = sum / values.Count;
                }
                _mean = mean;
                _meanCalculated = true;
                return mean;
            }
        }

        public double Variance {
            get {
                if (_varianceCalculated) {
                    return _variance;
                }
                var variance = 0d;
                var values = _values;
                if (values.Count > 0) {
                    var mean = Mean;
                    foreach (var value in values) {
                        variance += (value - mean) * (value - mean);
                    }
                    variance /= values.Count;
                }
                _variance = variance;
                _varianceCalculated = true;
                return variance;
            }
        }

        private readonly List<double> _values = new List<double>();
        private double _mean;
        private double _variance;
        private bool _meanCalculated;
        private bool _varianceCalculated;

    }
}
