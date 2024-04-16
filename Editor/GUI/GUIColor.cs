using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI color state
    /// </summary>
    public readonly struct GUIColor : IDisposable
    {
        private readonly Color lastState;

        public GUIColor(Color lastState)
        {
            this.lastState = GUI.color;
            GUI.color = lastState;
        }

        public static GUIColor By(Color v)
        {
            return new GUIColor(v);
        }

        public void Dispose()
        {
            GUI.color = lastState;
        }
    }
}