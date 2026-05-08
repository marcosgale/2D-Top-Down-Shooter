using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    /// <summary> The gameobject that owns the FSM. </summary>
    public GameObject Owner;

    /// <summary> All available states in the FSM. </summary>
    private Dictionary<string, AIState> _states = new Dictionary<string, AIState>();

    /// <summary> The current state of the FSM. </summary>
    private AIState _currentState;
    
    /// <summary> The state that this FSM switches to in the next tick. </summary>
    private AIState _upcomingState;

    /// <summary> Adds a new state to the FSM. </summary>
    public void AddState(AIState state)
    {
        Debug.Assert(!_states.ContainsKey(state.Name), $"A state with the same name ({state.Name}) has already existed!");
        _states[state.Name] = state;
        state.OnAdded(this);
    }

    /// <summary> Sets the state for the next tick. </summary>
    public void SetUpcomingState(string stateName)
    {
        Debug.Assert(_states.ContainsKey(stateName), $"The FSM doesn't contain the state {stateName}!");
        _upcomingState = _states[stateName];
    }

    /// <summary> Returns the name of the upcoming state. </summary>
    public string GetUpcomingStateName()
    {
        if (_upcomingState == null)
            return string.Empty;

        return _upcomingState.Name;
    }

    public void Tick(float deltaTime)
    {
        // Handle state switch.
        if (_upcomingState != null)
        {
            _currentState?.OnExit(this);
            _currentState = _upcomingState;
            _currentState?.OnEnter(this);

            _upcomingState = null;
        }

        // Execute the state.
        _currentState?.OnTick(this, deltaTime);
    }
}
