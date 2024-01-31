using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI color state
    /// </summary>
    public class GUIBackgroundColor : IDisposable
    {
        private readonly Color lastState;

        public GUIBackgroundColor(Color lastState)
        {
            this.lastState = GUI.backgroundColor;
            GUI.backgroundColor = lastState;
        }

        public static GUIColor By(Color v)
        {
            return new GUIColor(v);
        }

        public void Dispose()
        {
            GUI.backgroundColor = lastState;
        }
    }
}