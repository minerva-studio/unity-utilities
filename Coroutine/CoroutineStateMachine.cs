using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Abstract class of building an state machine in Unity Coroutine (of state Enum <see cref="T"/>)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CoroutineStateMachine<T> : CoroutineStateMachineBase, IEnumerator where T : struct, Enum
    {
        [SerializeField] T state;
        [SerializeField] T nextState;
        readonly Dictionary<T, Func<CoroutineStateMachine<T>, IEnumerator>> actionMap;

        public T CurrentState => state;
        public T NextState => nextState;


        protected CoroutineStateMachine()
        {
            actionMap = new Dictionary<T, Func<CoroutineStateMachine<T>, IEnumerator>>();
        }




        /// <summary>
        /// Enumerator  Move next
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            // ending state
            if (end)
            {
                currentStateEnumerator = null;
                return false;
            }

            // init
            if (!started)
            {
                started = true;
                currentStateHadNext = true;
                var firstState = InitStateMachine();
                currentStateEnumerator = actionMap[firstState](this);
                currentStateHadNext = currentStateEnumerator.MoveNext();
                return true;
            }

            // try move current next
            if (currentStateHadNext && currentStateEnumerator.MoveNext())
            {
                return true;
            }

            #region Next state 
            currentStateEnumerator = MoveToNextState();
            // end of states
            if (currentStateEnumerator == null)
            {
                return end = false;
            }

            currentStateHadNext = currentStateEnumerator.MoveNext();
            return true;
            #endregion
            //return end = false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        IEnumerator MoveToNextState()
        {
            if (delayCounter > 0)
            {
                return Delay();
            }

            state = nextState;
            if (!actionMap.ContainsKey(state))
            {
                Debug.LogErrorFormat("Invalid state {0}", state);
                return null;
            }

            return actionMap[state](this);
        }

        IEnumerator Delay()
        {
            while (delayCounter > 0)
            {
                delayCounter -= Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Init the state machine
        /// </summary>
        /// <returns> The first state of the state machine </returns>
        protected abstract T InitStateMachine();


        /// <summary>
        /// Add state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="method"></param>
        public void AddState(T state, Func<CoroutineStateMachine<T>, IEnumerator> method)
        {
            actionMap.Add(state, method);
        }

        public void AddState(T state, Func<CoroutineStateMachineBase, IEnumerator> method)
        {
            actionMap.Add(state, method);
        }

        /// <summary>
        /// Remove given state
        /// </summary>
        /// <param name="state"></param>
        public void RemoveState(T state)
        {
            actionMap.Remove(state);
        }

        /// <summary>
        /// Switch the state machine to next state
        /// </summary>
        /// <param name="nextState"></param>
        public void SwitchState(T nextState)
        {
            this.delayCounter = 0;
            this.nextState = nextState;
        }

        /// <summary>
        /// Switch the state machine to next state with <paramref name="delay"/> delay
        /// </summary>
        /// <param name="nextState"></param>
        /// <param name="delay"></param>
        public void SwitchState(T nextState, float delay)
        {
            this.delayCounter = delay;
            this.nextState = nextState;
        }

        public override void AddState(int state, Func<CoroutineStateMachineBase, IEnumerator> method)
        {
            AddState((T)(object)state, method);
        }

        public override void RemoveState(int state)
        {
            RemoveState((T)(object)state);
        }

        public override void SwitchState(int nextState)
        {
            SwitchState((T)(object)nextState, 0);
        }

        public override void SwitchState(int nextState, float delay)
        {
            SwitchState((T)(object)nextState, delay);
        }
    }
}