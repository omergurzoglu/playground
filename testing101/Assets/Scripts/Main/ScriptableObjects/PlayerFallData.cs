
using System;
using UnityEngine;

[Serializable]
    public class PlayerFallData
    {
        [field: SerializeField]
        [field: Range(0f, 10f)]
        public float FallSpeedLimit { get; private set; } = 10f;
        [field: SerializeField]
        [field: Range(0f, 100f)]
        public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3f;

    }
