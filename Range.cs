using System;

namespace Minerva.Module
{
    /// <summary>
    /// Describes a float range.
    /// </summary>
    [Serializable]
    public struct Range
    {
        //
        // Summary:
        //     The starting index of the range, where 0 is the first position, 1 is the second,
        //     2 is the third, and so on.
        public float min;

        //
        // Summary:
        //     The end index of the range (not inclusive, unless min == max, range == 0).
        public float max;

        /// <summary>
        /// The length of the range.
        /// </summary>
        public float range => max - min;

        public float value { get => min == max ? min : UnityEngine.Random.Range(min, max); }

        /// <summary>
        /// Constructs a new RangeInt with given start, length values.
        /// </summary>
        /// <param name="min">The start index of the range.</param>
        /// <param name="max">The end index of the range.</param>
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// is given value within the range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsWithin(float value)
        {
            return value <= max && value >= min;
        }


        public override string ToString()
        {
            return $"[{min}, {max}]";
        }

    }
}