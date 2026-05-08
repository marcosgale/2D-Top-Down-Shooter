using System.Collections.Generic;
using UnityEngine;

public class ActionComponent : MonoBehaviour
{
    [SerializeField]
    private List<Action> _initialActions;

    private Dictionary<string, Action> _actions;

    private void Awake()
    {
        _actions = new Dictionary<string, Action>();

        foreach (var action in _initialActions) 
        {
            _actions[action.ActionName] = action;
        }
    }

    public void Execute(string actionName)
    {
        _actions.TryGetValue(actionName, out Action action);
        if (!action)
        {
            Debug.LogError($"Cannot execute action with name \"{actionName}\"!");
            return;
        }

        action.Execute();
    }
}
