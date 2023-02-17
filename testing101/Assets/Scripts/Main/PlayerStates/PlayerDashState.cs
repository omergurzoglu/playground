
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerGroundedState
{
    protected PlayerDashData _dashData;

    private float _startTime;
    private int _consecutiveDashesUsed;
    private bool _shouldKeepRotating;

    public PlayerDashState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _dashData = movementData.DashData;
    } 

    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = _dashData.SpeedModifier;
        base.OnEnter();
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
        _playerMovementSm.ReusableData.RotationData = _dashData.RotationData;
        
        Dash();
        _shouldKeepRotating = _playerMovementSm.ReusableData.MovementInput != Vector2.zero;
        UpdateConsecutiveDashes();
        _startTime = Time.time;
    }

    public override void OnExit()
    {
        base.OnExit();
        SetBaseRotationData();
    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
       
    }

   
    public override void PhysicsTick()
    {
        base.PhysicsTick();
        if (!_shouldKeepRotating)
        {
            return;
        }
        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.HardStopState);
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.SprintingState);
        
    }

    private void UpdateConsecutiveDashes()
    {
        if (!IsConsecutive())
        {
            _consecutiveDashesUsed = 0;
        }

        ++_consecutiveDashesUsed;

        if (_consecutiveDashesUsed == _dashData.ConsecutiveDashLimitAmount)
        {
            _consecutiveDashesUsed = 0;
            _playerMovementSm.Player.playerInput.DisableActionFor(_playerMovementSm.Player.playerInput.PlayerActions.Dash,_dashData.DashLimitReachedCoolDown);
            
        }
    }

    private bool IsConsecutive()
    {
        return Time.time < _startTime + _dashData.TimeToBeConsideredConsecutive;
    }

    private void Dash()
    {
        Vector3 dashDirection = _playerMovementSm.Player.transform.forward;

        dashDirection.y = 0f;
        UpdateTargetRotation(dashDirection, false);
        if (_playerMovementSm.ReusableData.MovementInput != Vector2.zero)
        {
            UpdateTargetRotation(GetMovementInputDirection());
            dashDirection = GetTargetRotationDirection(_playerMovementSm.ReusableData.CurrentTargetRotation.y);
        }

        
        _playerMovementSm.Player._rigidbody.velocity = dashDirection * GetMovementSpeed(false);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    protected override void OnMovementPerformed(InputAction.CallbackContext context)
    {
        base.OnMovementPerformed(context);
        _shouldKeepRotating = true;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.performed -= OnMovementPerformed;
    }
}
