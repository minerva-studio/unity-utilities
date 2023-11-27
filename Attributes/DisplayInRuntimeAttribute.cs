using System;
using UnityEngine;

namespace Minerva.Module
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DisplayInRuntimeAttribute : PropertyAttribute
    {

    }
}