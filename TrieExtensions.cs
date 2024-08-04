using System;
using System.Collections.Generic;

namespace Minerva.Module
{
    public interface ITrieBase
    {
        char Separator { get; }

        bool ContainsPartialKey(string s);
        bool ContainsPartialKey<T>(T prefix) where T : IList<string>;
    }

    public static class TrieExtensions
    {
        public static TrieSegment GetSegment<T>(this T trie, string s) where T : ITrie
        {
            if (string.IsNullOrEmpty(s))
            {
                return new TrieSegment(trie.Root, trie.Separator);
            }
            return trie.TryGetNode(s, out var currentNode)
                ? new TrieSegment(currentNode, trie.Separator)
                : throw new ArgumentException();
        }

        public static TrieSegment GetSegment<TTrie, TPrefix>(this TTrie trie, TPrefix prefix) where TPrefix : IList<string> where TTrie : ITrie
        {
            return trie.TryGetNode(prefix, out var currentNode)
                ? new TrieSegment(currentNode, trie.Separator)
                : throw new ArgumentException();
        }

        public static bool TryGetSegment<T>(this T self, string s, out TrieSegment trie) where T : ITrie
        {
            if (!self.TryGetNode(s, out Trie.Node currentNode))
            {
                trie = default;
                return false;
            }
            trie = new TrieSegment(currentNode, self.Separator);
            return true;
        }

        public static bool TryGetSegment<T, TPrefix>(this T self, TPrefix prefix, out TrieSegment trie) where TPrefix : IList<string> where T : ITrie
        {
            if (!self.TryGetNode(prefix, out Trie.Node currentNode))
            {
                trie = default;
                return false;
            }
            trie = new TrieSegment(currentNode, self.Separator);
            return true;
        }





        public static TriesSegment<TValue> GetSegment<TValue>(this Tries<TValue> trie, string s) => GetSegment<Tries<TValue>, TValue>(trie, s);

        public static TriesSegment<TValue> GetSegment<TValue>(this TriesSegment<TValue> trie, string s) => GetSegment<TriesSegment<TValue>, TValue>(trie, s);

        public static TriesSegment<TValue> GetSegment<T, TValue>(this T trie, string s) where T : ITries<TValue>
        {
            if (string.IsNullOrEmpty(s)) return new TriesSegment<TValue>(trie.Root, trie.Separator);
            return trie.TryGetNode(s, out var currentNode)
                ? new TriesSegment<TValue>(currentNode, trie.Separator)
                : throw new ArgumentException();
        }

        public static TriesSegment<TValue> GetSegment<TValue, TPrefix>(this Tries<TValue> trie, TPrefix s) where TPrefix : IList<string> => GetSegment<Tries<TValue>, TValue, TPrefix>(trie, s);

        public static TriesSegment<TValue> GetSegment<TValue, TPrefix>(this TriesSegment<TValue> trie, TPrefix s) where TPrefix : IList<string> => GetSegment<TriesSegment<TValue>, TValue, TPrefix>(trie, s);

        public static TriesSegment<TValue> GetSegment<T, TValue, TPrefix>(this T trie, TPrefix prefix) where TPrefix : IList<string> where T : ITries<TValue>
        {
            return trie.TryGetNode(prefix, out var currentNode)
                ? new TriesSegment<TValue>(currentNode, trie.Separator)
                : throw new ArgumentException();
        }

        public static bool TryGetSegment<TValue>(this Tries<TValue> self, string s, out TriesSegment<TValue> trie) => TryGetSegment<Tries<TValue>, TValue>(self, s, out trie);

        public static bool TryGetSegment<TValue>(this in TriesSegment<TValue> self, string s, out TriesSegment<TValue> trie) => TryGetSegment<TriesSegment<TValue>, TValue>(self, s, out trie);

        public static bool TryGetSegment<T, TValue>(this T self, string s, out TriesSegment<TValue> trie) where T : ITries<TValue>
        {
            if (!self.TryGetNode(s, out Tries<TValue>.Node currentNode))
            {
                trie = default;
                return false;
            }
            trie = new TriesSegment<TValue>(currentNode, self.Separator);
            return true;
        }

        public static bool TryGetSegment<TValue, TPrefix>(this Tries<TValue> self, TPrefix prefix, out TriesSegment<TValue> trie) where TPrefix : IList<string> => TryGetSegment<Tries<TValue>, TValue, TPrefix>(self, prefix, out trie);

        public static bool TryGetSegment<TValue, TPrefix>(this in TriesSegment<TValue> self, TPrefix prefix, out TriesSegment<TValue> trie) where TPrefix : IList<string> => TryGetSegment<TriesSegment<TValue>, TValue, TPrefix>(self, prefix, out trie);

        public static bool TryGetSegment<T, TValue, TPrefix>(this T self, TPrefix prefix, out TriesSegment<TValue> trie) where TPrefix : IList<string> where T : ITries<TValue>
        {
            if (!self.TryGetNode(prefix, out Tries<TValue>.Node currentNode))
            {
                trie = default;
                return false;
            }
            trie = new TriesSegment<TValue>(currentNode, self.Separator);
            return true;
        }
    }
}