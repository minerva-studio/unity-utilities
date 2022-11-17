using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    [Serializable]
    public class StringPair : IEquatable<StringPair>, IComparable<StringPair>
    {
        [SerializeField] private string key;
        [SerializeField] private string value;
        public StringPair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Value { get => value; set => this.value = value; }
        public string Key { get => key; set => key = value; }


        public bool Equals(StringPair other)
        {
            return other.value == value && other.key == key;
        }

        public override bool Equals(object obj)
        {
            return obj is StringPair ? Equals((StringPair)obj) : false;
        }

        public int CompareTo(StringPair other)
        {
            return key.CompareTo(other.key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public static implicit operator KeyValuePair<string, string>(StringPair arg)
        {
            return new KeyValuePair<string, string>(arg.key, arg.value);
        }

        public static implicit operator StringPair(KeyValuePair<string, string> arg)
        {
            return new StringPair(arg.Key, arg.Value);
        }
    }

    public static class StringPairExtensions
    {
        public static bool ContainsKey(this IEnumerable<StringPair> nameables, string key)
        {
            foreach (var nameable in nameables)
            {
                if (nameable.Key == key) return true;
            }
            return false;
        }

        public static string GetValue(this IEnumerable<StringPair> nameables, string key)
        {
            foreach (var nameable in nameables)
            {
                if (nameable.Key == key) return nameable.Value;
            }
            return string.Empty;
        }

        public static bool SetValue(this IEnumerable<StringPair> nameables, string key, string value)
        {
            foreach (var nameable in nameables)
            {
                if (nameable.Key == key)
                {
                    nameable.Value = value;
                }
                return true;
            }
            return false;
        }
    }
}