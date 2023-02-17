using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollingState : PlayerLandingState
{
    private PlayerRollData _rollData;
    public PlayerRollingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _rollData = movementData.RollData;
    }

    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = _rollData.SpeedModifier;
        base.OnEnter();
        
        _playerMovementSm.ReusableData.ShouldSprint = false;
    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        if (_playerMovementSm.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }
        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.MediumStopState);
            return;
        }
        OnMove();
        
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        
    }
}