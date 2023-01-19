using System;

namespace Minerva.Module
{

    /// <summary>
    /// Describes an integer range, see <see cref="UnityEngine.RangeInt"/>
    /// </summary>
    [Serializable]
    public struct RangeInt
    {
        //
        // Summary:
        //     The starting index of the range, where 0 is the first position, 1 is the second,
        //     2 is the third, and so on.
        public int min;

        //
        // Summary:
        //     The end index of the range (not inclusive, unless min == max, range == 0).
        public int max;

        /// <summary>
        /// The length of the range.
        /// </summary>
        public int length => max - min;

        public int value { get => min == max ? min : UnityEngine.Random.Range(min, max); }

        /// <summary>
        /// Constructs a new RangeInt with given start, length values.
        /// </summary>
        /// <param name="min">The start index of the range.</param>
        /// <param name="max">The end index of the range.</param>
        public RangeInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }


        public static implicit operator UnityEngine.RangeInt(RangeInt ri)
        {
            return new UnityEngine.RangeInt(ri.min, ri.length);
        }

        public static implicit operator RangeInt(UnityEngine.RangeInt ri)
        {
            return new RangeInt(ri.start, ri.length);
        }
    }
}