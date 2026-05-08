using TMPro;
using UnityEngine;

/// <summary> Utility functions related to steering behaviors. </summary>
public static class SteeringBehaviorUtils
{
    public static Vector2 GetSeekVelocity(Vector2 currentPosition, Vector2 targetPosition, float speed)
    {
        return (targetPosition - currentPosition).normalized * speed;
    }

    public static Vector2 GetFleeVelocity(Vector2 currentPosition, Vector2 targtetToFleeFrom, float speed)
    {
        return (currentPosition - targtetToFleeFrom).normalized * speed;
    }

    public static Vector2 GetArriveVelocity(Vector2 currentPosition, Vector2 targetPosition, float speed, float slowDistance = 0.0f, float stoppingDistance = 0.0f)
    {
        Vector2 displacementToTarget = targetPosition - currentPosition;
        float distanceToTarget = displacementToTarget.magnitude;

        if (distanceToTarget < stoppingDistance)
            return Vector2.zero;

        Vector2 directionToTarget = displacementToTarget.normalized;
        float targetSpeed;

        if (distanceToTarget <= slowDistance)
        {
            float speedScale = distanceToTarget / slowDistance;
            targetSpeed = speed * speedScale;
            return directionToTarget * targetSpeed;
        }

        return directionToTarget * speed;
    }
}
