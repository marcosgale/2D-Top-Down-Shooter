using UnityEngine;

public class AIState_Idle : AIState
{
    public override string Name => "Idle";

    private NPCMovement _movement;
    private AISight _aisight;

    public override void OnAdded(FiniteStateMachine finiteStateMachine)
    {
        _movement = finiteStateMachine.Owner.GetComponent<NPCMovement>();
        Debug.Assert(_movement);

        _aisight = finiteStateMachine.Owner.GetComponent<AISight>();
        Debug.Assert(_aisight);
    }

    public override void OnEnter(FiniteStateMachine finiteStateMachine)
    {
        _movement.StopMoving();
        _aisight.Radius = 5.0f;
    }

    public override void OnTick(FiniteStateMachine finiteStateMachine, float deltaTime)
    {
        if (CanSeeEnemy())
        {
            finiteStateMachine.SetUpcomingState("Combat");
            return;
        }
    }

    private bool CanSeeEnemy()
    {
        if (!PlayerCharacter.Instance || !PlayerCharacter.Instance.IsAlive)
            return false;

        for (int i = 0; i < _aisight.VisibleColliders.Count; ++i)
        {
            if (!_aisight.VisibleColliders[i])
                continue;

            if (_aisight.VisibleColliders[i].transform.IsChildOf(PlayerCharacter.Instance.transform))
                return true;
        }

        return false;
    }
}
