using System;
using UnityEditor;

namespace Minerva.Module.Editor
{
    public static class TypeSearch
    {
        static Tries<Type> classTrie;

        public static Type GetType(string name)
        {
            if (classTrie == null)
                Init();
            return classTrie.TryGetValue(name, out var type) ? type : null;
        }

        public static Tries<Type> GetTypes(string name)
        {
            if (classTrie == null)
                Init();
            return classTrie.TryGetSubTrie(name, out var tries) ? tries.Clone() : null;
        }

        public static Tries<Type> GetTypesDerivedFrom(Type baseType)
        {
            var types = TypeCache.GetTypesDerivedFrom(baseType);
            var trie = new Tries<Type>();
            foreach (var type in types)
            {
                if (type.IsNotPublic)
                {
                    continue;
                }
                if (type.IsNestedPrivate)
                {
                    continue;
                }
                // static
                //if (type.IsAbstract && type.IsSealed)
                //{
                //    continue;
                //}
                trie.Add(type.FullName, type);
            }
            return trie;
        }

        private static void Init()
        {
            classTrie = new();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int a = 0; a < assemblies.Length; a++)
            {
                System.Reflection.Assembly item = assemblies[a];
                Type[] array = item.GetTypes();
                for (int t = 0; t < array.Length; t++)
                {
                    Type type = array[t];
                    if (type.IsNotPublic)
                    {
                        continue;
                    }
                    if (type.IsNestedPrivate)
                    {
                        continue;
                    }
                    classTrie.Add(type.FullName, type);
                }
            }
        }
    }
}