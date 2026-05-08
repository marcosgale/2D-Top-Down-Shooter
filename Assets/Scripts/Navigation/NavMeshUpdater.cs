using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// Updates the NavMesh at runtime when obstacles change
/// </summary>
public class NavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface _navMeshSurface;

    private void Start()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshUpdater requires a NavMeshSurface component!");
        }
    }

    /// <summary>
    /// Call this method when obstacles are added or removed
    /// </summary>
    public void UpdateNavMesh()
    {
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh updated!");
        }
    }

    /// <summary>
    /// Automatically update NavMesh every X seconds (optional, can be expensive)
    /// </summary>
    public void StartAutoUpdate(float interval = 1f)
    {
        InvokeRepeating(nameof(UpdateNavMesh), interval, interval);
    }

    public void StopAutoUpdate()
    {
        CancelInvoke(nameof(UpdateNavMesh));
    }
}
