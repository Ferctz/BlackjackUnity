using UnityEngine;
using System.Collections.Generic;

namespace Blackjack.Utils
{
    /// <summary>
    /// Light weight state machine
    /// </summary>
    /// <typeparam name="T"></typeparam>

    class StateMachine<T>
    {
        public delegate void StateFunc();

        public void Add(T id, StateFunc enter, StateFunc update, StateFunc leave)
        {
            m_States.Add(id, new State(id, enter, update, leave));
        }

        public T CurrentState()
        {
            return m_CurrentState.Id;
        }

        public void Update()
        {
            // fixes compile when update delegate is null but maybe this should always be non-null
            if (m_CurrentState.Update != null)
            {
                m_CurrentState.Update();
            }
        }

        public void Shutdown()
        {
            if (m_CurrentState != null && m_CurrentState.Leave != null)
            {
                m_CurrentState.Leave();
            }
            m_CurrentState = null;
        }

        public void SwitchTo(T state)
        {
            if (!m_States.ContainsKey(state))
            {
                Debug.LogWarning("Trying to switch to unknown state " + state.ToString());
                return;
            }

            if (m_CurrentState != null && m_CurrentState.Id.Equals(state))
            {
                Debug.LogWarning("Trying to switch to " + state.ToString() + " but that is already current state");
            }

            State newState = m_States[state];
            Debug.Log("Switching state: " + (m_CurrentState != null ? m_CurrentState.Id.ToString() : "null") + " -> " + state.ToString());

            if (m_CurrentState != null && m_CurrentState.Leave != null)
            {
                m_CurrentState.Leave();
            }
            if (newState.Enter != null)
            {
                newState.Enter();
            }
            m_CurrentState = newState;
        }

        class State
        {
            public State(T id, StateFunc enter, StateFunc update, StateFunc leave)
            {
                Id = id;
                Enter = enter;
                Update = update;
                Leave = leave;
            }

            public T Id;
            public StateFunc Enter;
            public StateFunc Update;
            public StateFunc Leave;
        }

        State m_CurrentState = null;
        Dictionary<T, State> m_States = new Dictionary<T, State>();
    }
}
