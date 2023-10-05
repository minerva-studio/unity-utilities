using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    public class Trie : IEnumerable<string>, IEnumerable, ICollection<string>
    {
        private class Node
        {
            public Dictionary<string, Node> children;
            public int count;
            public bool isTerminated;

            public Node()
            {
                children = new Dictionary<string, Node>();
            }

            public Node(bool isTerminated) : this()
            {
                this.isTerminated = isTerminated;
            }


            public Node Clone()
            {
                Node node = new()
                {
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
        }



        readonly char separator = '.';
        Node root;

        public bool this[string key]
        {
            get { return Contains(key); }
            set { Set(key, value); }
        }

        public int Count => root.count;

        public ICollection<string> FirstLevelKeys => root.children.Keys.ToList();

        public List<string> Keys
        {
            get
            {
                List<string> array = new List<string>();
                GetKeys(array, root, "");
                return array;
            }
        }

        public bool IsReadOnly => false;

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
            this.root = root;
        }


        void ICollection<string>.Add(string item) => Add(item);
        public bool Add(string s)
        {
            if (Contains(s))
            {
                return false;
            }

            string[] prefix = GetPrefix(s);

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
            currentNode.isTerminated = true;

            return true;
        }

        public bool Contains(string s)
        {
            string[] prefix = GetPrefix(s);
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
            string[] prefix = GetPrefix(s);
            Node currentNode = root;
            List<Node> stack = new List<Node>();
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                if (!currentNode.children.ContainsKey(key)) return false;
                Node childNode = currentNode.children[key];
                stack.Add(currentNode);
                currentNode = childNode;
            }

            currentNode.isTerminated = false;
            foreach (var item in stack)
            {
                item.count--;
            }
            return true;
        }

        public bool Set(string s, bool value)
        {
            if (value)
            {
                return Add(s);
            }
            else
            {
                return Remove(s);
            }
        }

        public Trie GetSubTrie(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Trie(root, separator);
            }
            string[] prefix = GetPrefix(s);
            Node currentNode = GetNode(prefix);
            return currentNode == null ? throw new ArgumentException() : new Trie(currentNode, separator);
        }

        public bool TryGetSubTrie(string s, out Trie trie)
        {
            string[] prefix = GetPrefix(s);
            if (!TryGetNode(prefix, out Node currentNode))
            {
                trie = null;
                return false;
            }
            trie = new Trie(currentNode, separator);
            return true;
        }

        public List<string> GetChildrenKeys()
        {
            return root.children.Keys.ToList();
        }

        private Node GetNode(string[] prefix)
        {
            Node currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
            {
                string key = prefix[i];
                currentNode = currentNode.children[key];
            }

            return currentNode;
        }

        private bool TryGetNode(string[] prefix, out Node node)
        {
            Node currentNode = root;
            for (int i = 0; i < prefix.Length; i++)
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

        private void GetKeys(List<string> list, Node node, string prefix = "")
        {
            foreach (var item in node.children)
            {
                Node child = item.Value;
                string conbinedKey = prefix + item.Key;
                if (child.isTerminated)
                {
                    list.Add(conbinedKey);
                }
                GetKeys(list, child, conbinedKey + ".");
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            var list = new List<string>();
            GetKeys(list, root);
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string[] GetPrefix(string s)
        {
            if (s.EndsWith(separator)) return s.Split(separator)[..^1];
            return s.Split(separator);
        }

        public void Clear()
        {
            root = new Node();
        }

        public void Clear(string key)
        {
            var node = GetNode(GetPrefix(key));
            node.isTerminated = false;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            var list = new List<string>();
            GetKeys(list, root);
            Array.Copy(list.ToArray(), 0, array, arrayIndex, list.Count);
        }

        public Trie Clone()
        {
            return new Trie(root.Clone(), separator);
        }
    }
}