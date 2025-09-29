using System;

namespace Minerva.Module
{
    public struct IndexGenerator
    {
        public static readonly IndexGenerator Default = new();

        private System.Random r;
        private Func<int, int> func;

        public IndexGenerator(Random r) : this()
        {
            this.r = r;
        }

        public IndexGenerator(Func<int, int> func) : this()
        {
            this.func = func;
        }

        public readonly int Get(int length)
        {
            return r?.Next(length) ?? func?.Invoke(length) ?? UnityEngine.Random.Range(0, length);
        }

        public static implicit operator IndexGenerator(System.Random random)
        {
            return new IndexGenerator(random);
        }

        public static implicit operator IndexGenerator(Func<int, int> func)
        {
            return new IndexGenerator(func);
        }
    }
}
