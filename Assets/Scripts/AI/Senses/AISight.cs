using System.Collections.Generic;
using UnityEngine;

public class AISight : MonoBehaviour
{
    [SerializeField]
    private float _radius = 5.0f;
    public float Radius
    {
        get => _radius;
        set => _radius = value < 0.0f ? 0.0f : value;
    }

    [SerializeField, Tooltip("The layermask used in scanning colliders around the AI sights.")]
    private LayerMask _scanLayerMask = -1;

    [SerializeField, Tooltip("The layermask used in wall check. Any colliders that match this layermask will block the raycasts during wall check..")]
    private LayerMask _wallCheckLayerMask = -1;

#if UNITY_EDITOR
    [SerializeField]
    private bool _visualizeRadius = true;

    [SerializeField]
    private bool _visualizeScanResults = false;

    [SerializeField]
    private bool _visualizeVisibilityResults = true;
#endif

    private List<Collider2D> _surroundingColliders;

    private List<Collider2D> _visibleColliders;
    public List<Collider2D> VisibleColliders => _visibleColliders;

    private RaycastHit2D[] _raycastBuffer;   // Buffer used for wall check raycasts.

    private void Awake()
    {
        _surroundingColliders = new List<Collider2D>(8);
        _visibleColliders = new List<Collider2D>(8);
        _raycastBuffer = new RaycastHit2D[8];
    }

    private void FixedUpdate()
    {
        //
        // Step 1: Scan the colliders around the AI sight.
        //

        // Set up the contact filter for the scanning process.
        ContactFilter2D scanContactFiler = new ContactFilter2D();
        scanContactFiler.SetLayerMask(_scanLayerMask);

        // Scan for colliders around the AI sight.
        _surroundingColliders.Clear();
        int surroundingCollidersCount = Physics2D.OverlapCircle(transform.position, _radius, scanContactFiler, _surroundingColliders);

        //
        // Step 2: Check the visibility of each collider.
        //

        ContactFilter2D raycastContactFilter = new ContactFilter2D();
        raycastContactFilter.SetLayerMask(_wallCheckLayerMask);

        _visibleColliders.Clear();
        for (int i = 0; i < surroundingCollidersCount; ++i)
        {
            // Set up the raycast.
            Vector3 displacementToCollider = _surroundingColliders[i].transform.position - transform.position;
            Vector2 displacementToCollider2D = displacementToCollider;
            float distanceToCollider = displacementToCollider2D.magnitude;
            Vector2 directionToCollider = displacementToCollider2D.normalized;

            // Perform the raycast.
            int raycastHitCount = Physics2D.Raycast(transform.position, directionToCollider, raycastContactFilter, _raycastBuffer);

            // Get the closest wall collider.
            Collider2D closestWallCollider = GetClosestCollider(transform, _raycastBuffer, raycastHitCount);

            // Check the result of the raycast.
            // If the ray hits the target collider or doesn't hit anything, that collider is considered visible.
            if (!closestWallCollider || closestWallCollider == _surroundingColliders[i])
                _visibleColliders.Add(_surroundingColliders[i]);
            else
                Debug.Log($"{name}: Raycast check for {_surroundingColliders[i].name} hits {closestWallCollider.name}.");
        }
    }

    public bool CanSeeObject(GameObject targetObject)
    {
        if (!targetObject)
            return false;

        for (int i = 0; i < _visibleColliders.Count; ++i)
        {
            if (!_visibleColliders[i])
                continue;

            if (_visibleColliders[i].transform == targetObject.transform || _visibleColliders[i].transform.IsChildOf(targetObject.transform))
                return true;
        }

        return false;
    }

    /// <summary> Returns the collider that is the closest to the origin. </summary>
    private Collider2D GetClosestCollider(Transform origin, RaycastHit2D[] hitResults, int hitResultCount)
    {
        Collider2D closestHitCollider = null;
        float closestHitColliderDistanceSq = float.MaxValue;

        for (int i = 0; i < hitResultCount; ++i)
        {
            // Skip self.
            if (hitResults[i].transform.IsChildOf(origin))
                continue;

            if (hitResults[i].distance < closestHitColliderDistanceSq)
            {
                closestHitCollider= hitResults[i].collider;
                closestHitColliderDistanceSq = hitResults[i].distance;
            }
        }

        return closestHitCollider;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Visualize the scan radius.
        if (_visualizeRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        // Visualize the scan results.
        if (_visualizeScanResults && Application.isPlaying && _surroundingColliders != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _surroundingColliders.Count; ++i)
            {
                if (_surroundingColliders[i])
                    Gizmos.DrawLine(transform.position, _surroundingColliders[i].transform.position);
            }
        }

        // Visualize the wall check results.
        if (_visualizeVisibilityResults && Application.isPlaying && _visibleColliders != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _visibleColliders.Count; ++i)
            {
                if (_visibleColliders[i])
                    Gizmos.DrawLine(transform.position, _visibleColliders[i].transform.position);
            }
        }
    }
#endif
}