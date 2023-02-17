
using System;
using UnityEngine;

[Serializable]
public class PlayerSprintData
{
    [field: SerializeField] [field: Range(1f, 3f)] public float SpeedModifier { get; private set; } = 1.7f;
    [field: SerializeField] [field: Range(0f, 5f)] public float SprintToRunTime { get; private set; } = 1.0f;
    [field: SerializeField] [field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;

}
