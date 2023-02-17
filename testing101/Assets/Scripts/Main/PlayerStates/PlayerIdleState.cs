
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private PlayerIdleData _idleData;
    public PlayerIdleState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _idleData = movementData.IdleData;
    }
    
    public override void OnEnter()
    {
        
        _playerMovementSm.ReusableData.MovementSpeedModifer = 0f;
        _playerMovementSm.ReusableData.BackwardsCameraRecenterData = _idleData.BackwardsCameraRecenterData;
        base.OnEnter();

        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;
        ResetVelocity();
    }

    public override void Tick()
    {
        base.Tick();
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        OnMove();
    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        if (!IsMovingHorizontally())
        {
            return;
        }
        ResetVelocity();
    }
}
