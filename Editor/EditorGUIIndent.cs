using System;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI enable state
    /// </summary>
    public class EditorGUIIndent : IDisposable
    {
        private int indent;
        private bool disposed;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EditorGUIIndent Default => new EditorGUIIndent();

        public EditorGUIIndent() : this(1) { }

        public EditorGUIIndent(int indent)
        {
            this.disposed = false;
            this.indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = EditorGUI.indentLevel + indent;
        }

        public static EditorGUIIndent By(int indentation = 1)
        {
            return new EditorGUIIndent(indentation);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            EditorGUI.indentLevel = indent;
        }
    }
}