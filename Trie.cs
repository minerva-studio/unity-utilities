using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minerva.Module
{
    public class Trie : ITrie, IEnumerable<string>, IEnumerable, ICollection<string>
    {
        class CharsComparer : IEqualityComparer<ReadOnlyMemory<char>>
        {
            public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
            {
                var xSpan = x.Span;
                var ySpan = y.Span;

                return xSpan.SequenceEqual(ySpan);
            }

            public int GetHashCode(ReadOnlyMemory<char> obj)
            {
                var hash = new HashCode();
                ReadOnlySpan<char> span = obj.Span;
                for (int i = 0; i < span.Length; i++)
                {
                    hash.Add(span[i]);
                }
                return hash.ToHashCode();
            }
        }

        internal ref struct StringKey
        {
            public string key;
            Span<int> indice;

            public StringKey(string key, Span<int> indice)
            {
                this.key = key;
                this.indice = indice;
            }

            public int Count => indice.Length;
            public ReadOnlyMemory<char> this[int index]
            {
                get
                {
                    int start = indice[index] + 1;
                    int end = index == Count - 1 ? key.Length : indice[index + 1];
                    return key.AsMemory(start, end - start);
                }
            }
        }

        internal ref struct KeyPointer
        {
            int mode;
            public IList<string> keys;
            public IList<ReadOnlyMemory<char>> keys2;
            public StringKey readOnlyMemories;

            private KeyPointer(int mode) : this() { this.mode = mode; }
            public KeyPointer(IList<string> keys) : this(1) { this.keys = keys; }
            public KeyPointer(IList<ReadOnlyMemory<char>> keys2) : this(2) { this.keys2 = keys2; }
            public KeyPointer(StringKey readOnlyMemories) : this(3) { this.readOnlyMemories = readOnlyMemories; }

            public readonly int Count => mode switch
            {
                1 => keys.Count,
                2 => keys2.Count,
                3 => readOnlyMemories.Count,
                _ => throw new NotSupportedException(),
            };

            public readonly ReadOnlyMemory<char> this[int index] => mode switch
            {
                1 => keys[index].AsMemory(),
                2 => keys2[index],
                3 => readOnlyMemories[index],
                _ => throw new NotSupportedException(),
            };
        }

        internal interface INode
        {
            public bool Clear();
        }

        internal class NodeBase<TNode> : INode where TNode : NodeBase<TNode>, INode
        {
            protected Dictionary<ReadOnlyMemory<char>, TNode> children;
            public int count;
            public bool isTerminated;

            public Dictionary<ReadOnlyMemory<char>, TNode> Children { get => children ??= new(new CharsComparer()); }
            public int LocalCount => children?.Count ?? 0;

            public bool Clear()
            {
                int count = this.count;
                Children.Clear();
                this.count = 0;
                return count > 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool ContainsKey(string s, char separator)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return ContainsKey(key);
            }

            internal bool ContainsKey(KeyPointer prefix) => TryGetNode(prefix, out var node) && node.isTerminated;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool ContainsPartialKey(string s, char separator)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return ContainsPartialKey(key);
            }

            internal bool ContainsPartialKey(KeyPointer prefix) => TryGetNode(prefix, out var node) && (node.count > 0 || node.isTerminated);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Remove(string s, char separator)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return Remove(key);
            }

            internal bool Remove(KeyPointer prefix)
            {
                var currentNode = this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out var childNode)) return false;
                    currentNode = childNode;
                }
                if (!currentNode.isTerminated) return false;

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

            /// <summary>
            /// Shrink the trie
            /// </summary>
            public void Shrink()
            {
                if (this.count == 0)
                {
                    Clear();
                    return;
                }
                foreach (var (key, childNode) in Children.ToArray())
                {
                    if (childNode.count == 0)
                    {
                        Children.Remove(key);
                    }
                    else childNode.Shrink();
                }
            }

            public IEnumerator<string[]> GetKeyEnumerator(Stack<string> stack)
            {
                if (isTerminated)
                {
                    yield return stack.ToArray();
                }
                if (count <= 1) yield break;
                foreach (var (key, node) in Children)
                {
                    stack.Push(key.ToString());
                    var enumerator = node.GetKeyEnumerator(stack);
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                    stack.Pop();
                }
            }

            public IEnumerator<string> GetKeyEnumerator(StringBuilder stringBuilder, char separator)
            {
                if (isTerminated)
                {
                    yield return stringBuilder.ToString();
                }
                if (count <= 1) yield break;
                if (stringBuilder.Length > 0) stringBuilder.Append(separator);
                foreach (var (key, node) in Children)
                {
                    int length = stringBuilder.Length;
                    stringBuilder.Append(key);
                    var enumerator = node.GetKeyEnumerator(stringBuilder, separator);
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                    stringBuilder.Length = length;
                }
                if (stringBuilder.Length > 0) stringBuilder.Length--;
            }

            public void TraverseCopyKey(Stack<string> stack, char separator, IList<string>[] arr, ref int idx)
            {
                if (isTerminated)
                {
                    arr[idx] = stack.ToArray();
                    idx++;
                    if (count <= 1) return;
                }
                foreach (var (key, node) in Children)
                {
                    if (node.count == 0) continue;
                    stack.Push(key.ToString());
                    node.TraverseCopyKey(stack, separator, arr, ref idx);
                    stack.Pop();
                }
            }

            public void TraverseCopyKey(StringBuilder stringBuilder, char separator, IList arr, ref int idx)
            {
                if (isTerminated)
                {
                    arr[idx] = stringBuilder.ToString();
                    idx++;
                    if (count <= 1) return;
                }
                if (stringBuilder.Length > 0) stringBuilder.Append(separator);
                foreach (var (key, node) in Children)
                {
                    if (node.count == 0) continue;
                    var baseLength = stringBuilder.Length;
                    stringBuilder.Append(key);
                    node.TraverseCopyKey(stringBuilder, separator, arr, ref idx);
                    stringBuilder.Length = baseLength;
                }
                if (stringBuilder.Length > 0) stringBuilder.Length--;
            }

            internal bool TryGetNode(string s, char separator, out TNode node)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return TryGetNode(key, out node);
            }

            internal bool TryGetNode(KeyPointer prefix, out TNode node)
            {
                TNode currentNode = (TNode)this;
                for (int i = 0; i < prefix.Count; i++)
                {
                    var key = prefix[i];
                    if (!currentNode.Children.TryGetValue(key, out currentNode))
                    {
                        node = null;
                        return false;
                    }
                }
                node = currentNode;
                return node != null;
            }
        }

        internal class Node : NodeBase<Node>, INode
        {
            public FirstLayerKeyCollection LocalKeys => new FirstLayerKeyCollection(this);

            internal bool Add(string s, char separator)
            {
                int count = GetSeperatorCount(s, separator);
                Span<int> indices = stackalloc int[count + 1];
                var key = GetStringKey(s, separator, indices);
                return Add(key);
            }

            internal bool Add(KeyPointer prefix)
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

            public Node Clone()
            {
                Node node = new()
                {
                    count = count,
                    isTerminated = isTerminated,
                };

                foreach (var item in node.Children)
                {
                    node.Children[item.Key] = item.Value.Clone();
                }

                return node;
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

            public KeyCollection(Trie trie) : this(trie.root, trie.separator) { }

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
            public readonly void CopyTo(List<string> copyTo)
            {
                copyTo.Clear();
                int index = 0;
                self.TraverseCopyKey(new StringBuilder(), separator, copyTo, ref index);
            }
            public readonly bool Contains(string item) => self.ContainsKey(item, separator);
            public readonly bool Contains(IList<string> item) => self.ContainsKey(new KeyPointer(item));

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


        readonly char separator = '.';
        Node root;

        public bool this[string key]
        {
            get { return Contains(key); }
            set { Set(key, value); }
        }

        public int Count => root.count;

        public FirstLayerKeyCollection FirstLayerKeys => root?.LocalKeys ?? FirstLayerKeyCollection.Empty;

        public KeyCollection Keys => new KeyCollection(this);

        public bool IsReadOnly => false;

        Node ITrie.Root => root;

        public char Separator => separator;

        public Trie()
        {
            root = new Node();
        }

        public Trie(char separator) : this()
        {
            this.separator = separator;
        }

        public Trie(IEnumerable<string> ts) : this()
        {
            foreach (var item in ts)
            {
                Add(item);
            }
        }

        public Trie(char separator, IEnumerable<string> ts) : this(separator)
        {
            foreach (var item in ts)
            {
                Add(item);
            }
        }

        private Trie(Node root, char separator) : this(separator)
        {
            this.root = root.Clone();
        }


        void ICollection<string>.Add(string item) => Add(item);

        public bool Add(string s) => root?.Add(s, separator) ?? false;

        public bool Add<T>(T prefix) where T : IList<string> => root?.Add(new KeyPointer(prefix)) ?? false;

        public void AddRange(IEnumerable<string> ts) { foreach (var item in ts) Add(item); }

        public bool Contains(string s) => root?.ContainsKey(s, separator) ?? false;

        public bool Contains<T>(T prefix) where T : IList<string> => root?.ContainsKey(new KeyPointer(prefix)) ?? false;

        public bool ContainsPartialKey(string s) => root?.ContainsPartialKey(s, separator) ?? false;

        public bool ContainsPartialKey<T>(T prefix) where T : IList<string> => root?.ContainsPartialKey(new KeyPointer(prefix)) ?? false;

        public bool Remove(string s) => root?.Remove(s, separator) ?? false;

        public bool Remove<T>(T prefix) where T : IList<string> => root?.Remove(new(prefix)) ?? false;

        public bool Set(string s, bool value) => value ? Add(s) : Remove(s);

        public bool Set<T>(T s, bool value) where T : IList<string> => value ? Add(s) : Remove(s);

        public void Clear() => root.Clear();

        public bool Clear(string key)
        {
            if (!TryGetNode(key, out var node)) return false;
            return node.Clear();
        }

        public bool Clear<T>(T prefix) where T : IList<string>
        {
            if (!TryGetNode(prefix, out var node)) return false;
            return node.Clear();
        }

        public void Shrink() => root?.Shrink();



        public Trie GetSubTrie(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Trie(root, separator);
            }
            return TryGetNode(s, out var currentNode)
                ? new Trie(currentNode, separator)
                : throw new ArgumentException();
        }

        public Trie GetSubTrie<T>(T prefix) where T : IList<string>
        {
            return TryGetNode(prefix, out var currentNode)
                ? new Trie(currentNode, separator)
                : throw new ArgumentException();
        }

        public bool TryGetSubTrie(string s, out Trie trie)
        {
            if (!TryGetNode(s, out Node currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Trie(currentNode, separator);
            return true;
        }

        public bool TryGetSubTrie<T>(T prefix, out Trie trie) where T : IList<string>
        {
            if (!TryGetNode(prefix, out Node currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Trie(currentNode, separator);
            return true;
        }




        bool ITrie.TryGetNode<T>(T prefix, out Node node) => TryGetNode(prefix, out node);
        bool ITrie.TryGetNode(string prefix, out Node node) => TryGetNode(prefix, out node);
        private bool TryGetNode<T>(T prefix, out Node node) where T : IList<string> => root?.TryGetNode(new KeyPointer(prefix), out node) ?? ((node = default) == null && false);
        private bool TryGetNode(string s, out Node node) => root?.TryGetNode(s, separator, out node) ?? ((node = default) == null && false);





        public IEnumerator<string> GetEnumerator()
        {
            string[] arr = new string[Count];
            CopyTo(arr, 0);
            foreach (var item in arr)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }





        public void CopyTo(string[] array, int index)
        {
            int arrayIndex = index;
            root?.TraverseCopyKey(new(), separator, array, ref arrayIndex);
        }

        public Trie Clone()
        {
            return new Trie(root.Clone(), separator);
        }

        public string[] ToArray()
        {
            string[] arry = new string[Count];
            CopyTo(arry, 0);
            return arry;
        }






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetSeperatorCount(string s, char separator)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == separator) count++;
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static KeyPointer GetStringKey(string s, char separator, Span<int> indices)
        {
            indices[0] = -1;
            int index = 1;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == separator) indices[index++] = i;
            }
            return new KeyPointer(new StringKey(s, indices));
        }
    }
}