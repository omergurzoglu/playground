
public class PlayerMediumStopState : PlayerStopState
{
    public PlayerMediumStopState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _playerMovementSm.ReusableData.MovementDecelerationForce = movementData.StopData.MediumDecelerationForce;
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
        
    }
}
