using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpingState: PlayerAirborneState
{
    private bool _shouldKeepRotating;
    private PlayerJumpData _jumpData;
    private bool _canStartFalling;
    public PlayerJumpingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _jumpData = airborneData.JumpData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerMovementSm.ReusableData.MovementSpeedModifer = 0f; 
        _playerMovementSm.ReusableData.MovementDecelerationForce = _jumpData.DecelerationForce;
        _playerMovementSm.ReusableData.RotationData = _jumpData.RotationData;
        
        _shouldKeepRotating = _playerMovementSm.ReusableData.MovementInput != Vector2.zero;
        Jump();
    }

    public override void Tick()
    {
        base.Tick();
        if (!_canStartFalling&&IsMovingUp(0f))
        {
            _canStartFalling = true;
        }
        if (!_canStartFalling||IsMovingUp(0f))
        {
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.FallingState);
    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        if (_shouldKeepRotating)
        {
            RotateTowardsTargetRotation();
        }

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        SetBaseRotationData();
        _canStartFalling = false;
    }

    protected override void ResetSprintState()
    {
        
    }

    private void Jump()
    {
        Vector3 jumpForce = _playerMovementSm.ReusableData.CurrentJumpForce;
        Vector3 jumpDirection = _playerMovementSm.Player.transform.forward;
        if (_shouldKeepRotating)
        {
            UpdateTargetRotation(GetMovementInputDirection());
            jumpDirection = GetTargetRotationDirection(_playerMovementSm.ReusableData.CurrentTargetRotation.y);
        }
        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;

        Vector3 capsuleColliderCenterInWorldSpace = _playerMovementSm.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,_jumpData.JumpToGroundRayDistance, _playerMovementSm.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);
            if (IsMovingUp())
            {
                float forceModifier = _jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);
                jumpForce.x *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            if (IsMovingDown())
            {
                float forceModifier = _jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);
                jumpForce.y *= forceModifier;
            }
            
            
        }
        ResetVelocity();
        _playerMovementSm.Player._rigidbody.AddForce(jumpForce,ForceMode.VelocityChange);
    }

    protected override void OnMovementCancelled(InputAction.CallbackContext context)
    {
        
    }
}