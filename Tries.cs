using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

namespace Minerva.Module
{
    public class Tries<TValue> : ITries<TValue>, IEnumerable<KeyValuePair<string, TValue>>, IEnumerable, IDictionary<string, TValue>
    {
        private class Node
        {
            public Dictionary<string, Node> children;
            public TValue value;
            public int count;
            public bool isTerminated;

            public Node()
            {
                children = new Dictionary<string, Node>();
            }

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
                    children = new()
                };

                foreach (var item in node.children)
                {
                    node.children[item.Key] = item.Value.Clone();
                }

                return node;
            }

            public void TraverseCopy(KeyValuePair<string, TValue>[] arr, ref int idx, char separator, Stack<string> keyChain)
            {
                if (isTerminated)
                {
                    arr[idx] = KeyValuePair.Create(string.Join(separator, keyChain), value);
                    idx++;
                }
                foreach (var (key, val) in children)
                {
                    keyChain.Push(key);
                    val.TraverseCopy(arr, ref idx, separator, keyChain);
                    keyChain.Pop();
                }
            }
        }


        readonly char separator = '.';
        Node root;

        public TValue this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public int Count => root.count;

        public List<string> Keys
        {
            get
            {
                List<string> array = new();
                GetKeys(array, root, "");
                return array;
            }
        }

        public List<TValue> Values
        {
            get
            {
                List<TValue> array = new List<TValue>(Count * 2);
                GetValues(array, root);
                return array;
            }
        }

        ICollection<string> IDictionary<string, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<string, TValue>.Values => Values;

        public bool IsReadOnly => false;

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

        private Tries(Node root, char separator) : this(separator)
        {
            this.root = root;
        }

        public bool Add(string s, TValue value)
        {
            string[] prefix = s.Split(separator);

            Node currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                //move to the path
                if (currentNode.children.ContainsKey(key))
                {
                    currentNode = currentNode.children[key];
                }
                //current path does not exist, create nodes to create the path
                else
                {
                    Node newNode = new Node();
                    currentNode.children.Add(prefix[i], newNode);
                    currentNode = newNode;
                }
            }
            if (currentNode.isTerminated)
            {
                return false;
            }
            currentNode.value = value;
            currentNode.isTerminated = true;
            currentNode = root;
            currentNode.count++;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                currentNode = currentNode.children[key];
                currentNode.count++;
            }

            return true;
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool ContainsKey(string s)
        {
            string[] prefix = s.Split(separator);
            Node currentNode = root;

            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key))
                {
                    return false;
                }
                else
                {
                    currentNode = currentNode.children[key];
                }
            }
            return currentNode.isTerminated;
        }

        public bool Remove(string s)
        {
            string[] prefix = s.Split(separator);
            var currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key)) return false;
                var childNode = currentNode.children[key];
                currentNode = childNode;
            }

            if (!currentNode.isTerminated) return false;
            currentNode.isTerminated = false;

            currentNode = root;
            currentNode.count--;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                currentNode = currentNode.children[key];
                currentNode.count--;
            }
            return true;
        }

        public bool TryGetValue(string s, out TValue value)
        {
            string[] prefix = s.Split(separator);

            Node currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key))
                {
                    value = default;
                    return false;
                }
                else
                {
                    currentNode = currentNode.children[key];
                }
            }
            if (!currentNode.isTerminated)
            {
                value = default;
                return false;
            }
            value = currentNode.value;
            return true;
        }

        public TValue Get(string s)
        {
            string[] prefix = s.Split(separator);
            if (!TryGetNode(prefix, out var currentNode) || !currentNode.isTerminated)
                throw new KeyNotFoundException();
            return currentNode.value;
        }

        public void Set(string s, TValue value)
        {
            string[] prefix = s.Split(separator);

            Node currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                currentNode.count++;
                //move to the path
                if (currentNode.children.ContainsKey(key))
                {
                    currentNode = currentNode.children[key];
                }
                //current path does not exist, create nodes to create the path
                else
                {
                    Node newNode = new Node();
                    currentNode.children.Add(prefix[i], newNode);
                    currentNode = newNode;
                }
            }
            currentNode.value = value;
            currentNode.isTerminated = true;
        }

        public Tries<TValue> GetSubTrie(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Tries<TValue>(root, separator);
            }
            string[] prefix = s.Split(separator);
            return TryGetNode(prefix, out var currentNode)
                ? new Tries<TValue>(currentNode, separator)
                : throw new KeyNotFoundException();
        }

        public Tries<TValue> GetSubTrie(IEnumerable<string> prefix)
        {
            return (prefix is IList<string> ls
                ? TryGetNode(ls, out var currentNode)
                : TryGetNode(prefix, out currentNode))
                ? new Tries<TValue>(currentNode, separator)
                : throw new ArgumentException();
        }

        public bool TryGetSubTrie(string s, out Tries<TValue> trie)
        {
            string[] prefix = s.Split(separator);
            if (!TryGetNode(prefix, out Node currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Tries<TValue>(currentNode, separator);
            return true;
        }

        public bool TryGetSubTrie(IEnumerable<string> prefix, out Tries<TValue> trie)
        {
            if (prefix is string[] stra ? TryGetNode(stra, out Node currentNode) : TryGetNode(prefix, out currentNode))
            {
                trie = new Tries<TValue>(currentNode, separator);
                return true;
            }
            trie = null;
            return false;
        }

        public List<string> GetFirstLevelKeys()
        {
            return root.children.Keys.ToList();
        }

        private bool TryGetNode(IList<string> prefix, out Node node)
        {
            Node currentNode = root;
            for (int i = 0; i < prefix.Count; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.TryGetValue(key, out currentNode))
                {
                    node = null;
                    return false;
                }
            }
            node = currentNode;
            return node != null;
        }

        private bool TryGetNode(IEnumerable<string> prefix, out Node node)
        {
            Node currentNode = root;
            foreach (string key in prefix)
            {
                if (!currentNode.children.TryGetValue(key, out currentNode))
                {
                    node = null;
                    return false;
                }
            }
            node = currentNode;
            return node != null;
        }

        private void GetValues(List<TValue> list, Node node)
        {
            foreach (var item in node.children)
            {
                Node child = item.Value;
                if (child.isTerminated)
                {
                    list.Add(child.value);
                }
                GetValues(list, child);
            }
        }

        private void GetKeys(List<string> list, Node node, string prefix)
        {
            foreach (var item in node.children)
            {
                Node child = item.Value;
                string conbinedKey = prefix + item.Key;
                if (child.isTerminated)
                {
                    list.Add(conbinedKey);
                }
                GetKeys(list, child, conbinedKey + separator);
            }
        }

        private void GetKeyValuePairs(List<KeyValuePair<string, TValue>> list, Node node, string prefix = "")
        {
            foreach (var item in node.children)
            {
                Node child = item.Value;
                string conbinedKey = prefix + item.Key;
                if (child.isTerminated)
                {
                    list.Add(new KeyValuePair<string, TValue>(conbinedKey, child.value));
                }
                GetKeyValuePairs(list, child, conbinedKey + ".");
            }
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            var list = new List<KeyValuePair<string, TValue>>();
            GetKeyValuePairs(list, root);
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary<string, TValue>.Add(string key, TValue value)
        {
            Add(key, value);
        }

        public void Clear()
        {
            root = new Node();
        }

        public bool Clear(string key)
        {
            if (!TryGetNode(key.Split(separator), out var node)) return false;
            node.isTerminated = false;
            return true;
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            string s = item.Key;
            string[] prefix = s.Split(separator);
            var currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key)) return false;
                var childNode = currentNode.children[key];
                currentNode = childNode;
            }
            return currentNode.isTerminated && Equals(currentNode.value, item.Value);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            root?.TraverseCopy(array, ref index, separator, new());
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            string s = item.Key;
            string[] prefix = s.Split(separator);
            var currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key)) return false;
                var childNode = currentNode.children[key];
                currentNode = childNode;
            }
            if (!currentNode.isTerminated || Equals(currentNode.value, item.Value)) return false;

            currentNode.isTerminated = false;
            currentNode = root;
            currentNode.count--;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                currentNode = currentNode.children[key];
                currentNode.count--;
            }
            return true;
        }

        public Tries<TValue> Clone()
        {
            return new Tries<TValue>(root.Clone(), separator);
        }




        public void CopyFirstLevelKeys(List<string> copyTo)
        {
            copyTo.Clear();
            copyTo.AddRange(root.children.Keys);
        }

        public void CopyKeys(List<string> copyTo)
        {
            copyTo.Clear();
            GetKeys(copyTo, root, string.Empty);
        }
    }

    public interface ITries<TValue>
    {
        TValue Get(string s);
    }
}