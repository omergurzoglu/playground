
using System;
using Cinemachine;
using UnityEngine;

[Serializable]
    public class PlayerCameraUtility
    {
        [field:SerializeField]public CinemachineVirtualCamera VirtualCamera { get; private set; }
        [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
        [field: SerializeField] public float DefaultHorizontalRecenterTime { get; private set; } = 4f;
        private CinemachinePOV _cinemachinePov;

        public void Initialize()
        {
            _cinemachinePov = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }

        public void EnableRecenter(float waitTime = -1f, float recenterTime = -1f,float baseMovementSpeed=1f,float movementSpeed=1f)
        {
            _cinemachinePov.m_HorizontalRecentering.m_enabled = true;
            _cinemachinePov.m_HorizontalRecentering.CancelRecentering();
            if (waitTime == -1f)
            {
                waitTime = DefaultHorizontalWaitTime;
            }

            if (recenterTime == -1f)
            {
                recenterTime = DefaultHorizontalRecenterTime;
            }

            recenterTime = recenterTime * baseMovementSpeed / movementSpeed;

            _cinemachinePov.m_HorizontalRecentering.m_WaitTime = waitTime;
            _cinemachinePov.m_HorizontalRecentering.m_RecenteringTime = recenterTime;

        }

        public void DisableRecenter()
        {
            _cinemachinePov.m_HorizontalRecentering.m_enabled = false;
        }

    }
