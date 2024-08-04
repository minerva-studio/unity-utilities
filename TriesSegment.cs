using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static Minerva.Module.Trie;

namespace Minerva.Module
{
    public interface ITries<TValue> : ITrieBase, IEnumerable<KeyValuePair<string, TValue>>, IDictionary<string, TValue>, IDictionary<IList<string>, TValue>
    {
        TValue Get(string s);
        Tries<TValue>.FirstLayerKeyCollection FirstLayerKeys { get; }

        internal Tries<TValue>.Node Root { get; }
        new Tries<TValue>.KeyCollection Keys { get; }
        new Tries<TValue>.ValueCollection Values { get; }

        internal bool TryGetNode<T>(T prefix, out Tries<TValue>.Node currentNode) where T : IList<string>;
        internal bool TryGetNode(string prefix, out Tries<TValue>.Node currentNode);
    }

    public struct TriesSegment<TValue> : ITries<TValue>
    {
        private Tries<TValue>.Node root;
        private char separator;


        internal TriesSegment(Tries<TValue>.Node root, char separator) : this()
        {
            this.root = root;
            this.separator = separator;
        }

        public TriesSegment(Tries<TValue> tries) : this()
        {
            this.root = tries.Root;
            this.separator = tries.Separator;
        }


        public readonly TValue this[string key]
        {
            get { return Get(key); }

            set { Set(key, value); }
        }

        public readonly TValue this[IList<string> key]
        {
            get { return Get(key); }

            set { Set(key, value); }
        }

        public readonly int Count => root?.count ?? 0;

        public readonly int FirstLayerCount => root?.LocalCount ?? 0;

        public readonly Tries<TValue>.FirstLayerKeyCollection FirstLayerKeys => root?.LocalKeys ?? Tries<TValue>.FirstLayerKeyCollection.Empty;

        public readonly Tries<TValue>.KeyCollection Keys => new(root, separator);

        public readonly Tries<TValue>.ValueCollection Values => new(root);

        public readonly bool IsReadOnly => false;

        public readonly char Separator => separator;

        internal readonly Tries<TValue>.Node Root => root;

        readonly Tries<TValue>.Node ITries<TValue>.Root => root;


        readonly ICollection<string> IDictionary<string, TValue>.Keys => Keys;

        readonly ICollection<IList<string>> IDictionary<IList<string>, TValue>.Keys => Keys;

        readonly ICollection<TValue> IDictionary<string, TValue>.Values => Values;

        readonly ICollection<TValue> IDictionary<IList<string>, TValue>.Values => Values;






        public readonly bool Add(string s, TValue value) => root?.Add(s, separator, value) ?? false;

        public readonly bool Add<T>(T prefix, TValue value) where T : IList<string> => root?.Add(new KeyPointer(prefix), value) ?? false;

        public readonly void Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

        public readonly void Add<T>(KeyValuePair<T, TValue> item) where T : IList<string> => Add(item.Key, item.Value);

        public readonly bool ContainsKey(string s) => root?.ContainsKey(s, separator) ?? false;

        public readonly bool ContainsKey<T>(T prefix) where T : IList<string> => root?.ContainsKey(new KeyPointer(prefix)) ?? false;

        public readonly bool ContainsPartialKey(string s) => root?.ContainsPartialKey(s, separator) ?? false;

        public readonly bool ContainsPartialKey<T>(T prefix) where T : IList<string> => root?.ContainsPartialKey(new KeyPointer(prefix)) ?? false;

        public readonly bool Remove(string s) => root?.Remove(s, separator) ?? false;

        public readonly bool Remove<T>(T prefix) where T : IList<string> => root?.Remove(new(prefix)) ?? false;

        public readonly bool TryGetValue(string s, out TValue value) => root?.TryGetValue(s, separator, out value) ?? ((value = default) == null && false);

        public readonly bool TryGetValue<T>(T prefix, out TValue value) where T : IList<string> => root?.TryGetValue(new(prefix), out value) ?? ((value = default) == null && false);

        public readonly TValue Get(string s) => !TryGetNode(s, out var currentNode) || !currentNode.isTerminated ? throw new KeyNotFoundException() : currentNode.value;

        public readonly TValue Get<T>(T prefix) where T : IList<string> => !TryGetNode(prefix, out var currentNode) || !currentNode.isTerminated ? throw new KeyNotFoundException() : currentNode.value;

        public readonly void Set(string s, TValue value) => root?.Set(s, separator, value);

        public readonly void Set<T>(T prefix, TValue value) where T : IList<string> => root?.Set(new(prefix), value);

        public readonly void Clear() => root?.Clear();

        public readonly bool Clear(string key)
        {
            if (!TryGetNode(key, out var node)) return false;
            return node.Clear();
        }

        public readonly bool Clear<T>(T key) where T : IList<string>
        {
            if (!TryGetNode(key, out var node)) return false;
            return node.Clear();
        }

        public readonly bool Remove(KeyValuePair<string, TValue> item) => root?.Remove(item.Key, separator, item.Value) ?? false;

        public readonly bool Remove<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Remove(item) ?? false;

        public readonly bool Contains(KeyValuePair<string, TValue> item) => root?.Contains(item.Key, separator, item.Value) ?? false;

        public readonly bool Contains<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Contains(item) ?? false;

        public readonly void Shrink() => root?.Shrink();



        readonly bool ITries<TValue>.TryGetNode<T>(T prefix, out Tries<TValue>.Node currentNode) => TryGetNode(prefix, out currentNode);
        readonly bool ITries<TValue>.TryGetNode(string prefix, out Tries<TValue>.Node currentNode) => TryGetNode(prefix, out currentNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool TryGetNode<T>(T prefix, out Tries<TValue>.Node node) where T : IList<string> => root?.TryGetNode(new KeyPointer(prefix), out node) ?? ((node = default) == null && false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool TryGetNode(string prefix, out Tries<TValue>.Node node) => root?.TryGetNode(new(prefix), separator, out node) ?? ((node = default) == null && false);









        public readonly IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            KeyValuePair<string, TValue>[] keyValuePairs = new KeyValuePair<string, TValue>[Count];
            CopyTo(keyValuePairs, 0);
            foreach (var item in keyValuePairs)
            {
                yield return item;
            }
        }

        readonly IEnumerator<KeyValuePair<IList<string>, TValue>> IEnumerable<KeyValuePair<IList<string>, TValue>>.GetEnumerator()
        {
            KeyValuePair<IList<string>, TValue>[] keyValuePairs = new KeyValuePair<IList<string>, TValue>[Count];
            CopyTo(keyValuePairs, 0);
            foreach (var item in keyValuePairs)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        readonly void IDictionary<string, TValue>.Add(string key, TValue value) => Add(key, value);





        public readonly void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            root?.TraverseCopy(new(), separator, array, ref index);
        }

        public readonly void CopyTo(KeyValuePair<IList<string>, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            root?.TraverseCopy(new(), separator, array, ref index);
        }





        #region Dictionary


        public readonly void Add(IList<string> prefix, TValue value) => Add<IList<string>>(prefix, value);
        public readonly void Add(KeyValuePair<IList<string>, TValue> value) => Add<IList<string>>(value);
        public readonly bool Remove(IList<string> prefix) => Remove<IList<string>>(prefix);
        public readonly bool ContainsKey(IList<string> prefix) => ContainsKey<IList<string>>(prefix);
        public readonly bool Contains(KeyValuePair<IList<string>, TValue> value) => Contains<IList<string>>(value);
        public readonly bool Remove(KeyValuePair<IList<string>, TValue> value) => Remove<IList<string>>(value);
        public readonly bool TryGetValue(IList<string> prefix, out TValue value) => TryGetValue<IList<string>>(prefix, out value);

        #endregion





        public readonly Tries<TValue> ToTries() => new Tries<TValue>(this);

        public readonly Dictionary<string, TValue> ToDictionary() => root.ToDictionary(separator);
    }
}