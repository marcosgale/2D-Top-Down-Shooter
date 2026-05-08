using UnityEngine;

public abstract class AIState
{
    public abstract string Name { get; }

    /// <summary> Called when added to the FSM. </summary>
    public virtual void OnAdded(FiniteStateMachine finiteStateMachine) {}

    /// <summary> Called when the FSM enters the state. </summary>
    public virtual void OnEnter(FiniteStateMachine finiteStateMachine) { }

    /// <summary> Called every frame when the state is current. </summary>
    public virtual void OnTick(FiniteStateMachine finiteStateMachine, float deltaTime) { }

    /// <summary> Calle when the FSM exits the state. </summary>
    public virtual void OnExit(FiniteStateMachine finiteStateMachine) { }
}
