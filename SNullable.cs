#nullable enable
using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Serializable <see cref="Nullable{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct SNullable<T> where T : struct
    {
        [SerializeField]
        bool hasValue;
        [SerializeField]
        T value;


        public readonly bool HasValue => hasValue;
        public readonly T Value => hasValue ? value : throw new InvalidOperationException();
        public readonly T? Nullable => this;



        public static explicit operator T(SNullable<T> nullable) => nullable.Value;

        public static implicit operator SNullable<T>(T value) => new() { hasValue = true, value = value };

        public static implicit operator T?(SNullable<T> nullable) => nullable.hasValue ? nullable.value : null;

        public static implicit operator SNullable<T>(T? nullable) => new() { hasValue = nullable.HasValue, value = nullable ?? default };

        public override readonly bool Equals(object other)
        {
            if (other is SNullable<T> sn) return sn.HasValue == HasValue && (!HasValue || Value.Equals(sn.Value));
            return other is T n && HasValue && Value.Equals(n);
        }

        public override readonly int GetHashCode() => HasValue ? Value.GetHashCode() : 0;
        public readonly T GetValueOrDefault() => hasValue ? value : default;
        public readonly T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue;
        public readonly override string ToString() => HasValue ? "null" : Value.ToString();
    }
}