using UnityEngine;

[System.Serializable]
public class StatInstance
{
    [SerializeField]
    private StatDefinition _definition;
    public StatDefinition Definition => _definition;

   

    [SerializeField]
    private float _valueFloat;

    public float ValueFloat
    {
        get => _valueFloat;
        set => _valueFloat = value;
    }


    [SerializeField]
    private string _valueString;
    public string ValueString => _valueString;
}
