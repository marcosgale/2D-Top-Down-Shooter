using UnityEngine;

public class TestActionComponent : MonoBehaviour
{
    [SerializeField]
    private ActionComponent _actionComponent;

    private void OnValidate()
    {
        if (!_actionComponent)
            _actionComponent = GetComponent<ActionComponent>();
    }

    private void Start()
    {
        _actionComponent.Execute("Fire");
    }
}
