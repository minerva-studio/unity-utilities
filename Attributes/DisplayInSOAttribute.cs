using System;
using UnityEngine;

namespace Minerva.Module
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayInSOAttribute : PropertyAttribute
    {
        public bool displayInScriptableObject;
        public DisplayInSOAttribute()
        {
        }
        public DisplayInSOAttribute(bool value)
        {
            displayInScriptableObject = value;
        }
    }
}