

    using UnityEngine;

    public class PlayerLightLandingState : PlayerLandingState
    {
        public PlayerLightLandingState(PlayerMovementSM playerMovementSm) : base(playerMovementSm)
        {
        }

        public override void OnEnter()
        {
            _playerMovementSm.ReusableData.MovementSpeedModifer = 0f;
            base.OnEnter();
            
            _playerMovementSm.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;
            
            ResetVelocity();
        }

        public override void Tick()
        {
            base.Tick();
            if (_playerMovementSm.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }
            OnMove();
        }

        public override void OnAnimationTransitionEvent()
        {
            _playerMovementSm.ChangeState(_playerMovementSm.IdleState);
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
    }
