using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static Minerva.Module.Trie;
using KeyPointer = Minerva.Module.Trie.KeyPointer;

namespace Minerva.Module
{
    public class Tries<TValue> : ITries<TValue>, IEnumerable<KeyValuePair<string, TValue>>, IEnumerable, IDictionary<string, TValue>, IDictionary<IList<string>, TValue>
    {
        internal class Node : NodeBase<Node>, INode
        {
            public TValue value;
            public FirstLayerKeyCollection LocalKeys => new FirstLayerKeyCollection(this);

            public Node() : base() { }

            public Node(TValue value) : this()
            {
                this.value = value;
                isTerminated = true;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Add(string s, char separator, TValue value)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return Add(key, value);
            }

            internal bool Add(KeyPointer prefix, TValue value)
            {
                Node currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    //move to the path
                    if (currentNode.Children.TryGetValue(key, out var childNode))
                    {
                        currentNode = childNode;
                    }
                    //current path does not exist, create nodes to create the path
                    else
                    {
                        childNode = new Node();
                        currentNode.Children.Add(key, childNode);
                        currentNode = childNode;
                    }
                }
                if (currentNode.isTerminated)
                {
                    return false;
                }
                currentNode.value = value;
                currentNode.isTerminated = true;
                currentNode = this;
                currentNode.count++;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    currentNode = currentNode.Children[key];
                    currentNode.count++;
                }

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Contains(string s, char separator, TValue value)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return Contains(key, value);
            }
            internal bool Contains<T>(KeyValuePair<T, TValue> item) where T : IList<string> => Contains(new(item.Key), item.Value);
            internal bool Contains<T>(T prefix, TValue item) where T : IList<string> => Contains(new(prefix), item);
            internal bool Contains(KeyPointer prefix, TValue item)
            {
                var currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode)) return false;
                    currentNode = childNode;
                }
                return currentNode.isTerminated && EqualityComparer<TValue>.Default.Equals(currentNode.value, item);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Remove(string s, char separator, TValue value)
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (!isTerminated || !EqualityComparer<TValue>.Default.Equals(this.value, value))
                        return false;

                    this.isTerminated = false;
                    this.count--;
                    return true;
                }

                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return Remove(key, value);
            }
            internal bool Remove<T>(KeyValuePair<T, TValue> item) where T : IList<string> => Remove(item.Key, item.Value);
            internal bool Remove<T>(T prefix, TValue item) where T : IList<string> => Remove(new KeyPointer(prefix), item);
            internal bool Remove(KeyPointer prefix, TValue item)
            {
                var currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode)) return false;
                    currentNode = childNode;
                }
                if (!currentNode.isTerminated || !EqualityComparer<TValue>.Default.Equals(currentNode.value, item))
                    return false;

                currentNode.isTerminated = false;
                currentNode = this;
                currentNode.count--;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    currentNode = currentNode.Children[key];
                    currentNode.count--;
                }
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Set(string s, char separator, TValue value)
            {
                if (string.IsNullOrEmpty(s))
                {
                    bool wasTerminated = isTerminated;
                    this.value = value;
                    this.isTerminated = true;

                    if (!wasTerminated)
                        this.count++;

                    return;
                }

                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                Set(key, value);
            }

            internal void Set(KeyPointer prefix, TValue value)
            {
                Node currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var child))
                    {
                        child = new Node();
                        currentNode.Children.Add(key, child);
                    }
                    currentNode = child;
                }

                bool wasTerminated = currentNode.isTerminated;
                currentNode.value = value;
                currentNode.isTerminated = true;

                if (!wasTerminated)
                {
                    currentNode = this;
                    currentNode.count++;
                    for (int i = 0; i < prefix.Count; i++)
                    {
                        var key = prefix[i];
                        currentNode = currentNode.Children[key];
                        currentNode.count++;
                    }
                }
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool TryGetValue(string s, char separator, out TValue value)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return TryGetSelfValue(out value);
                }
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return TryGetValue(key, out value);
            }

            private bool TryGetSelfValue(out TValue value)
            {
                if (isTerminated)
                {
                    value = this.value;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            internal bool TryGetValue(KeyPointer prefix, out TValue value)
            {
                if (prefix.Count == 0)
                {
                    return TryGetSelfValue(out value);
                }
                Node currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode))
                    {
                        value = default;
                        return false;
                    }
                    currentNode = childNode;
                }
                if (!currentNode.isTerminated)
                {
                    value = default;
                    return false;
                }
                value = currentNode.value;
                return true;
            }

            internal void GetValues(List<TValue> list, Node node)
            {
                foreach (var item in node.Children)
                {
                    Node child = item.Value;
                    if (child.isTerminated)
                    {
                        list.Add(child.value);
                    }
                    GetValues(list, child);
                }
            }





            public void TraverseCopy(StringBuilder stringBuilder, char separator, KeyValuePair<string, TValue>[] arr, ref int idx)
            {
                if (isTerminated)
                {
                    arr[idx] = new KeyValuePair<string, TValue>(stringBuilder.ToString(), value);
                    idx++;
                    if (count <= 1) return;
                }
                if (stringBuilder.Length > 0) stringBuilder.Append(separator);
                foreach (var (key, node) in Children)
                {
                    if (node.count == 0) continue;
                    var baseLength = stringBuilder.Length;
                    stringBuilder.Append(key.Span);
                    node.TraverseCopy(stringBuilder, separator, arr, ref idx);
                    stringBuilder.Length = baseLength;
                }
                if (stringBuilder.Length > 0) stringBuilder.Length--;
            }

            public void TraverseCopy(Stack<string> stack, char separator, KeyValuePair<IList<string>, TValue>[] arr, ref int idx)
            {
                if (isTerminated)
                {
                    arr[idx] = new(stack.ToArray(), value);
                    idx++;
                    if (count <= 1) return;
                }
                foreach (var (key, node) in Children)
                {
                    if (node.count == 0) continue;
                    stack.Push(new string(key.Span));
                    node.TraverseCopy(stack, separator, arr, ref idx);
                    stack.Pop();
                }
            }

            public void TraverseCopyValue(IList array, ref int arrayIndex)
            {
                if (isTerminated)
                {
                    array[arrayIndex] = value;
                    arrayIndex++;
                    if (count <= 1) return;
                }
                foreach (var (key, node) in Children)
                {
                    if (node.count == 0) continue;
                    node.TraverseCopyValue(array, ref arrayIndex);
                }
            }

            public IEnumerator<TValue> GetValueEnumerator()
            {
                foreach (var (key, node) in Children)
                {
                    if (node.isTerminated)
                    {
                        yield return node.value;
                    }

                    var enumerator = node.GetValueEnumerator();
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }

            public bool ContainsValue(TValue item)
            {
                if (isTerminated && EqualityComparer<TValue>.Default.Equals(value, item))
                {
                    return true;
                }
                foreach (var (key, node) in Children)
                {
                    if (node.ContainsValue(item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public Dictionary<string, TValue> ToDictionary(char keySeparator)
            {
                Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>();
                ToDictionary(new StringBuilder(), dictionary, keySeparator);
                return dictionary;
            }

            private void ToDictionary(StringBuilder stringBuilder, Dictionary<string, TValue> dictionary, char separator)
            {
                if (this.isTerminated)
                {
                    dictionary[stringBuilder.ToString()] = value;
                }
                if (stringBuilder.Length > 0) stringBuilder.Append(separator);
                foreach (var (key, node) in Children)
                {
                    var baseLength = stringBuilder.Length;
                    stringBuilder.Append(key.Span);
                    node.ToDictionary(stringBuilder, dictionary, separator);
                    stringBuilder.Length = baseLength;
                }
                if (stringBuilder.Length > 0) stringBuilder.Length--;
            }
        }

        public readonly struct FirstLayerKeyCollection : ICollection<ReadOnlyMemory<char>>, IEnumerable<ReadOnlyMemory<char>>, IReadOnlyCollection<ReadOnlyMemory<char>>, ICollection<string>, IEnumerable<string>, IReadOnlyCollection<string>, IEnumerable, ICollection
        {
            internal readonly Dictionary<ReadOnlyMemory<char>, Node>.KeyCollection collection;

            public static FirstLayerKeyCollection Empty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new FirstLayerKeyCollection(null); }

            public int Count => collection.Count;
            bool ICollection<string>.IsReadOnly => ((ICollection<ReadOnlyMemory<char>>)collection).IsReadOnly;
            bool ICollection<ReadOnlyMemory<char>>.IsReadOnly => ((ICollection<ReadOnlyMemory<char>>)collection).IsReadOnly;
            bool ICollection.IsSynchronized => ((ICollection)collection).IsSynchronized;
            object ICollection.SyncRoot => ((ICollection)collection).SyncRoot;


            internal FirstLayerKeyCollection(Node root) : this()
            {
                this.collection = root.Children.Keys;
            }

            void ICollection<string>.Add(string item) => throw new NotSupportedException();
            void ICollection<string>.Clear() => throw new NotSupportedException();
            bool ICollection<string>.Remove(string item) => throw new NotSupportedException();
            void ICollection<ReadOnlyMemory<char>>.Add(ReadOnlyMemory<char> item) => throw new NotSupportedException();
            void ICollection<ReadOnlyMemory<char>>.Clear() => throw new NotSupportedException();
            bool ICollection<ReadOnlyMemory<char>>.Remove(ReadOnlyMemory<char> item) => throw new NotSupportedException();


            public bool Contains(string item) => ((ICollection<ReadOnlyMemory<char>>)collection).Contains(item.AsMemory());
            public bool Contains(ReadOnlyMemory<char> item) => ((ICollection<ReadOnlyMemory<char>>)collection).Contains(item);
            public void CopyTo(ReadOnlyMemory<char>[] array) => collection.CopyTo(array, 0);
            public void CopyTo(ReadOnlyMemory<char>[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
            public void CopyTo(string[] array) => CopyTo(array, 0);
            public void CopyTo(string[] array, int arrayIndex) { foreach (var item in this) array[arrayIndex++] = item; }
            public List<string> ToList() => new List<string>(collection.Select(m => m.ToString()));
            public string[] ToArray() => ((IEnumerable<string>)this).ToArray();

            public IEnumerator<string> GetEnumerator()
            {
                foreach (var item in (IEnumerable<ReadOnlyMemory<char>>)this) yield return item.ToString();
            }
            IEnumerator<ReadOnlyMemory<char>> IEnumerable<ReadOnlyMemory<char>>.GetEnumerator() => ((IEnumerable<ReadOnlyMemory<char>>)collection).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)collection).GetEnumerator();
            void ICollection.CopyTo(Array array, int index) => ((ICollection)collection).CopyTo(array, index);
        }

        public readonly struct KeyCollection : ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>, ICollection,
            ICollection<IList<string>>, IEnumerable<IList<string>>, IReadOnlyCollection<IList<string>>
        {
            readonly Node self;
            readonly char separator;

            public KeyCollection(Tries<TValue> trie) : this(trie.root, trie.separator) { }

            internal KeyCollection(Node node, char separator) : this()
            {
                this.self = node;
                this.separator = separator;
            }

            public readonly int Count => self.count;
            public readonly bool IsReadOnly => true;
            public readonly bool IsSynchronized => false;
            public readonly object SyncRoot => this;
            public readonly void Add(string item) { throw new NotSupportedException(); }
            public readonly void Clear() { throw new NotSupportedException(); }
            public readonly bool Remove(string item) { throw new NotSupportedException(); }
            public readonly void Add(IList<string> item) { throw new NotSupportedException(); }
            public readonly bool Remove(IList<string> item) { throw new NotSupportedException(); }
            public readonly bool Contains(string item) => self.ContainsKey(item, separator);
            public readonly bool Contains(IList<string> item) => self.ContainsKey(new KeyPointer(item));
            public readonly void CopyTo(string[] array, int arrayIndex)
            {
                int index = arrayIndex;
                self.TraverseCopyKey(new StringBuilder(), separator, array, ref index);
            }
            public readonly void CopyTo(IList<string>[] array, int arrayIndex)
            {
                int index = arrayIndex;
                self.TraverseCopyKey(new Stack<string>(), separator, array, ref index);
            }
            public readonly void CopyTo(Array array, int index)
            {
                int refIndex = index;
                if (typeof(IList).IsAssignableFrom(array.GetType().GetElementType()))
                {
                    int i = 0;
                    var enumerator = self.GetKeyEnumerator(new Stack<string>());
                    while (enumerator.MoveNext())
                    {
                        array.SetValue(enumerator.Current, i++);
                    }
                    return;
                }
                self.TraverseCopyKey(new StringBuilder(), separator, array, ref refIndex);
            }


            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<string> GetEnumerator()
            {
                return self.GetKeyEnumerator(new StringBuilder(), separator);
            }
            IEnumerator<IList<string>> IEnumerable<IList<string>>.GetEnumerator()
            {
                return self.GetKeyEnumerator(new Stack<string>());
            }
        }

        public readonly struct ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyCollection<TValue>, ICollection
        {
            readonly Node self;

            public ValueCollection(Tries<TValue> trie) : this()
            {
                this.self = trie.root;
            }

            internal ValueCollection(Node node) : this()
            {
                this.self = node;
            }

            public readonly int Count => self.count;
            public readonly bool IsReadOnly => true;
            public readonly bool IsSynchronized => false;
            public readonly object SyncRoot => this;
            public readonly void Add(TValue item) { throw new NotSupportedException(); }
            public readonly void Clear() { throw new NotSupportedException(); }
            public readonly bool Remove(TValue item) { throw new NotSupportedException(); }
            public readonly bool Contains(TValue item) => self.ContainsValue(item);
            public readonly void CopyTo(TValue[] array, int arrayIndex)
            {
                int index = arrayIndex;
                self.TraverseCopyValue(array, ref index);
            }
            public readonly void CopyTo(Array array, int index)
            {
                self.TraverseCopyValue(array, ref index);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<TValue> GetEnumerator()
            {
                return self.GetValueEnumerator();
            }
        }


        readonly char separator = '.';
        Node root;

        public TValue this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public TValue this[IList<string> key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public int Count => root.count;

        public int FirstLayerCount => root.LocalCount;

        public FirstLayerKeyCollection FirstLayerKeys => root?.LocalKeys ?? FirstLayerKeyCollection.Empty;

        public KeyCollection Keys => new KeyCollection(this);

        public ValueCollection Values => new ValueCollection(this);

        public bool IsReadOnly => false;

        public char Separator => separator;

        internal Node Root => root;

        Node ITries<TValue>.Root => root;

        ICollection<string> IDictionary<string, TValue>.Keys => Keys;

        ICollection<IList<string>> IDictionary<IList<string>, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<string, TValue>.Values => Values;

        ICollection<TValue> IDictionary<IList<string>, TValue>.Values => Values;





        public Tries()
        {
            root = new Node();
        }

        public Tries(char separator) : this()
        {
            this.separator = separator;
        }

        public Tries(IEnumerable<string> ts) : this()
        {
            foreach (var item in ts)
            {
                Add(item, default);
            }
        }

        public Tries(char separator, IEnumerable<string> ts) : this(separator)
        {
            foreach (var item in ts)
            {
                Add(item, default);
            }
        }

        public Tries(Dictionary<string, TValue> dictionary) : this()
        {
            foreach (var item in dictionary)
            {
                Add(item);
            }
        }

        public Tries(TriesSegment<TValue> segment) : this(segment.Root.Clone(), segment.Separator) { }

        /// <summary>
        /// Will create a new tries with cloned nodes
        /// </summary>
        /// <param name="root"></param>
        /// <param name="separator"></param>
        private Tries(Node root, char separator) : this(separator)
        {
            this.root = root.Clone();
        }




        public bool Add(string s, TValue value) => root?.Add(s, separator, value) ?? false;

        public bool Add<T>(T prefix, TValue value) where T : IList<string> => root?.Add(new KeyPointer(prefix), value) ?? false;

        public void Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

        public void Add<T>(KeyValuePair<T, TValue> item) where T : IList<string> => Add(item.Key, item.Value);

        public bool ContainsKey(string s) => root?.ContainsKey(s, separator) ?? false;

        public bool ContainsKey<T>(T prefix) where T : IList<string> => root?.ContainsKey(new KeyPointer(prefix)) ?? false;

        public bool ContainsPartialKey(string s) => root?.ContainsPartialKey(s, separator) ?? false;

        public bool ContainsPartialKey<T>(T prefix) where T : IList<string> => root?.ContainsPartialKey(new KeyPointer(prefix)) ?? false;

        public bool Remove(string s) => root?.Remove(s, separator) ?? false;

        public bool Remove<T>(T prefix) where T : IList<string> => root?.Remove(new(prefix)) ?? false;

        public bool TryGetValue(string s, out TValue value) => root?.TryGetValue(s, separator, out value) ?? ((value = default) == null && false);

        public bool TryGetValue<T>(T prefix, out TValue value) where T : IList<string> => root?.TryGetValue(new(prefix), out value) ?? ((value = default) == null && false);

        public TValue Get(string s) => !TryGetNode(s, out var currentNode) || !currentNode.isTerminated ? throw new KeyNotFoundException() : currentNode.value;

        public TValue Get<T>(T prefix) where T : IList<string> => !TryGetNode(prefix, out var currentNode) || !currentNode.isTerminated ? throw new KeyNotFoundException() : currentNode.value;

        public void Set(string s, TValue value) => root?.Set(s, separator, value);

        public void Set<T>(T prefix, TValue value) where T : IList<string> => root?.Set(new(prefix), value);

        public void Clear() => Clear(false);

        public void Clear(bool keepStructure) => root?.Clear(keepStructure);

        public bool Clear(string key, bool keepStructure = false)
        {
            if (!TryGetNode(key, out var node)) return false;
            var changed = node.Clear(keepStructure);
            root?.Recount();
            return changed;
        }

        public bool Clear<T>(T prefix, bool keepStructure = false) where T : IList<string>
        {
            if (!TryGetNode(prefix, out var node)) return false;
            var changed = node.Clear(keepStructure);
            root?.Recount();
            return changed;
        }

        public bool Remove(KeyValuePair<string, TValue> item) => root?.Remove(item.Key, separator, item.Value) ?? false;

        public bool Remove<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Remove(item) ?? false;

        public bool Contains(KeyValuePair<string, TValue> item) => root?.Contains(item.Key, separator, item.Value) ?? false;

        public bool Contains<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Contains(item) ?? false;

        public void Shrink() => root?.Shrink();



        bool ITries<TValue>.TryGetNode<T>(T prefix, out Tries<TValue>.Node currentNode) => TryGetNode(prefix, out currentNode);
        bool ITries<TValue>.TryGetNode(string prefix, out Tries<TValue>.Node currentNode) => TryGetNode(prefix, out currentNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetNode<T>(T prefix, out Tries<TValue>.Node node) where T : IList<string> => root?.TryGetNode(new KeyPointer(prefix), out node) ?? ((node = default) == null && false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetNode(string prefix, out Tries<TValue>.Node node) => root?.TryGetNode(new(prefix), separator, out node) ?? ((node = default) == null && false);








        public Tries<TValue> GetSubTrie(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Tries<TValue>(root, separator);
            }
            return TryGetNode(s, out var currentNode)
                ? new Tries<TValue>(currentNode, separator)
                : throw new KeyNotFoundException();
        }

        public Tries<TValue> GetSubTrie<T>(T prefix) where T : IList<string>
        {
            return TryGetNode(prefix, out Node currentNode)
                ? new Tries<TValue>(currentNode, separator)
                : throw new ArgumentException();
        }

        public bool TryGetSubTrie(string s, out Tries<TValue> trie)
        {
            if (!TryGetNode(s, out var currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Tries<TValue>(currentNode, separator);
            return true;
        }

        public bool TryGetSubTrie<T>(T prefix, out Tries<TValue> trie) where T : IList<string>
        {
            if (!TryGetNode(prefix, out var currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Tries<TValue>(currentNode, separator);
            return true;
        }



        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            KeyValuePair<string, TValue>[] keyValuePairs = new KeyValuePair<string, TValue>[Count];
            CopyTo(keyValuePairs, 0);
            foreach (var item in keyValuePairs)
            {
                yield return item;
            }
        }

        IEnumerator<KeyValuePair<IList<string>, TValue>> IEnumerable<KeyValuePair<IList<string>, TValue>>.GetEnumerator()
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

        void IDictionary<string, TValue>.Add(string key, TValue value) => Add(key, value);





        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            root?.TraverseCopy(new(), separator, array, ref index);
        }

        public void CopyTo(KeyValuePair<IList<string>, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            root?.TraverseCopy(new(), separator, array, ref index);
        }





        #region Dictionary


        public void Add(IList<string> prefix, TValue value) => Add<IList<string>>(prefix, value);
        public void Add(KeyValuePair<IList<string>, TValue> value) => Add<IList<string>>(value);
        public bool Remove(IList<string> prefix) => Remove<IList<string>>(prefix);
        public bool ContainsKey(IList<string> prefix) => ContainsKey<IList<string>>(prefix);
        public bool Contains(KeyValuePair<IList<string>, TValue> value) => Contains<IList<string>>(value);
        public bool Remove(KeyValuePair<IList<string>, TValue> value) => Remove<IList<string>>(value);
        public bool TryGetValue(IList<string> prefix, out TValue value) => TryGetValue<IList<string>>(prefix, out value);

        #endregion


        public Tries<TValue> Clone()
        {
            return new Tries<TValue>(root, separator);
        }

        public Dictionary<string, TValue> ToDictionary() => root.ToDictionary(separator);
    }
}