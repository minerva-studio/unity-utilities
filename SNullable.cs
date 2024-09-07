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


        public bool HasValue => hasValue;
        public T Value => hasValue ? value : throw new InvalidOperationException();



        public static explicit operator T(SNullable<T> nullable) => nullable.Value;

        public static implicit operator SNullable<T>(T value) => new SNullable<T>() { hasValue = true, value = value };

        public static implicit operator T?(SNullable<T> nullable) => nullable.hasValue ? nullable.value : null;

        public static implicit operator SNullable<T>(Nullable<T> nullable) => new SNullable<T>() { hasValue = nullable.HasValue, value = nullable ?? default };

        public override bool Equals(object other)
        {
            if (other is SNullable<T> sn) return sn.HasValue == HasValue && (!HasValue || Value.Equals(sn.Value));
            return other is T n && HasValue && Value.Equals(n);
        }

        public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;
        public T GetValueOrDefault() => hasValue ? value : default;
        public T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue;
        public override string ToString() => HasValue ? "null" : Value.ToString();
    }
}