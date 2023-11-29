using System;
using UnityEditor;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI enable state
    /// </summary>
    public class EditorGUIIndent : IDisposable
    {
        private readonly int indent;

        public static EditorGUIIndent Default => new EditorGUIIndent();

        public EditorGUIIndent()
        {
            this.indent = 1;
            EditorGUI.indentLevel += indent;
        }

        public EditorGUIIndent(int indent)
        {
            this.indent = indent;
            EditorGUI.indentLevel += indent;
        }

        public static EditorGUIIndent By(int indentation = 1)
        {
            return new EditorGUIIndent(indentation);
        }

        public void Dispose()
        {
            EditorGUI.indentLevel -= indent;
        }
    }
}