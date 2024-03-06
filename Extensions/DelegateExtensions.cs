using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minerva.Module
{
    public static class DelegateExtensions
    {
        public static void SaveInvoke(this Action action)
        {
            SaveInvokeDelegate(action);
        }

        public static void SaveInvoke<T>(this Action<T> action, T arg)
        {
            SaveInvokeDelegate(action, arg);
        }

        public static void SaveInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            SaveInvokeDelegate(action, arg1, arg2);
        }

        public static void SaveInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3);
        }

        public static void SaveInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3, arg4);
        }

        public static void SaveInvoke<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3, arg4, arg5);
        }

        public static void SaveInvoke<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void SaveInvoke<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void SaveInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            SaveInvokeDelegate(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static TResult SaveInvoke<TResult>(Func<TResult> func)
        {
            return (TResult)SaveInvokeDelegate(func);
        }

        public static TResult SaveInvoke<T, TResult>(Func<T, TResult> func, T arg)
        {
            return (TResult)SaveInvokeDelegate(func, arg);
        }

        public static TResult SaveInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2);
        }

        public static TResult SaveInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3);
        }

        public static TResult SaveInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3, arg4);
        }

        public static TResult SaveInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3, arg4, arg5);
        }

        public static TResult SaveInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static TResult SaveInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static TResult SaveInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return (TResult)SaveInvokeDelegate(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }



        public static void SaveInvoke(this UnityEvent unityEvent)
        {
            try { unityEvent?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
        }

        public static void SaveInvoke<T0>(this UnityEvent<T0> unityEvent, T0 arg)
        {
            try { unityEvent?.Invoke(arg); } catch (Exception e) { Debug.LogException(e); }
        }

        public static void SaveInvoke<T0, T1>(this UnityEvent<T0, T1> unityEvent, T0 arg0, T1 arg1)
        {
            try { unityEvent?.Invoke(arg0, arg1); } catch (Exception e) { Debug.LogException(e); }
        }



        public static object SaveInvoke<T>(this T @delegate, params object[] args) where T : Delegate
        {
            object result = null;
            foreach (var item in @delegate.GetInvocationList())
            {
                try
                {
                    result = item?.DynamicInvoke(args);
                }
                catch (Exception e) { Debug.LogException(e); }
            }
            return result;
        }

        private static object SaveInvokeDelegate(Delegate @delegate, params object[] args)
        {
            object result = null;
            foreach (var item in @delegate.GetInvocationList())
            {
                try
                {
                    result = item?.DynamicInvoke(args);
                }
                catch (Exception e) { Debug.LogException(e); }
            }
            return result;
        }
    }
}
