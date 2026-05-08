using UnityEngine;

public class AIBrain : MonoBehaviour
{
    [SerializeField]
    private AISight _aiSight;
    public AISight AISight => _aiSight;

    private FiniteStateMachine _finiteStateMachine;

    protected virtual void OnValidate()
    {
        if (!_aiSight)
            _aiSight = GetComponent<AISight>();
    }

    private void Awake()
    {
        _finiteStateMachine = new FiniteStateMachine();
        _finiteStateMachine.Owner = gameObject;
    }

    private void Start()
    {
        InitFiniteStateMachine(ref _finiteStateMachine);
    }

    private void Update()
    {
        _finiteStateMachine.Tick(Time.deltaTime);
    }

    /// <summary> 
    /// Override this function to set up how the finite state machine is initialized:
    /// + Use AddState() to add more states to the FSM.
    /// + Use SetUpcomingState() to set the initial state.
    /// </summary>
    public virtual void InitFiniteStateMachine(ref FiniteStateMachine finiteStateMachine)
    {

    }
}
