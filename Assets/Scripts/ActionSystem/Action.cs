using UnityEngine;

public abstract class Action : MonoBehaviour
{
    [SerializeField]
    private string _actionName;
    public string ActionName => _actionName;

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(_actionName))
            _actionName = name;
    }

    public void Execute()
    {
        if (CanBeExecuted())
            OnExecuted();
    }

    protected virtual bool CanBeExecuted()
    {
        return true;
    }

    protected virtual void OnExecuted()
    {

    }
}
