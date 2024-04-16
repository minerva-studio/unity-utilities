using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI enable state
    /// </summary>
    public readonly struct GUIEnable : IDisposable
    {
        private readonly bool lastState;

        public GUIEnable(bool state)
        {
            this.lastState = GUI.enabled;
            GUI.enabled = state;
        }

        public static GUIEnable By(bool v)
        {
            return new GUIEnable(v);
        }

        public void Dispose()
        {
            GUI.enabled = lastState;
        }
    }
}