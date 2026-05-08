using UnityEngine;

public class AIState_Combat : AIState
{
    public override string Name => "Combat";

    private NPCMovement _movement;
    private AISight _aisight;
    private AIMemory _aiMemory;

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
        if (!PlayerCharacter.Instance)
            return;

        Transform chaseDestination = PlayerCharacter.Instance.transform;
        _movement.SetDestinationDynamic(chaseDestination);
        _movement.NPCRotationMode = NPCRotationMode.MoveDestination;

        _aisight.Radius = 10.0f;
    }

    public override void OnTick(FiniteStateMachine finiteStateMachine, float deltaTime)
    {
        if (!CanSeeEnemy())
        {
            finiteStateMachine.SetUpcomingState("Alert");
            return;
        }

        // Update the enemy's last seen location.
        if (PlayerCharacter.Instance)
            _aiMemory.EnemyLastSeenPosition = PlayerCharacter.Instance.transform.position;
    }

    public override void OnExit(FiniteStateMachine finiteStateMachine)
    {
        _movement.NPCRotationMode = NPCRotationMode.Velocity;
        // _movement.StopMoving();
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
