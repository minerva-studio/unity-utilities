using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI color state
    /// </summary>
    public class GUIContentColor : IDisposable
    {
        private readonly Color lastState;

        public GUIContentColor(Color lastState)
        {
            this.lastState = GUI.contentColor;
            GUI.contentColor = lastState;
        }

        public static GUIColor By(Color v)
        {
            return new GUIColor(v);
        }

        public void Dispose()
        {
            GUI.contentColor = lastState;
        }
    }
}