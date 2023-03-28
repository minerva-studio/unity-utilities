using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Mark that the method is called in Editor, it is not useless
    /// <br></br>
    /// Not useful really
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CallInEditorAttribute : PropertyAttribute
    {
        public CallInEditorAttribute()
        {
            Debug.LogWarning($"Warning: this field is a temporary solution only, the perminant solution has not yet be implemented yet.");
        }

        public CallInEditorAttribute(string value)
        {
            Debug.LogWarning(value);
        }
    }
}