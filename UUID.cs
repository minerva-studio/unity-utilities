#nullable enable
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A serializable unique identifer, compatible with <see cref="Guid"/>
    /// </summary>
    [Serializable]
    public struct UUID : IComparable, IComparable<UUID>, IComparable<Guid>, IEquatable<UUID>, IEquatable<Guid>, ISerializationCallbackReceiver
    {
        public static UUID Empty = Guid.Empty;

        [SerializeField]
        [ContextMenuItem("New UUID", "NewUUID")]
        private string? value;
        private Guid guid;

        public string Value => string.IsNullOrEmpty(value) ? value = Empty.Value : value;
        public readonly Guid Numeric => guid;


        public UUID(string value)
        {
            this.value = value;
            try
            {
                this.guid = string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
            }
            catch (Exception)
            {
                this.guid = Guid.Empty;
                this.value = Empty.Value;
            }
        }

        public UUID(Guid value)
        {
            this.value = value.ToString();
            this.guid = value;
        }

        public readonly int CompareTo(object value)
        {
            return value switch
            {
                Guid g => g.CompareTo(Numeric),
                UUID guid => Numeric.CompareTo(guid.Numeric),
                _ => throw new ArgumentException("Must be Guid"),
            };
        }

        public readonly int CompareTo(UUID other) => Numeric.CompareTo(other.Numeric);
        public readonly int CompareTo(Guid other) => Numeric.CompareTo(other);
        public readonly bool Equals(UUID other) => Numeric == other.Numeric;
        public readonly bool Equals(Guid other) => Numeric == other;
        public readonly override bool Equals(object obj) => (obj is UUID other && Equals(other)) || (obj is Guid guid && Equals(guid));
        public readonly override int GetHashCode() => Numeric.GetHashCode();
        public override string ToString() => Value;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UUID NewUUID() => Guid.NewGuid();



        public static bool operator ==(UUID u1, UUID u2) => u1.Equals(u2);

        public static bool operator !=(UUID u1, UUID u2) => !(u1 == u2);

        public static implicit operator UUID(Guid guid) => new UUID(guid);

        public static implicit operator Guid(UUID uuid) => string.IsNullOrEmpty(uuid.Value) || uuid.Value == null ? Guid.Empty : new Guid(uuid.Value);

        public static implicit operator string(UUID u) => u.Value;

        public static implicit operator BigInteger(UUID u) => ParseToNumeric(u.value);

        public static BigInteger ParseToNumeric(UUID u) => ParseToNumeric(u.value);

        public static BigInteger ParseToNumeric(string? str)
        {
            if (string.IsNullOrEmpty(str)) return 0;

            BigInteger value = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char current = str[i];
                switch (current)
                {
                    case >= '0' and <= '9':
                        value <<= 4;
                        value += current - '0';
                        break;
                    case >= 'a' and <= 'f':
                        value <<= 4;
                        value += current - 'a' + 10;
                        break;
                    default:
                        break;
                }
            }
            return value;
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => Guid.TryParse(value, out guid);
    }
}