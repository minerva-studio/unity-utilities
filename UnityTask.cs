using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Tasks that behave like some of the Unity Coroutine <see cref="YieldInstruction"/>
    /// </summary>
    public static class UnityTask
    {
        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitUntil"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task WaitUntil(Func<bool> predicate)
        {
            var tcs = new TaskCompletionSource<bool>();

            void CheckPredicate()
            {
                if (predicate())
                {
                    tcs.SetResult(true);
                }
                else
                {
                    // Retry after a short delay
                    Task.Delay(TimeSpan.FromSeconds(Time.deltaTime)).ContinueWith(_ => CheckPredicate());
                }
            }

            CheckPredicate();

            return tcs.Task;
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitWhile"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task WaitWhile(Func<bool> predicate)
        {
            var tcs = new TaskCompletionSource<bool>();

            void CheckPredicate()
            {
                if (predicate())
                {
                    // Retry after a short delay
                    Task.Delay(TimeSpan.FromSeconds(Time.deltaTime)).ContinueWith(_ => CheckPredicate());
                }
                else
                {
                    tcs.SetResult(true);
                }
            }

            CheckPredicate();

            return tcs.Task;
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSecondsRealtime"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Task WaitForSecondsRealtime(float seconds)
        {
            return Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSeconds"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Task WaitForSeconds(float seconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            var ending = Time.time + seconds;
            return WaitUntil(() => ending >= Time.time);
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForFixedUpdate"/>
        /// <br/>
        /// Warning: this implementation is not garantee to be correct and suggest not to use it
        /// </summary> 
        /// <returns></returns>
        public static Task WaitForFixedUpdate()
        {
            var obj = new GameObject("Unity Task");
            var tcs = new TaskCompletionSource<bool>();
            obj.AddComponent<FixedUpdateWaiter>().tcs = tcs;
            return tcs.Task;
        }


        class FixedUpdateWaiter : MonoBehaviour
        {
            internal TaskCompletionSource<bool> tcs;

            private void FixedUpdate()
            {
                tcs.SetResult(true);
                Destroy(gameObject);
            }
        }
    }
}