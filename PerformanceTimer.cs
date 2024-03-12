using System;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A simple clock by using statement
    /// </summary>
    public struct PerformanceTimer : IDisposable
    {
        private bool echo;
        private DateTime start;
        private string message;
        private Func<string> provider;

        public PerformanceTimer(bool echo)
        {
            this.echo = echo;
            this.start = DateTime.Now;
            this.message = "Section took {0} ms";
            this.provider = null;
        }

        public PerformanceTimer(bool echo, string message)
        {
            this.echo = echo;
            this.start = DateTime.Now;
            this.message = message;
            this.provider = null;
        }

        public PerformanceTimer(bool echo, Func<string> provider)
        {
            this.echo = echo;
            this.start = DateTime.Now;
            this.message = null;
            this.provider = provider;
        }

        public void Dispose()
        {
            if (echo)
            {
                Debug.LogFormat(this.message ?? provider(), (DateTime.Now.Ticks - start.Ticks) / 10000f);
            }
        }
    }
}