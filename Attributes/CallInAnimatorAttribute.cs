using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Mark that the method is called in Animator, it is not useless
    /// <br></br>
    /// Not useful really
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CallInAnimatorAttribute : PropertyAttribute
    {
        public CallInAnimatorAttribute()
        {
            Debug.LogWarning($"Warning: this field is a temporary solution only, the perminant solution has not yet be implemented yet.");
        }

        public CallInAnimatorAttribute(string value)
        {
            Debug.LogWarning(value);
        }
    }
}