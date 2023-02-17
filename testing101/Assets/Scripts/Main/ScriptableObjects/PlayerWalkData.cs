using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerWalkData
{
    [field: SerializeField] [field: Range(0f, 1f)] public float SpeedModifer { get; private set; } = 0.225f; 
    [field:SerializeField]public List<PlayerCameraRecenteringData> BackwardsCameraRecenterData { get; private set; }
    
}
