using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI enable state
    /// </summary>
    public class GUIEnable : IDisposable
    {
        private readonly bool lastState;

        public GUIEnable()
        {
            lastState = GUI.enabled;
            GUI.enabled = true;
        }

        public GUIEnable(bool lastState)
        {
            this.lastState = GUI.enabled;
            GUI.enabled = lastState;
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