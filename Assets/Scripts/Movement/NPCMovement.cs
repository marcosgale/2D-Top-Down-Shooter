using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum NPCRotationMode
{
    /// <summary> No rotation</summary>
    None = 0,

    /// <summary> The NPC rotates in its move direction. </summary>
    Velocity = 1,

    /// <summary> The NPC looks at the move destination. </summary>
    MoveDestination = 2
}

public class NPCMovement : CharacterMovement
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private float _slowDistance = 1.0f;

    [SerializeField]
    private float _stoppingDistance = 0.2f;

    [SerializeField]
    private NPCRotationMode _npcRotationMode = NPCRotationMode.Velocity;
    public NPCRotationMode NPCRotationMode
    {
        get => _npcRotationMode;
        set => _npcRotationMode = value;
    }

    /// <summary> The position where the NPC wants to go to. </summary>
    private Vector2 _moveDestination;

    /// <summary> 
    /// The dynamic position where the NPC wants to go to. </br>
    /// If set, _moveDestination is updated every frame. 
    /// </summary>
    private Transform _dynamicMoveDestination;

    /// <summary> 
    /// Should the character move or not? <br/> 
    /// Doesn't take external factors into consideration. 
    /// </summary>
    private bool _shouldMove = false;

    /// <summary> The velocity the NPC wishes to achieve. Updated in UpdatePosition(). </summary>
    private Vector2 _desiredVelocity;

    /// <summary> The desired rotation the NPC wishes to achieve. Updated in UpdateRotation(). </summary>
    private float _desiredRotation;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!_navMeshAgent)
            _navMeshAgent = GetComponent<NavMeshAgent>();

        if (_navMeshAgent)
        {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.speed = BaseMoveSpeed;
        }
    }

    protected virtual void Start()
    {
        // Do not make the navmesh agent update the position and rotation.
        // We only want to get the desired velocity so that this script can handle movement.
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.updatePosition = false;
    }

    protected override void UpdatePosition()
    {
        // Synchronize the navmesh agent's position with the rigidbody's position.
        _navMeshAgent.nextPosition = RigidBody.position;

        // Update the position the NPC needs to move to if the destination is dynamic.
        if (_dynamicMoveDestination)
        {
            _moveDestination = _dynamicMoveDestination.position;

            _navMeshAgent.SetDestination(_moveDestination);
            _navMeshAgent.isStopped = false;
        }

        // Calculate the desired move velocity.
        if (!_shouldMove)
            _desiredVelocity = Vector2.zero;
        else
        {
            //_desiredVelocity = SteeringBehaviorUtils.GetArriveVelocity(RigidBody.position, _moveDestination, BaseMoveSpeed, _slowDistance, _stoppingDistance);

            // If the agent is close to the target, stop moving.
            Vector2 displacementToTarget = _moveDestination - RigidBody.position;
            float distanceToTarget = displacementToTarget.magnitude;

            if (distanceToTarget <= _stoppingDistance)
                _desiredVelocity = Vector2.zero;

            else
                _desiredVelocity = _navMeshAgent.desiredVelocity;
        }
        
        // Update the rigidbody's velocity.
        RigidBody.linearVelocity = Vector2.MoveTowards(RigidBody.linearVelocity, _desiredVelocity, MoveAcceleration * Time.deltaTime);
    }

    protected override void UpdateRotation()
    {
        if (_npcRotationMode == NPCRotationMode.None)
            return;

        // Calculate the desired rotation.
        _desiredRotation = RigidBody.rotation;
        if (_npcRotationMode == NPCRotationMode.Velocity)
        {
            if (_desiredVelocity.sqrMagnitude >= Mathf.Epsilon)
                _desiredRotation = Mathf.Atan2(_desiredVelocity.y, _desiredVelocity.x) * Mathf.Rad2Deg;
        }
        else if (_npcRotationMode == NPCRotationMode.MoveDestination)
        {
            Vector2 displacementToDestination = _moveDestination - new Vector2(transform.position.x, transform.position.y);
            if (displacementToDestination.sqrMagnitude >= Mathf.Epsilon)
                _desiredRotation = Mathf.Atan2(displacementToDestination.y, displacementToDestination.x) * Mathf.Rad2Deg;
        }

        // Rotate towards the desired rotation.
        RigidBody.rotation = Mathf.MoveTowardsAngle(RigidBody.rotation, _desiredRotation, RotateAcceleration * Time.deltaTime);
    }

    public void SetDestinationStatic(Vector3 destination)
    {
        _moveDestination = destination;
        _dynamicMoveDestination = null;
        _shouldMove = true;

        _navMeshAgent.SetDestination(destination);
        _navMeshAgent.isStopped = false;
    }

    public void SetDestinationDynamic(Transform destination)
    {
        if (destination)
        {
            _dynamicMoveDestination = destination;
            _shouldMove = true;

            _navMeshAgent.SetDestination(destination.position);
            _navMeshAgent.isStopped = false;
        }
        else
        {
            _dynamicMoveDestination = null;
            _shouldMove = false;

            _navMeshAgent.SetDestination(transform.position);
            _navMeshAgent.isStopped = true;
        }
    }

    public void StopMoving()
    {
        _shouldMove = false;

        _navMeshAgent.isStopped = true;
    }
}
