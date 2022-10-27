using System;

namespace Minerva.Module
{
    /// <summary>
    /// A reminder that you must call the base method of this method
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MustCallBaseAttribute : Attribute
    {
        public MustCallBaseAttribute()
        {
        }
    }
}