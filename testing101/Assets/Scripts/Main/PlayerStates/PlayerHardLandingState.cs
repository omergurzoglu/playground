

    using UnityEngine.InputSystem;

    public class PlayerHardLandingState : PlayerLandingState
    {
        public PlayerHardLandingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
        {
        }

        public override void OnEnter()
        {
            _playerMovementSm.ReusableData.MovementSpeedModifer = 0f;
            base.OnEnter();
            _playerMovementSm.Player.playerInput.PlayerActions.Movement.Disable();
            
            ResetVelocity();
        }
        public override void PhysicsTick()
        {
            base.PhysicsTick();
            if (!IsMovingHorizontally())
            {
                return;
            }
            ResetVelocity();
        }

        public override void OnExit()
        {
            base.OnExit();
            _playerMovementSm.Player.playerInput.PlayerActions.Movement.Enable();
         }

        public override void OnAnimationExitEvent()
        {
            _playerMovementSm.Player.playerInput.PlayerActions.Movement.Enable();
        }

        public override void OnAnimationTransitionEvent()
        {
            _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
        }

        protected override void OnMove()
        {
            if (_playerMovementSm.ReusableData.ShouldWalk)
            {
                return;
            }
            _playerMovementSm.ChangeState(_playerMovementSm.RunningState);
            
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

        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            
            
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            _playerMovementSm.Player.playerInput.PlayerActions.Movement.started -= OnMovementStarted;
        }
    }
