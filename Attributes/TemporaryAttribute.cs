using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// a field, method, or anything that is temporary
    /// <br/>
    /// Not useful really
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class TemporaryAttribute : PropertyAttribute
    {
        public TemporaryAttribute()
        {
            Debug.LogWarning($"Warning: this field is a temporary solution only, the perminant solution has not yet be implemented yet.");
        }
        public TemporaryAttribute(string value)
        {
            Debug.LogWarning(value);
        }
    }
}