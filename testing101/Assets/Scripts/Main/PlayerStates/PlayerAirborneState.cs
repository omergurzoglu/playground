
using UnityEngine;

public class PlayerAirborneState : PlayerMovementState
{
    public PlayerAirborneState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        ResetSprintState();
    }

    protected override void OnContactWithGround(Collider collider)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.LightLandingState);
    }

    protected virtual void ResetSprintState()
    {
        _playerMovementSm.ReusableData.ShouldSprint = false; 
    }
}