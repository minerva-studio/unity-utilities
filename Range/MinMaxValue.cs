using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// a struct representing a float value based on given value in unity, could be constant or random
    /// </summary>
    [Serializable]
    public struct MinMaxValue
    {
        [SerializeField] public bool randomize;
        [SerializeField, DisplayIf(nameof(randomize), false)] public float value;
        [SerializeField, DisplayIf(nameof(randomize))] public Range range;

        public readonly float Value => randomize ? range.min : value;
        public readonly float Min => randomize ? range.min : value;
        public readonly float Max => randomize ? range.max : value;



        public readonly float Lerp(float t)
        {
            return randomize ? range.Lerp(t) : value;
        }



        public static implicit operator MinMaxValue(float value)
        {
            return new MinMaxValue { randomize = false, value = value };
        }
    }
}