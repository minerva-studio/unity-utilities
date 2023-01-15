using System;
using UnityEngine;

namespace Minerva.Module
{
    [Serializable]
    public struct MinMaxValueInt
    {
        [SerializeField] private bool randomize;
        [SerializeField, DisplayIf(nameof(randomize), false)] private int count;
        [SerializeField, DisplayIf(nameof(randomize))] private RangeInt range;
        public int Value => randomize ? range.value : count;

        public static implicit operator int(MinMaxValueInt minMaxValue)
        {
            return minMaxValue.Value;
        }
    }
}