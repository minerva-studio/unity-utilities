using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public class GUIHorizontalLayout : IDisposable
    {
        public GUIHorizontalLayout(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }

        public GUIHorizontalLayout(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }
}