
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprintingState : PlayerMovingState
{

    private PlayerSprintData _sprintData;
    private bool _keepSprinting;
    private float _startTime;
    private bool _shouldResetSprintState;
    public PlayerSprintingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _sprintData = movementData.SprintData;
    }

    public override void OnEnter()
    {
        _playerMovementSm.ReusableData.MovementSpeedModifer = _sprintData.SpeedModifier;
        base.OnEnter();
        
        _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
        
        _startTime = Time.time;
        _shouldResetSprintState = true;
        if (!_playerMovementSm.ReusableData.ShouldSprint)
        {
            _keepSprinting = false;
        }   
        
    }

    public override void Tick()
    {
        base.Tick();
        if (_keepSprinting)
        {
            return;
        }

        if (Time.time < _startTime + _sprintData.SprintToRunTime)
        {
            return;
        }

        StopSprinting();
    }

    public override void OnExit()
    {
        base.OnExit();
        if (_shouldResetSprintState)
        {
            _keepSprinting = false;
            _playerMovementSm.ReusableData.ShouldSprint = false;
        }
    }

    private void StopSprinting()
    {
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.RunningState);    
    }
    protected override void OnMovementCancelled(InputAction.CallbackContext context)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.HardStopState);
        base.OnMovementCancelled(context);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        _shouldResetSprintState = false;
        base.OnJumpStarted(context);
        
    }

    protected override void OnFall()
    {
        _shouldResetSprintState = false;
        
        base.OnFall();
        
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _keepSprinting = true;
        _playerMovementSm.ReusableData.ShouldSprint = true; 
    }

    protected override void RemoveInputActionsCallbacks()
    {
        _playerMovementSm.Player.playerInput.PlayerActions.Sprint.performed -= OnSprintPerformed;
    }
}