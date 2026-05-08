using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float _max = 100f;
    [SerializeField] private float _current = 100f;

    public void AddHealth(float amount)
    {
        _current = Mathf.Clamp(_current + amount, 0f, _max);
    }
}

