using System.Collections.Generic;
using UnityEngine;

public class StatComponent : MonoBehaviour 
{
    private Dictionary<StatDefinition, StatInstance> stats = new Dictionary<StatDefinition, StatInstance>();

    [SerializeField]
    private List<StatInstance> _initialStats;

    private void Awake()
    {
        foreach (var stat in _initialStats)
        {
            stats[stat.Definition] = stat;
        }
    }

    public List<StatDefinition> GetStatDefinitions()
    {
        List<StatDefinition> statDefinitions = new List<StatDefinition>(stats.Count);

        foreach (var stat in stats)
            statDefinitions.Add(stat.Key);

        return statDefinitions;
    }

    public float GetValue(StatDefinition definition)
    {
        return stats[definition].ValueFloat;
    }

      public float GetValue(string displayName)
    {
        foreach (var stat in stats)
        {
            if (stat.Key.DisplayName == displayName)
            {
                return stat.Value.ValueFloat;
            }
        }

        Debug.LogError($"Stat {displayName} not found!");
        return 0f;
    }

    public bool TryGetValue(StatDefinition definition, out float value)
    {
        if (stats.TryGetValue(definition, out StatInstance statInstance))
        {
            value = 0;
            return false;
        }

        value = statInstance.ValueFloat;
        return true;
    }

    public bool TryGetValue(string displayName, out float value)
    {
        foreach (var stat in stats)
        {
            if (stat.Key.DisplayName == displayName)
            {
                value = stat.Value.ValueFloat;
                return true;
            }
        }

        value = 0;
        return false;
    }

    public void SetValue(StatDefinition definition, float value)
    {
        stats[definition].ValueFloat = value;
    }

    public void SetValue(string displayName, float value)
    {
        foreach (var stat in stats)
        {
            if (stat.Key.DisplayName == displayName)
            {
                stat.Value.ValueFloat = value;
                return;
            }
        }
        Debug.LogError($"Stat {displayName} not found!");
    }

}
