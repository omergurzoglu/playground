using System;
using UnityEngine;

[Serializable]
public class SlopeData
{
    [field: SerializeField] [field:Range(0f, 1f)] public float StepHeightPercentage { get; private set; } = 0.25f;

    [field: SerializeField] [field: Range(0f, 5f)] public float FloatRayDistance { get; private set; } = 2.0f;
    [field: SerializeField] [field: Range(0f, 50f)] public float StepReachForce { get; private set; } = 25.0f;

}
 