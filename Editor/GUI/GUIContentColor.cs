using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Control GUI color state
    /// </summary>
    public readonly struct GUIContentColor : IDisposable
    {
        private readonly Color lastState;

        public static GUIContentColor red => new GUIContentColor(Color.red);
        public static GUIContentColor green => new GUIContentColor(Color.green);
        public static GUIContentColor yellow => new GUIContentColor(Color.yellow);
        public static GUIContentColor blue => new GUIContentColor(Color.blue);


        public GUIContentColor(Color color)
        {
            this.lastState = GUI.contentColor;
            GUI.contentColor = color;
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