
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateReusableData 
{
    public Vector2 MovementInput { get; set; }
    public float MovementSpeedModifer { get; set; } = 1f;
    
    public float OnSlopesMovementSpeedModifer { get; set; } = 1f;
    public float MovementDecelerationForce { get; set; } = 1f;
    public bool ShouldWalk { get; set; }
    public bool ShouldSprint { get; set; }
    public List<PlayerCameraRecenteringData> BackwardsCameraRecenterData { get; set; }
    public List<PlayerCameraRecenteringData> SidewaysCameraRecenterData { get; set; }

    private Vector3 _currentTargetRotation;
    private Vector3 _timeToReachTargetRotation;
    private Vector3 _dampedTargetRotationCurrentVelocity;
    private Vector3 _dampedTargetRotationPassedTime; 
    

    public ref Vector3 CurrentTargetRotation
    {
        get { return ref _currentTargetRotation; }
    }

    public ref Vector3 TimeToReachTargetRotation
    {
        get { return ref _currentTargetRotation; }
    }

    public ref Vector3 DampedTargetRotationCurrentVelocity
    {
        get { return ref _currentTargetRotation; }
    }

    public ref Vector3 DampedTargetRotationPassedTime
    {
        get { return ref _currentTargetRotation; }
    }
    
    public Vector3 CurrentJumpForce { get; set; }

    public PlayerRotationData RotationData { get; set; }
}
