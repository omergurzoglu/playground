
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerMovingState
{
    private float _startTime;
    private PlayerSprintData _sprintData;
    public PlayerRunningState(PlayerMovementSM sm) : base(sm)
    {
        _sprintData = movementData.SprintData;
    }

    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = movementData.RunData.SpeedModifer;
        base.OnEnter();
        
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
       
        _startTime = Time.time;

    }

    public override void Tick()
    {
        base.Tick();
        if (!_playerMovementSm.ReusableData.ShouldWalk)
        {
            return;
        }

        if (Time.time < _startTime + _sprintData.RunToWalkTime)
        {
            return;
        }

        StopRunning();

    }
    protected override void OnMovementCancelled(InputAction.CallbackContext context)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.MediumStopState);
        base.OnMovementCancelled(context);
    }

   

    private void StopRunning()
    {
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.WalkingState);
       
    }

    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        _playerMovementSm.ChangeState(_playerMovementSm.WalkingState);
    }
    

    
}