
using UnityEngine.InputSystem;

public class PlayerStopState : PlayerGroundedState
{
    public PlayerStopState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        
    }

    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = 0f;
        SetBaseCameraRecenterData();
        base.OnEnter();
        
    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        RotateTowardsTargetRotation();
        if (!IsMovingHorizontally())
        {
            return;
        }
        DecelerateHorizontally();
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.started += OnMovementStarted;
    }

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
       OnMove();
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.started -= OnMovementStarted;
    }

    public override void OnAnimationTransitionEvent()
    {
        _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
    }

    
}