using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Fleeing,
    Patrolling,
    Searching,
    Stuck,
}

public class enemymovement : MonoBehaviour
{
    public Transform _Wall;
    public Transform _Player;
    private float _moveSpeed = 3f;
    private EnemyState _CurrentState;
    private float _distanceToPlayer;
    private float _chaseRange = 10f;
    private float _attackRange = 2f;
    private float _lineofSight = 5f;
    private float _distanceToObstacle;
    private float _obstacleCheckCircleRadius;
    private float _obstacleCheckDistance;

    public LayerMask _obstacleLayerMask;
    private RaycastHit2D[] _obstacleCollisions;
    private object hit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Holds 5 previous collisions
        _obstacleCollisions = new RaycastHit2D[5]; 
    }
    void HandleObstacles()
    {
        // Set the position of the circle to be in front of the enemy
        var _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_obstacleLayerMask);
        _contactFilter.useTriggers = false;

        int _numberOfCollisions = Physics2D.CircleCast(transform.position, 0.5f, transform.up, _contactFilter, _obstacleCollisions, 0.1f);

        for (int i = 0; i < _numberOfCollisions; i++)
        {
            var _obstacleCollision = _obstacleCollisions[i];
            if (_CurrentState == EnemyState.Stuck)
            {
                // Get the normal of the obstacle surface
                Vector3 _obstacleNormal = _obstacleCollision.normal;
                // Reflect the enemy's movement direction off the obstacle normal
                Vector3 _newDirection = Vector3.Reflect(transform.up, _obstacleNormal).normalized;
                // Set the enemy's rotation to face the new direction
                float _angle = Mathf.Atan2(_newDirection.y, _newDirection.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, _angle);
                // Move the enemy in the new direction
                transform.position += _newDirection * _moveSpeed * Time.deltaTime;
                _CurrentState = EnemyState.Chasing;
                break; // Only handle the first collision for this frame
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        _distanceToObstacle = Vector3.Distance(transform.position, _Wall.position);
        _distanceToPlayer = Vector3.Distance(transform.position, _Player.position);

        if (_distanceToObstacle <= 1f)
        {
            _CurrentState = EnemyState.Stuck;
        }
        else if (_distanceToPlayer > _chaseRange)
        {
            _CurrentState = EnemyState.Idle;
        }
        else if (_distanceToPlayer <= _attackRange)
        {
            _CurrentState = EnemyState.Attacking;
            
        }
        else if (_distanceToPlayer <= _lineofSight)
        {
            _CurrentState = EnemyState.Chasing;
        }
        else
        {
            _CurrentState = EnemyState.Patrolling;
        }

        switch (_CurrentState)
        {
            case EnemyState.Stuck:
                HandleObstacles();
                break;
            case EnemyState.Idle:
                // Do nothing
                break;
            case EnemyState.Chasing:
                Vector3 direction = _Player.position - transform.position;
                direction.Normalize();
                transform.position += direction * _moveSpeed * Time.deltaTime;
                // Chase logic here
                break;
            case EnemyState.Attacking:
                // Attack logic here
                break;
            case EnemyState.Fleeing:
                // Flee logic here
                break;
            case EnemyState.Patrolling:
                // Patrol logic here
                break;
            case EnemyState.Searching:
                // Search logic here
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _lineofSight);
    }
}
