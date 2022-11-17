using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A serializable dictionary
    /// </summary>
    [Serializable]
    public class SerializableDictionary : Dictionary<string, string>, ISerializationCallbackReceiver
    {
        public List<StringPair> arguments;

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var item in arguments)
            {
                Add(item.Key, item.Value);
            }
        }

        public void OnBeforeSerialize()
        {
            arguments.Clear();
            foreach (var item in this)
            {
                arguments.Add(new StringPair(item.Key, item.Value));
            }
            arguments.Sort();
        }
    }
}