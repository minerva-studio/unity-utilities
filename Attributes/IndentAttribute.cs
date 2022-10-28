using UnityEngine;

namespace Minerva.Module
{
    public class IndentAttribute : PropertyAttribute
    {
        public int indent;

        public IndentAttribute()
        {
            this.indent = 1;
        }
        public IndentAttribute(int indent)
        {
            this.indent = indent;
        }
    }
}