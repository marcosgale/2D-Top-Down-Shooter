using UnityEngine;

/// <summary> Alert: The character is aware of the enemy, but not sure where the enemy is. </summary>
public class AIState_Alert : AIState
{
    private const float MAX_ALERT_REMAINING_TIME = 5.0f;

    public override string Name => "Alert";

    private NPCMovement _movement;
    private AISight _aisight;
    private AIMemory _aiMemory;

    private float _alertRemainingTime;

    public override void OnAdded(FiniteStateMachine finiteStateMachine)
    {
        _movement = finiteStateMachine.Owner.GetComponent<NPCMovement>();
        Debug.Assert(_movement);

        _aisight = finiteStateMachine.Owner.GetComponent<AISight>();
        Debug.Assert(_aisight);

        _aiMemory = finiteStateMachine.Owner.GetComponent<AIMemory>();
        Debug.Assert(_aiMemory);
    }

    public override void OnEnter(FiniteStateMachine finiteStateMachine)
    {
        // Reset the alert's remaining time.
        _alertRemainingTime = MAX_ALERT_REMAINING_TIME;

        // Go to the enemy's last seen location.
        _movement.SetDestinationStatic(_aiMemory.EnemyLastSeenPosition);
    }

    public override void OnTick(FiniteStateMachine finiteStateMachine, float deltaTime)
    {
        if (CanSeeEnemy())
        {
            finiteStateMachine.SetUpcomingState("Combat");
            return;
        }

        _alertRemainingTime -= deltaTime;
        if (_alertRemainingTime <= 0.0f)
        {
            finiteStateMachine.SetUpcomingState("Idle");
            return;
        }
    }

    public override void OnExit(FiniteStateMachine finiteStateMachine)
    {
        if (finiteStateMachine.GetUpcomingStateName() == "Idle")
            _movement.StopMoving();
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
