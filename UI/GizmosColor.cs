using System;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public readonly struct GizmosColor : IDisposable
    {
        private readonly Color lastState;

        public GizmosColor(Color color)
        {
            this.lastState = Gizmos.color;
            Gizmos.color = color;
        }

        public readonly void Dispose()
        {
            Gizmos.color = lastState;
        }
    }
}