using System;
using System.Collections;


namespace Minerva.Module
{
    /// <summary>
    /// an advanced coroutine helper that convert normal function to a coroutine
    /// </summary>
    public struct CoroutineFunction
    {
        private Action<object[]> paramFunc;
        private Func<object[], IEnumerator> enumerator;
        private object[] vs;


        public IEnumerator Execute()
        {
            return enumerator != null ? GetEnumerator()
                : paramFunc != null ? AsIEnumerator(paramFunc)
                : Empty();
        }

        public IEnumerator Empty()
        {
            yield return null;
        }

        public IEnumerator GetEnumerator()
        {
            return enumerator(vs);
        }

        public IEnumerator AsIEnumerator(Action<object[]> func)
        {
            yield return null;
            func(vs);
            yield return null;
        }

        public IEnumerator AsIEnumerator(Action func)
        {
            yield return null;
            func();
            yield return null;
        }


        public CoroutineFunction(Action func)
        {
            paramFunc = (vs) => func();
            enumerator = null;
            vs = null;
        }

        public CoroutineFunction(Action<object[]> paramFunc, params object[] vs)
        {
            this.paramFunc = paramFunc;
            enumerator = null;
            this.vs = vs;
        }

        public CoroutineFunction(Func<IEnumerator> paramFunc)
        {
            this.paramFunc = null;
            enumerator = (vs) => paramFunc();
            vs = null;
        }

        public CoroutineFunction(Func<object[], IEnumerator> enumerator, params object[] vs)
        {
            paramFunc = null;
            this.enumerator = enumerator;
            this.vs = vs;
        }

        public bool Equals(CoroutineFunction other)
        {
            return other.enumerator == enumerator && paramFunc == other.paramFunc && Equals(vs, other.vs);
        }

        public override bool Equals(object obj)
        {
            return obj is CoroutineFunction ? Equals((CoroutineFunction)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CoroutineFunction left, CoroutineFunction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoroutineFunction left, CoroutineFunction right)
        {
            return !(left == right);
        }
    }
}