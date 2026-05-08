using UnityEngine;

public class AIBrain_MeleeEnemy : AIBrain
{
    public override void InitFiniteStateMachine(ref FiniteStateMachine finiteStateMachine)
    {
        finiteStateMachine.AddState(new AIState_Idle());
        finiteStateMachine.AddState(new AIState_Alert());
        finiteStateMachine.AddState(new AIState_Combat());
        finiteStateMachine.SetUpcomingState("Idle");
    }
}
