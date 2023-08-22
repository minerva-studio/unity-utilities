using UnityEngine;

namespace Minerva.Module.Tasks
{
    /// <summary>
    /// a way to return to the main unity thread when using multiple threads with async methods
    /// </summary> 
    public class WaitForUpdate : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return false; }
        }
    }
}