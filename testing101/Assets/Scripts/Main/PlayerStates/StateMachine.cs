
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void HandeInput()
    {
        currentState?.HandleInput();
    }
    public void Tick()
    {
        currentState?.Tick();
    }
    public void PhysicsTick()
    {
        currentState?.PhysicsTick();
    }

    public void OnAnimationEnterEvent()
    {
        currentState?.OnAnimationEnterEvent();
    }
    public void OnAnimationExitEvent()
    {
        currentState?.OnAnimationExitEvent();
    }
    public void OnAnimationTransitionEvent()
    {
        currentState?.OnAnimationTransitionEvent();
    }

    public void OnTriggerEnter(Collider collider)
    {
        currentState?.OnTriggerEnter(collider);
    }
    public void OnTriggerExit(Collider collider)
    {
        currentState?.OnTriggerExit(collider);
    }
    

}
