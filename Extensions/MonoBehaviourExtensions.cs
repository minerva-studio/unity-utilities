using System.Threading.Tasks;
using UnityEngine;

namespace Minerva.Module
{
    public static class MonoBehaviourExtensions
    {
        public static async Task AsTask(this MonoBehaviour monoBehaviour)
        {
            while (monoBehaviour)
            {
                await Task.Yield();
            }
        }
    }
}