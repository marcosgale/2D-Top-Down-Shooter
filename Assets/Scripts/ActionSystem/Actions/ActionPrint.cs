using UnityEngine;

public class ActionPrint : Action
{
    [SerializeField]
    private string _message = "Fire!";

    protected override void OnExecuted()
    {
        Debug.Log(_message);
    }
}
