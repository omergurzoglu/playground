
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    private float _inputX;

    private float _inputZ;

    private Vector3 _desiredMoveDirection;

    private bool _blockRotationPlayer = false;

    [Range(0, 0.5f)] public float desiredRotationSpeed = 0.1f;

    public Animator animator;

    private float _speed;

    [Range(0, 1f)] public float allowPlayerRotation = 0.1f;

    public Camera cam;

    public CharacterController characterController;

    private bool _isGrounded;
    private Vector3 _rightFootPosition, _leftFootPosition, _leftFootIKPosition, _rightFootIKPosition;
    private Quaternion _leftFootIKRotation, _rightFootIKRotation;
    private float _lastPelvisPositionY, _lastRightFootPositionY, _lastLeftFootPositionY;

    [Header("Smoothing")]
    [Range(0, 1f)] public float horizontalAnimationSmoothTime=0.2f;

    [Range(0, 1f)] public float verticalAnimationSmoothTime=0.2f;
    [Range(0, 1f)] public float startAnimationTime = 0.3f;
    [Range(0, 1f)] public float stopAnimationTime = 0.15f;

    private float _verticalVelocity;
    private Vector3 _moveVector;
    [Header("Feet")] 
    public bool enableFootIK=true;

    [SerializeField][Range(0,2f)]private float heightFromGroundRaycast=1.14f;
    [SerializeField] [Range(0, 2f)] private float raycastDownDistance=1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField] [Range(0, 1)] private float pelvisUpAndDownSpeed=0.28f;
    [SerializeField] [Range(0, 1)] private float feetToIKPositionSpeed = 0.5f;
    public string leftFootAnimatorVariableName = "LeftFootCurve";
    public string rightFootAnimatorVariableName = "RightFootCurve";
    public bool useProIKFeature = false;
    public bool showSolverDebug=true;
    
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cam=Camera.main;
        characterController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        InputMagnitude();
        // _isGrounded = characterController.isGrounded;
        // if (_isGrounded)
        // {
        //     _verticalVelocity -= 0;
        // }
        // else
        // {
        //     _verticalVelocity -= 2;
        // }
        //
        // _moveVector = new Vector3(0, _verticalVelocity, 0);
        // characterController.Move(_moveVector);
    }

    private void FixedUpdate()
    {
        if (enableFootIK == false)
        {
            return;
        }
        AdjustFeetTarget(ref _rightFootPosition,HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref _leftFootPosition,HumanBodyBones.LeftFoot);
        
        FeetPositionSolver(_rightFootPosition,ref _rightFootIKPosition,ref _rightFootIKRotation);
        FeetPositionSolver(_leftFootPosition,ref _leftFootIKPosition,ref _leftFootIKRotation);
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFootIK == false)
        {
            return;
        }
        MovePelvisHeight();
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,animator.GetFloat(rightFootAnimatorVariableName));
        }
        
        MoveFeetToIKPoint(AvatarIKGoal.RightFoot,_rightFootIKPosition,_rightFootIKRotation,ref _lastRightFootPositionY);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,animator.GetFloat(leftFootAnimatorVariableName));
        }
        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot,_leftFootIKPosition,_leftFootIKRotation,ref _lastLeftFootPositionY);
    }

    void MoveFeetToIKPoint(AvatarIKGoal foot,Vector3 positionIKHolder,Quaternion rotationIKHolder,ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = animator.GetIKPosition(foot);
        if (positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);
            float y = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
            targetIKPosition.y += y;
            lastFootPositionY = y;
            targetIKPosition = transform.TransformPoint(targetIKPosition);
            animator.SetIKRotation(foot,rotationIKHolder);
            
        }
        animator.SetIKPosition(foot,targetIKPosition);
        
    }

    private void MovePelvisHeight()
    {
        if (_rightFootIKPosition == Vector3.zero || _leftFootIKPosition == Vector3.zero || _lastPelvisPositionY == 0)
        {
            _lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float leftOffsetPosition = _leftFootIKPosition.y - transform.position.y;
        float rightOffsetPosition = _rightFootIKPosition.y - transform.position.y;
        float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;
        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPosition.y = Mathf.Lerp(_lastPelvisPositionY, newPelvisPosition.y,pelvisUpAndDownSpeed);
        animator.bodyPosition = newPelvisPosition;
        _lastPelvisPositionY = animator.bodyPosition.y;

    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        RaycastHit hit;
        if (showSolverDebug)
        {
            Debug.DrawLine(fromSkyPosition,fromSkyPosition+Vector3.down*(raycastDownDistance+heightFromGroundRaycast),Color.yellow);
        }

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out hit, raycastDownDistance + heightFromGroundRaycast, groundLayer))
        {
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = hit.point.y + pelvisOffset;
            feetIKRotations=Quaternion.FromToRotation(Vector3.up, hit.normal)*transform.rotation;
            return;
        }

        feetIKPositions = Vector3.zero;
    }

    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }
    
        
    

    private void PlayerMoveAndRotate()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
        
        
        var cameraTransform = cam.transform;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        _desiredMoveDirection = forward * _inputZ + right * _inputX;
        if (_blockRotationPlayer == false)
        {
            transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(_desiredMoveDirection),desiredRotationSpeed);
        }
        
    }

    private void InputMagnitude()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
        animator.SetFloat("_inputX",_inputX,horizontalAnimationSmoothTime,Time.deltaTime);
        animator.SetFloat("_inputZ",_inputZ,verticalAnimationSmoothTime,Time.deltaTime);

        _speed = new Vector2(_inputX, _inputZ).sqrMagnitude;
        if (_speed > allowPlayerRotation)
        {
            animator.SetFloat("InputMagnitude",_speed,startAnimationTime,Time.deltaTime);
            PlayerMoveAndRotate();
        }
        else if (_speed<allowPlayerRotation)
        {
            animator.SetFloat("InputMagnitude",_speed,stopAnimationTime,Time.deltaTime);
        }

    }
}
