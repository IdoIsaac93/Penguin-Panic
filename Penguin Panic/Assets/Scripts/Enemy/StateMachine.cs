using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public class State
    {
        //name of state
        public string name;

        //handle things that might happen on entering this state
        public System.Action onEnter;

        //handle things that might happen on exiting this state
        public System.Action onExit;

        //handle things that might happen while staying on this state
        public System.Action onStay;
    }

    Dictionary<string, State> states = new();

    public State currentState;
    public State initialState;

    public State CreateState(string name)
    {
        var newState = new State { name = name };
        if (states.Count == 0)
        {
            initialState = newState;
        }
        states[name] = newState;
        return newState;
    }
    public void Update()
    {
        if (states.Count == 0 || initialState == null)
        {
            Debug.Log("No states!");
        }
        if (currentState == null)
        {
            TransitionTo(initialState);
        }
        currentState.onStay?.Invoke();
    }
    public void TransitionTo(State newState)
    {
        //check for null
        if (newState == null)
        {
            Debug.Log("New state is null!");
            return;
        }

        //check onExit of current state
        if (currentState != null && currentState.onExit != null)
        {
            currentState.onExit();
        }

        currentState = newState;

        //check onEnter of the newState
        newState.onEnter?.Invoke();
    }
    public void TransitionTo(string newStateName)
    {
        if (!states.ContainsKey(newStateName))
        {
            Debug.Log($"StateMachine doesn't contain the state {newStateName}");
            return;
        }
        var state = states[newStateName];
        TransitionTo(state);
    }
}