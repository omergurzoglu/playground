
public class PlayerLightStopState : PlayerStopState
{
    public PlayerLightStopState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerMovementSm.ReusableData.MovementDecelerationForce = movementData.StopData.LightDecelerationForce;
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;

    }
}
