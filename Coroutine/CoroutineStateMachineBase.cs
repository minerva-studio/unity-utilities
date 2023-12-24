using System;
using System.Collections;

namespace Minerva.Module
{
    public abstract class CoroutineStateMachineBase
    {
        protected bool started;
        protected bool end;
        protected IEnumerator currentStateEnumerator;
        protected bool currentStateHadNext;
        protected float delayCounter;
        public object Current => currentStateEnumerator?.Current;// Update();

        public void End()
        {
            end = true;
        }

        /// <summary>
        /// Add state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="method"></param>
        public abstract void AddState(int state, Func<CoroutineStateMachineBase, IEnumerator> method);

        /// <summary>
        /// Remove given state
        /// </summary>
        /// <param name="state"></param>
        public abstract void RemoveState(int state);

        /// <summary>
        /// Switch the state machine to next state
        /// </summary>
        /// <param name="nextState"></param>
        public abstract void SwitchState(int nextState);

        /// <summary>
        /// Switch the state machine to next state with <paramref name="delay"/> delay
        /// </summary>
        /// <param name="nextState"></param>
        /// <param name="delay"></param>
        public abstract void SwitchState(int nextState, float delay);

    }
}