using System;
using System.Collections;
using UnityEngine;

namespace Minerva.Module
{
    public static class MonoBehaviourExtensions
    {
        public static void StartDelayEvent(this MonoBehaviour behaviour, Action<MonoBehaviour> action, float duration)
            => StartDelayEvent<MonoBehaviour>(behaviour, action, duration);
        public static void StartDelayEvent<T>(this T behaviour, Action<T> action, float duration) where T : MonoBehaviour
        {
            behaviour.StartCoroutine(Run());

            IEnumerator Run()
            {
                float counter = duration;
                while (counter > 0)
                {
                    counter -= Time.deltaTime;
                    yield return null;
                }
                if (behaviour) action?.Invoke(behaviour);
            }
        }

        public static void StartDelayEvent(this MonoBehaviour behaviour, Action action, float duration)
            => StartDelayEvent<MonoBehaviour>(behaviour, action, duration);
        public static void StartDelayEvent<T>(this T behaviour, Action action, float duration) where T : MonoBehaviour
        {
            behaviour.StartCoroutine(Run());

            IEnumerator Run()
            {
                float counter = duration;
                while (counter > 0)
                {
                    counter -= Time.deltaTime;
                    yield return null;
                }
                if (behaviour) action?.Invoke();
            }
        }


    }
}