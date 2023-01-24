using System;
using UnityEngine.Serialization;

namespace Minerva.Module
{
    [Serializable]
    public struct MinMaxValueInt
    {
        public bool randomize;
        [FormerlySerializedAs("count")]
        [DisplayIf(nameof(randomize), false)] public int value;
        [DisplayIf(nameof(randomize))] public RangeInt range;
        public int Value => randomize ? range.value : value;

        public static implicit operator int(MinMaxValueInt minMaxValue)
        {
            return minMaxValue.Value;
        }
    }
}