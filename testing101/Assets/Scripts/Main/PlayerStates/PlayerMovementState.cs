
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    protected PlayerMovementSM _playerMovementSm;
    protected PlayerGroundedData movementData;
    protected PlayerAirborneData airborneData;
    
    public PlayerMovementState(PlayerMovementSM playerMovementSm)
    {
        _playerMovementSm = playerMovementSm;
        movementData = _playerMovementSm.Player.Data.PlayerGroundedData;
        airborneData = playerMovementSm.Player.Data.AirborneData;
        
        InitializeData();
    }

    #region  Istate
    private void InitializeData()
    {
        SetBaseCameraRecenterData();
        SetBaseRotationData();
    }

    protected void SetBaseRotationData()
    {
        _playerMovementSm.ReusableData.RotationData = movementData.BaseRotationData;
        _playerMovementSm.ReusableData.TimeToReachTargetRotation = _playerMovementSm.ReusableData.RotationData.targetRotationReachTime;
    }

    public virtual void OnEnter()
    {
        Debug.Log("state"+GetType().Name );
        AddInputActionsCallbacks();
    }
    
    public virtual void OnExit()
    {
        RemoveInputActionsCallbacks();
    }
    

    public virtual void Tick()
    {
        
    }

    public virtual void PhysicsTick()
    {
        Move();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void OnAnimationEnterEvent()
    {
        
    }

    public virtual void OnAnimationExitEvent()
    {
        
    }

    public virtual void OnAnimationTransitionEvent()
    {
        
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (_playerMovementSm.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGround(collider);
            return;
        }
    }

    public virtual void OnTriggerExit(Collider collider)
    {
        if (_playerMovementSm.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGroundExited(collider);
            return;
        }
    }

    protected virtual void OnContactWithGroundExited(Collider collider)
    {
        
    }

    protected virtual void OnContactWithGround(Collider collider)
    {
       
    }

    

    #endregion
    

    private void Move()
    {
        if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero||_playerMovementSm.ReusableData.MovementSpeedModifer==0f) 
        {
            return;
        }
        Vector3 movementDirection = GetMovementInputDirection();
        float targetRotationYAngle = Rotate(movementDirection);
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle); 
        float movementSpeed = GetMovementSpeed();
        Vector3 currentHorizontalVelocity = GetPlayerHorizontalVelocity();
        _playerMovementSm.Player._rigidbody.AddForce(targetRotationDirection*movementSpeed-currentHorizontalVelocity,ForceMode.VelocityChange);
    }

    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);
        RotateTowardsTargetRotation();
        return directionAngle;
    }

    protected float UpdateTargetRotation(Vector3 direction,bool shouldConsiderCameraRotation=true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }
            
        if (directionAngle != _playerMovementSm.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    protected Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0f, _playerMovementSm.Player._rigidbody.velocity.y, 0f);
    }

    protected void ResetVerticalVelocity()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        _playerMovementSm.Player._rigidbody.velocity=playerHorizontalVelocity;
    }

    private void UpdateTargetRotationData(float targetAngle)
    {
        _playerMovementSm.ReusableData.CurrentTargetRotation.y = targetAngle;
        _playerMovementSm.ReusableData.DampedTargetRotationPassedTime.y = 0f;
    }

    protected void RotateTowardsTargetRotation()
    {
        float currentYAngle = _playerMovementSm.Player._rigidbody.rotation.eulerAngles.y;
        if (currentYAngle == _playerMovementSm.ReusableData.CurrentTargetRotation.y)
        {
           return;   
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, _playerMovementSm.ReusableData.CurrentTargetRotation.y,
            ref _playerMovementSm.ReusableData.DampedTargetRotationCurrentVelocity.y,_playerMovementSm.ReusableData.TimeToReachTargetRotation.y-_playerMovementSm.ReusableData.DampedTargetRotationPassedTime.y);

        _playerMovementSm.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;
        
        Quaternion targetRotation=Quaternion.Euler(0f,smoothedYAngle,0f);
        _playerMovementSm.Player._rigidbody.MoveRotation(targetRotation);
    }

    private float AddCameraRotationToAngle(float angle)
    {
        angle += _playerMovementSm.Player.MainCameraTransform.eulerAngles.y;
        if (angle > 360f)
        {
            angle -= 360f;
        }
    
        return angle;
    }

    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg;
        if (directionAngle < 0f)
        {
            directionAngle += 360f;
        }
        return directionAngle;
    }

    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = _playerMovementSm.Player._rigidbody.velocity;
        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }

    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(_playerMovementSm.ReusableData.MovementInput.x, 0f, _playerMovementSm.ReusableData.MovementInput.y);
    }

    protected float GetMovementSpeed(bool shouldConsiderSlopes=true)
    {
        float movementSpeed = movementData.BaseSpeed * _playerMovementSm.ReusableData.MovementSpeedModifer;
        if (shouldConsiderSlopes)
        {
            movementSpeed *= _playerMovementSm.ReusableData.OnSlopesMovementSpeedModifer;
        }

        return movementSpeed;
    }

    private void ReadMovementInput()
    {
        _playerMovementSm.ReusableData.MovementInput = _playerMovementSm.Player.playerInput.PlayerActions.Movement.ReadValue<Vector2>();
    }

    protected void ResetVelocity()
    {
        _playerMovementSm.Player._rigidbody.velocity = Vector3.zero;
    }
   

    protected virtual void OnMovementPerformed(InputAction.CallbackContext context)
    {
        UpdateCameraRecenterState(context.ReadValue<Vector2>());
    }

    private void OnMouseMovementStarted(InputAction.CallbackContext context)
    {
       UpdateCameraRecenterState(_playerMovementSm.ReusableData.MovementInput);
    }

    protected virtual void OnMovementCancelled(InputAction.CallbackContext context)
    {
        DisableCameraRecenter();    
    }

    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        _playerMovementSm.ReusableData.ShouldWalk = !_playerMovementSm.ReusableData.ShouldWalk;
    }
    

    protected void DecelerateHorizontally()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        _playerMovementSm.Player._rigidbody.AddForce(-playerHorizontalVelocity*_playerMovementSm.ReusableData.MovementDecelerationForce,ForceMode.Acceleration);
    }
    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
        _playerMovementSm.Player._rigidbody.AddForce(-playerVerticalVelocity*_playerMovementSm.ReusableData.MovementDecelerationForce,ForceMode.Acceleration);
    }

    protected bool IsMovingHorizontally(float minMagnitude=0.1f)
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);
        return playerHorizontalMovement.magnitude > minMagnitude;
    }

    protected bool IsMovingUp(float minVelocity = 0.1f)
    {
        return GetPlayerVerticalVelocity().y > minVelocity;
    }
    protected bool IsMovingDown(float minVelocity = 0.1f)
    {
        return GetPlayerVerticalVelocity().y < -minVelocity;
    }

    #region  inputs
    
    protected virtual void AddInputActionsCallbacks()
    {
        _playerMovementSm.Player.playerInput.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Look.started += OnMouseMovementStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.performed += OnMovementPerformed;
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.canceled += OnMovementCancelled;
        
        
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        _playerMovementSm.Player.playerInput.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Look.started -= OnMouseMovementStarted;
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.performed -= OnMovementPerformed;
        _playerMovementSm.Player.playerInput.PlayerActions.Movement.canceled -= OnMovementCancelled;
    }

    #endregion

    #region  camera
    protected void EnableCameraRecenter(float waitTime = -1f, float recenterTime = -1f)
    {
        float movementSpeed = GetMovementSpeed();
        if (movementSpeed == 0f)
        {
            movementSpeed = movementData.BaseSpeed;
        }
        _playerMovementSm.Player.CameraUtility.EnableRecenter(waitTime,recenterTime,movementData.BaseSpeed,movementSpeed);
        
    }

    protected void DisableCameraRecenter()
    {
        _playerMovementSm.Player.CameraUtility.DisableRecenter();
    }

    protected void UpdateCameraRecenterState(Vector2 movementInput)
    {
        if (movementInput == Vector2.zero)
        {
            return;
        }

        if (movementInput == Vector2.up)
        {
            DisableCameraRecenter();
            return;
        }

        float cameraVerticalAngle = _playerMovementSm.Player.MainCameraTransform.eulerAngles.x;
        if (cameraVerticalAngle >= 270f)
        {
            cameraVerticalAngle -= 360f;
        }
        cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);
        if (movementInput == Vector2.down)
        {
            SetCameraRecenterState(cameraVerticalAngle,_playerMovementSm.ReusableData.BackwardsCameraRecenterData);
            return;
        }
        SetCameraRecenterState(cameraVerticalAngle,_playerMovementSm.ReusableData.SidewaysCameraRecenterData);
    }

    protected void SetCameraRecenterState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenterData)
    {
        foreach (PlayerCameraRecenteringData recenterData in cameraRecenterData)
        {
            if (!recenterData.IsWithinRange(cameraVerticalAngle))
            {
                continue;
            }
            EnableCameraRecenter(recenterData.WaitTime, recenterData.RecenterTime);
            return;
            
        }

        DisableCameraRecenter();
        
    }
    protected void SetBaseCameraRecenterData()
    {
        _playerMovementSm.ReusableData.SidewaysCameraRecenterData = movementData.SidewaysCameraRecenterData;
        _playerMovementSm.ReusableData.BackwardsCameraRecenterData = movementData.BackwardsCameraRecenterData;
        
    }

    

    #endregion

    
}
