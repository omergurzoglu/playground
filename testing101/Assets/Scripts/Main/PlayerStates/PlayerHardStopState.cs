public class PlayerHardStopState : PlayerStopState
{
    public PlayerHardStopState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _playerMovementSm.ReusableData.MovementDecelerationForce = movementData.StopData.HardDecelerationForce;
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
        
    }

    protected override void OnMove()
    {
        if (_playerMovementSm.ReusableData.ShouldWalk)
        {
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.RunningState);
        
    }
}
