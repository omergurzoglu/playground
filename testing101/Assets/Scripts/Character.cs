
using System;
using Unity.Mathematics;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float turnRate;
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private Vector3 _direction;
    private bool isRunning = false;
    private bool isIdle = true;
    [SerializeField] private LayerMask _layerMask;
    
    [Range(0,1f)]
    [SerializeField] private float distanceToGround;

    private static readonly int IKLeftFootWeight = Animator.StringToHash("IKLeftFootWeight");
    private static readonly int IKRightFootWeight = Animator.StringToHash("IKRightFootWeight");


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        
    }
    
    void Update()
    {
        CheckGround();
        CheckAnimation();
    }

    private void CheckGround()
    {
        if (!_characterController.isGrounded) {
            Vector3 gravityDirection = new Vector3(0, gravity, 0);
            _characterController.Move(gravityDirection * Time.deltaTime);
        }
        
    }

    private void CheckAnimation()
    {
     
        if (Input.anyKey)
        {
            if (isIdle)
            {
                _animator.SetBool(IsRunning, true);
                _animator.SetBool(IsIdle, false);
                isRunning = true;
                isIdle = false;
            }
            Movement();
        }
        else
        {
            if (isRunning)
            {
                _animator.SetBool(IsRunning, false);
                _animator.SetBool(IsIdle, true);
                isRunning = false;
                isIdle = true;
            }
        }
    }

    private void Movement()
    {
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // Vector3 move = transform.right * x + transform.forward * z;
        // _characterController.Move(move * (moveSpeed * Time.deltaTime));
        
        var newMove=new Vector3(x*moveSpeed,0,z*moveSpeed);
        _characterController.Move(newMove * (moveSpeed * Time.deltaTime));
        _direction = Vector3.forward * z + Vector3.right * x;
        transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(newMove),turnRate*Time.deltaTime);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,_animator.GetFloat(IKLeftFootWeight));
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,_animator.GetFloat(IKLeftFootWeight));
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,_animator.GetFloat(IKRightFootWeight));
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,_animator.GetFloat(IKRightFootWeight));

        RaycastHit raycastHit;
        Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out raycastHit, distanceToGround + 1f,_layerMask))
        {
            Vector3 footPos = raycastHit.point;
            footPos.y += distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot,footPos);
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward,raycastHit.normal)); 
            //_animator.SetIKRotation(AvatarIKGoal.LeftFoot,quaternion.LookRotation(transform.forward,raycastHit.normal));
        }
        ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out raycastHit, distanceToGround + 1f,_layerMask))
        {
            Vector3 footPos = raycastHit.point;
            footPos.y += distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.RightFoot,footPos);
           // _animator.SetIKRotation(AvatarIKGoal.RightFoot,quaternion.LookRotation(transform.forward,raycastHit.normal));
           Vector3 forward = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
           _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward,raycastHit.normal)); 
        }
    }
}
