using System.Collections.Generic;
using UnityEngine;

public enum AINoiseFlag
{
    None = 0,
    GunShot = 1 << 0
}

public class AIHearing : MonoBehaviour
{
    public void OnHearNoise(Vector2 noiseSource, GameObject instigator, AINoiseFlag noiseFlags)
    {
    }

    public static void MakeNoise(Vector2 noiseSource, float radius, GameObject instigator, AINoiseFlag noiseFlags)
    {
        HashSet<AIHearing> aiHearings = new HashSet<AIHearing>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(noiseSource, radius);
        for (int i = 0; i < colliders.Length; ++i)
        {
            AIHearing aiHearing = colliders[i].GetComponentInParent<AIHearing>();
            if (!aiHearing || aiHearings.Contains(aiHearing))
                continue;

            aiHearing.OnHearNoise(noiseSource, instigator, noiseFlags);
        }
    }
}
