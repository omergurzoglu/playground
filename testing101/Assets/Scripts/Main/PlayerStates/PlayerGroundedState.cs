using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerMovementState
{

    private SlopeData _slopeData;
    public PlayerGroundedState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
    {
        _slopeData = _playerMovementSm.Player.CapsuleColliderUtility.SlopeData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        UpdateShouldSprintState();
        UpdateCameraRecenterState(_playerMovementSm.ReusableData.MovementInput);
    }

   

    private void UpdateShouldSprintState()
    {
        if (!_playerMovementSm.ReusableData.ShouldSprint)
        {
            return;
        }

        if (_playerMovementSm.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        _playerMovementSm.ReusableData.ShouldSprint = false;
    }

    public override void PhysicsTick()
    {
        base.PhysicsTick();
        FloatingCapsule();
    }

    protected override void OnContactWithGroundExited(Collider collider)
    {
        //base.OnContactWithGroundExited(collider);
        if (IsThereGroundUnderneath())
        {
            return;
        }
        Vector3 capsuleColliderCenterInWorldSpace = _playerMovementSm.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace-_playerMovementSm.Player.CapsuleColliderUtility.CapsuleColliderData.ColliderVerticalExtends, Vector3.down);
        if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _,movementData.GroundToFallRayDistance,_playerMovementSm.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
        {
            OnFall();
        }
       
    }

    private bool IsThereGroundUnderneath()
    {
        BoxCollider groundCheckCollider = _playerMovementSm.Player.CapsuleColliderUtility.TriggerColliderData.GroundCheckCollider;
        Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
        Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, _playerMovementSm.Player.CapsuleColliderUtility.TriggerColliderData.GroundCheckColliderExtends, groundCheckCollider.transform.rotation,_playerMovementSm.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore);
        return overlappedGroundColliders.Length > 0;
    }


    protected virtual void OnFall()
    {
        _playerMovementSm.ChangeState(_playerMovementSm.FallingState);
    }

    private void FloatingCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace = _playerMovementSm.Player.CapsuleColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRay = new Ray(capsuleColliderCenterInWorldSpace,Vector3.down);
        if (Physics.Raycast(downwardsRay, out RaycastHit hit,_slopeData.FloatRayDistance,_playerMovementSm.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal,-downwardsRay.direction);

            float slopeSpeedModifier=SetSlopeSpeedModiferOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                return;
            }
            float distanceToFloatingPoint = _playerMovementSm.Player.CapsuleColliderUtility.CapsuleColliderData
                .ColliderCenterInLocalSpace.y*_playerMovementSm.Player.transform.localScale.y-hit.distance;
            if (distanceToFloatingPoint == 0f)
            {               
                return;
            }
            float amountToLift = distanceToFloatingPoint * _slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            _playerMovementSm.Player._rigidbody.AddForce(liftForce,ForceMode.VelocityChange);
        }
    }

    private float SetSlopeSpeedModiferOnAngle(float angle)
    {
        float slopeSpeedModifer = movementData.SlopeSpeedAngle.Evaluate(angle);
        if (_playerMovementSm.ReusableData.OnSlopesMovementSpeedModifer != slopeSpeedModifer)
        {
            _playerMovementSm.ReusableData.OnSlopesMovementSpeedModifer = slopeSpeedModifer;
            UpdateCameraRecenterState(_playerMovementSm.ReusableData.MovementInput);
        }

        return slopeSpeedModifer;
    }

    protected override void OnMovementPerformed(InputAction.CallbackContext context)
    {
        base.OnMovementPerformed(context);
        UpdateTargetRotation(GetMovementInputDirection());
    }

    protected virtual void OnMove()
    {
        if (_playerMovementSm.ReusableData.ShouldSprint)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.SprintingState);
            return;
        }
        if (_playerMovementSm.ReusableData.ShouldWalk)
        {
            _playerMovementSm.ChangeState(_playerMovementSm.WalkingState);
            return;
        }
        _playerMovementSm.ChangeState(_playerMovementSm.RunningState);
    }
    
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Dash.started += OnDashStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Jump.started += OnJumpStarted;
    }

    
    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _playerMovementSm.Player.playerInput.PlayerActions.Dash.started -= OnDashStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Jump.started -= OnJumpStarted;
    }
    
    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.JumpingState);
    }
    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        _playerMovementSm.ChangeState(_playerMovementSm.DashState);
    }




}