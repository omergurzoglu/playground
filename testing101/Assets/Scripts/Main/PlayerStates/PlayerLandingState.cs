public class PlayerLandingState : PlayerGroundedState
{
    public PlayerLandingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        DisableCameraRecenter();
    }
}