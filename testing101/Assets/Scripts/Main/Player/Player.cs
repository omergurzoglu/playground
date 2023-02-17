using System;
using UnityEngine;
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    private PlayerMovementSM _playerMovementSm;
    public PlayerInput playerInput { get; private set; }
    public Rigidbody _rigidbody { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    [field:SerializeField]public PlayerSO Data { get; private set; }
    [field:SerializeField]public PlayerCapsuleColliderUtility CapsuleColliderUtility { get; private set; }
    [field:SerializeField]public LayerData LayerData { get; private set; }
    [field:SerializeField]public PlayerCameraUtility CameraUtility { get; private set; }
    
    private void Awake()
    {
        CameraUtility.Initialize();
         if (Camera.main != null) MainCameraTransform = Camera.main.transform;
        _rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        CapsuleColliderUtility.Initialize(gameObject);
        CapsuleColliderUtility.CalculateCapsuleColliderDimension();
        _playerMovementSm = new PlayerMovementSM(this);
    }

    private void OnValidate()
    {
        CapsuleColliderUtility.Initialize(gameObject);
        CapsuleColliderUtility.CalculateCapsuleColliderDimension();
    }

    private void Start()
    {
        _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
    }

    private void Update()
    {
        _playerMovementSm.HandeInput();
        _playerMovementSm.Tick();
    }

    private void FixedUpdate()
    {
        _playerMovementSm.PhysicsTick();
    }

    private void OnTriggerEnter(Collider collider)
    {
        _playerMovementSm.OnTriggerEnter(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        _playerMovementSm.OnTriggerExit(collider);
    }
}
