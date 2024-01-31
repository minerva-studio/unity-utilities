using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public class GUIVerticalLayout : IDisposable
    {
        public GUIVerticalLayout(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }

        public GUIVerticalLayout(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            GUILayout.EndVertical();
        }
    }
}