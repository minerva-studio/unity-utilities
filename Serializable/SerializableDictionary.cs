using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minerva.Module
{
    [Serializable]
    public class SerializableDictionary : SerializableDictionary<string, string>
    {
    }

    /// <summary>
    /// A serializable dictionary
    /// </summary> 
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class Data
        {
            public TKey key;
            public TValue value;

            public Data(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }

        [SerializeField]
        private Data[] keyValuePaires;

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var item in keyValuePaires)
            {
                Add(item.key, item.value);
            }
        }

        public void OnBeforeSerialize()
        {
            keyValuePaires = this.Select(s => new Data(s.Key, s.Value)).ToArray();
        }
    }
}