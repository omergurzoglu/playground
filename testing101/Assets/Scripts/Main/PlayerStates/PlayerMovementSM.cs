
using UnityEngine;

public class PlayerMovementSM : StateMachine
{
    public Player Player { get; }
    public PlayerStateReusableData ReusableData { get; }
    public PlayerRunningState RunningState { get; }
    public PlayerIdleState IdleState { get; }
    public PlayerWalkingState WalkingState { get; }
    public PlayerDashState DashState { get; }
    public PlayerSprintingState SprintingState { get; }
    public PlayerStopState LightStopState { get; }
    public PlayerMediumStopState MediumStopState { get; }
    public PlayerHardStopState HardStopState { get; }
    public PlayerJumpingState JumpingState { get; }
    public PlayerFallingState FallingState { get; }
    public PlayerLightLandingState LightLandingState { get; }
    public PlayerRollingState RollingState { get; }
    public PlayerHardLandingState HardLandingState { get; }

    
    public PlayerMovementSM(Player player)
    {
        Player = player;
        ReusableData = new PlayerStateReusableData();
        IdleState = new PlayerIdleState(this);
        DashState = new PlayerDashState(this);
        WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        SprintingState = new PlayerSprintingState(this);
        LightStopState = new PlayerStopState(this);
        MediumStopState = new PlayerMediumStopState(this);
        HardStopState = new PlayerHardStopState(this);
        LightLandingState = new PlayerLightLandingState(this);
        RollingState = new PlayerRollingState(this);
        HardLandingState = new PlayerHardLandingState(this);
        JumpingState = new PlayerJumpingState(this);
        FallingState = new PlayerFallingState(this);
    }


}