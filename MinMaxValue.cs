using System;
using UnityEngine;

namespace Minerva.Module
{
    [Serializable]
    public struct MinMaxValue
    {
        [SerializeField] public bool randomize;
        [SerializeField, DisplayIf(nameof(randomize), false)] public float value;
        [SerializeField, DisplayIf(nameof(randomize))] public Range range;

        public float Value => randomize ? range.value : value;

        public static implicit operator float(MinMaxValue minMaxValue)
        {
            return minMaxValue.Value;
        }
    }
}