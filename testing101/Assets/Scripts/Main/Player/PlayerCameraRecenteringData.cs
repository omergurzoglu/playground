
using System;
using UnityEngine;

[Serializable]

    public class PlayerCameraRecenteringData
    {
        [field:SerializeField][field:Range(0f,360f)]public float  MinimumAngle { get; private set; }
        [field:SerializeField][field:Range(0f,360f)]public float  MaximumAngle { get; private set; }
        [field:SerializeField][field:Range(-1f,200f)]public float  WaitTime { get; private set; }
        [field:SerializeField][field:Range(-1f,200f)]public float  RecenterTime { get; private set; }

        public bool IsWithinRange(float angle)
        {
            return angle >= MinimumAngle && angle <= MaximumAngle;
        }
    }
