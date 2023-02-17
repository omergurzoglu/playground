using UnityEngine;

public class PlayerFallingState : PlayerAirborneState
{
    private PlayerFallData _playerFallData;
    private Vector3 _playerPositionOnEnter;
    public PlayerFallingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _playerFallData = airborneData.FallData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerMovementSm.ReusableData.MovementSpeedModifer = 0f;
        _playerPositionOnEnter = _playerMovementSm.Player.transform.position;
        ResetVerticalVelocity();

    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        LimitVerticalVelocity();
    }

    protected override void ResetSprintState()
    {
        
        
    }

    protected override void OnContactWithGround(Collider collider)
    {
        float fallDistance = _playerPositionOnEnter.y - _playerMovementSm.Player.transform.position.y;
        if (fallDistance < _playerFallData.MinimumDistanceToBeConsideredHardFall)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.LightLandingState);
            return;
        }

        if (_playerMovementSm.ReusableData.ShouldWalk&& !_playerMovementSm.ReusableData.ShouldSprint|| _playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.HardLandingState);
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.RollingState);
            
    }

    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
        if (playerVerticalVelocity.y >= -_playerFallData.FallSpeedLimit)
        {
            return;
        }

        Vector3 limitedVelocity = new Vector3(0f, -_playerFallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);
        _playerMovementSm.Player._rigidbody.AddForce(limitedVelocity,ForceMode.VelocityChange);
        
        
    }
}