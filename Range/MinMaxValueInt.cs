using System;
using UnityEngine.Serialization;

namespace Minerva.Module
{
    /// <summary>
    /// a struct representing a int value based on given value in unity, could be constant or random
    /// </summary>
    [Serializable]
    public struct MinMaxValueInt
    {
        public bool randomize;
        [FormerlySerializedAs("count")]
        [DisplayIf(nameof(randomize), false)] public int value;
        [DisplayIf(nameof(randomize))] public RangeInt range;

        public int Value => randomize ? range.min : value;
        public int Min => randomize ? range.min : value;
        public int Max => randomize ? range.max : value;


        public readonly float Lerp(float t)
        {
            return randomize ? range.Lerp(t) : value;
        }


        public static implicit operator int(MinMaxValueInt minMaxValue)
        {
            return minMaxValue.Value;
        }

        public static implicit operator MinMaxValueInt(int value)
        {
            return new MinMaxValueInt() { randomize = false, value = value };
        }
    }
}