using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerMovingState
{
    private PlayerWalkData _walkData;
    public PlayerWalkingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _walkData = movementData.WalkData;
    }
    
    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = _walkData.SpeedModifer;
        _playerMovementSm.ReusableData.BackwardsCameraRecenterData = _walkData.BackwardsCameraRecenterData;
        base.OnEnter();
        
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
    }

    public override void OnExit()
    {
        base.OnExit();
        SetBaseCameraRecenterData();
    }


    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        _playerMovementSm.ChangeState(_playerMovementSm.RunningState);
    }

    protected override void OnMovementCancelled(InputAction.CallbackContext context)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.LightStopState);
        base.OnMovementCancelled(context);
    }
}