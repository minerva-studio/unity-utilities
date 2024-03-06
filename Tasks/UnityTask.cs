using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Minerva.Module.Tasks
{
    /// <summary>
    /// Tasks that behave like some of the Unity Coroutine <see cref="YieldInstruction"/> and completely written in Task
    /// </summary>
    public static class UnityTask
    {
        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSeconds"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSeconds(float seconds)
        {
            float current = 0;
            while (current < seconds)
            {
                current += Time.deltaTime;
                await Task.Yield();
            }
        }
    }
}