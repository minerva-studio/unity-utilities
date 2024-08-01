using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Minerva.Module
{
    public class Tries<TValue> : ITries<TValue>, IEnumerable<KeyValuePair<string, TValue>>, IEnumerable, IDictionary<string, TValue>, IDictionary<IList<string>, TValue>
    {
        internal class Node : Trie.NodeBase<Node>, Trie.INode
        {
            public TValue value;
            public FirstLayerKeyCollection LocalKeys => new FirstLayerKeyCollection(this);

            public Node() : base() { }

            public Node(TValue value) : this()
            {
                this.value = value;
                isTerminated = true;
            }

            public Node Clone()
            {
                Node node = new()
                {
                    value = value,
                    count = count,
                    isTerminated = isTerminated,
                };

                foreach (var item in node.Children)
                {
                    node.Children[item.Key] = item.Value.Clone();
                }

                return node;
            }




            public bool Add<T>(T prefix, TValue value) where T : IList<string>
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

            public bool Contains<T>(KeyValuePair<T, TValue> item) where T : IList<string>
            {
                var currentNode = this;
                T prefix = item.Key;
                for (int i = 0; i < prefix.Count; i++)
                {
                    string key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode)) return false;
                    currentNode = childNode;
                }
                return currentNode.isTerminated && EqualityComparer<TValue>.Default.Equals(currentNode.value, item.Value);
            }

            public bool Remove<T>(KeyValuePair<T, TValue> item) where T : IList<string>
            {
                var prefix = item.Key;
                var currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    string key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode)) return false;
                    currentNode = childNode;
                }
                if (!currentNode.isTerminated || Equals(currentNode.value, item.Value)) return false;

                currentNode.isTerminated = false;
                currentNode = this;
                currentNode.count--;
                for (int i = 0; i < prefix.Count; i++)
                {
                    string key = prefix[i];
                    currentNode = currentNode.Children[key];
                    currentNode.count--;
                }
                return true;
            }

            public void Set<T>(T prefix, TValue value) where T : IList<string>
            {
                Node currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    currentNode.count++;
                    //move to the path
                    if (currentNode.Children.TryGetValue(key, out var child))
                    {
                        currentNode = child;
                    }
                    //current path does not exist, create nodes to create the path
                    else
                    {
                        child = new Node();
                        currentNode.Children.Add(key, child);
                        currentNode = child;
                    }
                }
                currentNode.value = value;
                currentNode.isTerminated = true;
            }

            public bool TryGetValue<T>(T prefix, out TValue value) where T : IList<string>
            {
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

            public void GetValues(List<TValue> list, Node node)
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
                    stringBuilder.Append(key);
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
                    stack.Push(key);
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
                    stringBuilder.Append(key);
                    ToDictionary(stringBuilder, dictionary, separator);
                    stringBuilder.Length = baseLength;
                }
                if (stringBuilder.Length > 0) stringBuilder.Length--;
            }
        }

        public readonly struct FirstLayerKeyCollection : ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>, ICollection
        {
            internal readonly Dictionary<string, Node>.KeyCollection collection;

            public static FirstLayerKeyCollection Empty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new FirstLayerKeyCollection(null); }

            public int Count => collection.Count;
            bool ICollection<string>.IsReadOnly => ((ICollection<string>)collection).IsReadOnly;
            bool ICollection.IsSynchronized => ((ICollection)collection).IsSynchronized;
            object ICollection.SyncRoot => ((ICollection)collection).SyncRoot;


            internal FirstLayerKeyCollection(Node root) : this()
            {
                this.collection = root.Children.Keys;
            }

            void ICollection<string>.Add(string item) { throw new NotSupportedException(); }
            void ICollection<string>.Clear() { throw new NotSupportedException(); }
            bool ICollection<string>.Remove(string item) { throw new NotSupportedException(); }
            public bool Contains(string item) => ((ICollection<string>)collection).Contains(item);
            public void CopyTo(string[] array) => collection.CopyTo(array, 0);
            public void CopyTo(List<string> list) => list.AddRange(collection);
            public void CopyTo(string[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
            public List<string> ToList() => new List<string>(collection);

            public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)collection).GetEnumerator();
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
            public readonly bool Contains(string item) => self.ContainsKey(Split(item, separator));
            public readonly bool Contains(IList<string> item) => self.ContainsKey(item);
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




        public bool Add(string s, TValue value) => Add(Split(s), value);

        public bool Add<T>(T prefix, TValue value) where T : IList<string> => root?.Add(prefix, value) ?? false;

        public void Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

        public void Add<T>(KeyValuePair<T, TValue> item) where T : IList<string> => Add(item.Key, item.Value);

        public bool ContainsKey(string s) => ContainsKey(Split(s));

        public bool ContainsKey<T>(T prefix) where T : IList<string> => root?.ContainsKey(prefix) ?? false;

        public bool ContainsPartialKey(string s) => ContainsPartialKey(Split(s));

        public bool ContainsPartialKey<T>(T prefix) where T : IList<string> => root?.ContainsPartialKey(prefix) ?? false;

        public bool Remove(string s) => Remove(Split(s));

        public bool Remove<T>(T prefix) where T : IList<string> => root?.Remove(prefix) ?? false;

        public bool TryGetValue(string s, out TValue value) => TryGetValue(Split(s), out value);

        public bool TryGetValue<T>(T prefix, out TValue value) where T : IList<string> => root?.TryGetValue(prefix, out value) ?? ((value = default) == null && false);

        public TValue Get(string s) => Get(Split(s));

        public TValue Get<T>(T prefix) where T : IList<string> => !TryGetNode(prefix, out var currentNode) || !currentNode.isTerminated ? throw new KeyNotFoundException() : currentNode.value;

        public void Set(string s, TValue value) => Set(Split(s), value);

        public void Set<T>(T prefix, TValue value) where T : IList<string> => root?.Set(prefix, value);

        bool ITries<TValue>.TryGetNode<T>(T prefix, out Node currentNode) => TryGetNode(prefix, out currentNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetNode<T>(T prefix, out Node node) where T : IList<string> => root?.TryGetNode(prefix, out node) ?? ((node = default) == null && false);

        public void Clear() => root?.Clear();

        public bool Clear(string key) => Clear(Split(key));

        public bool Clear<T>(T key) where T : IList<string>
        {
            if (!TryGetNode(key, out var node)) return false;
            return node.Clear();
        }

        public void Shrink() => root?.Shrink();




        public bool Remove(KeyValuePair<string, TValue> item) => Remove(new KeyValuePair<string[], TValue>(Split(item.Key), item.Value));

        public bool Remove<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Remove(item) ?? false;

        public bool Contains(KeyValuePair<string, TValue> item) => Contains(new KeyValuePair<string[], TValue>(Split(item.Key), item.Value));

        public bool Contains<T>(KeyValuePair<T, TValue> item) where T : IList<string> => root?.Contains(item) ?? false;







        public Tries<TValue> GetSubTrie(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Tries<TValue>(root, separator);
            }
            string[] prefix = Split(s);
            return TryGetNode(prefix, out var currentNode)
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
            string[] prefix = Split(s);
            return TryGetSubTrie(prefix, out trie);
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



        private string[] Split(string s)
        {
            char separator = this.separator;
            return Split(s, separator);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] Split(string s, char separator) => Trie.Split(s, separator);
    }
}