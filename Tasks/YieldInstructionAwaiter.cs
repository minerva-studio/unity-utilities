using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;

namespace Minerva.Module.Tasks
{
    public class YieldInstructionAwaiter : INotifyCompletion
    {
        Task task;


        public YieldInstructionAwaiter(WaitForSeconds awaitable)
        {
            float time = (float)typeof(WaitForSeconds).GetType()
                .GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(awaitable);
            task = UnityTask.WaitForSeconds(time);
        }

        public YieldInstructionAwaiter(WaitForSecondsRealtime awaitable)
        {
            float time = awaitable.waitTime;
            task = UnityTask.WaitForSecondsRealtime(time);
        }

        public YieldInstructionAwaiter(WaitUntil awaitable)
        {
            var time = (Func<bool>)typeof(WaitUntil).GetType()
                .GetField("m_Predicate", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(awaitable);

            task = UnityTask.WaitUntil(time);
        }

        public YieldInstructionAwaiter(WaitWhile awaitable)
        {
            var time = (Func<bool>)typeof(WaitWhile).GetType()
                .GetField("m_Predicate", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(awaitable);

            task = UnityTask.WaitWhile(time);
        }

        public bool IsCompleted => task.IsCompleted;

        public void OnCompleted(Action continuation)
        {
            continuation?.Invoke(); // Notify completion 
        }

        public void GetResult()
        {
            task.GetAwaiter().GetResult();
        }
    }
}