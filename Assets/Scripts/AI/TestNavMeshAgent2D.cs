using UnityEngine;
using UnityEngine.AI;

public class TestNavMeshAgent2D : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private Transform _destination;

    private void OnValidate()
    {
        if(!_navMeshAgent)
            _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(_destination.position);
        transform.rotation = Quaternion.identity;
    }
}
