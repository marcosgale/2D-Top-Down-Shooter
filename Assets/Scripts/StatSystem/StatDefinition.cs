using UnityEngine;

[CreateAssetMenu(fileName = "NewStatDefinition", menuName = "Stat System/Stat Definition")]
public class StatDefinition : ScriptableObject
{
    [SerializeField]
    private string _displayName;
    public string DisplayName => _displayName;

    [SerializeField]
    private string _description;
}
