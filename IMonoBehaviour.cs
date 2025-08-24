using System.Threading;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// interface that connect to a monobehaviour directly
    /// <br> note: an class/struct of <see cref="IMonoBehaviour"/> doesn't have to be a subclass of <see cref="MonoBehaviour"/> </br>
    /// </summary>
    public interface IMonoBehaviour
    {
        CancellationToken destroyCancellationToken { get; }
        public bool enabled { get; set; }
        public Transform transform { get; }
        public GameObject gameObject { get; }
        /// <summary>
        /// The MonoBehaviour self
        /// </summary>
        public MonoBehaviour Script => this as MonoBehaviour;
    }

    /// <summary>
    /// interface that connect to a monobehaviour directly
    /// <br> note: an class/struct of <see cref="IMonoBehaviour"/> doesn't have to be a subclass of <see cref="MonoBehaviour"/> </br>
    /// </summary>
    public interface IMonoBehaviour<out T> : IMonoBehaviour where T : MonoBehaviour
    {
        public new T Script => this as T;
    }

    public static class MonobehaviourInterfaceExtension
    {
        /// <summary>
        /// checking unity object exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns>null (real null) if the object does not exist, the object if the object exist</returns>
        public static MonoBehaviour Exist(this IMonoBehaviour instance)
        {
            return instance != null && instance.Script ? instance.Script : null;
        }
    }
}