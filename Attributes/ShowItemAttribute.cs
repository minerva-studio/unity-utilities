using System;
using UnityEngine;

namespace Minerva.Module
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ShowItemAttribute : PropertyAttribute
    {
        public string path;

        public ShowItemAttribute(string path)
        {
            this.path = path;
        }
    }
}