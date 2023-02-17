
using UnityEngine;

public interface IState
{
    public void OnEnter();
    public void OnExit();
    public void Tick();
    public void PhysicsTick();
    public void HandleInput();
    public void OnAnimationEnterEvent();
    public void OnAnimationExitEvent();
    public void OnAnimationTransitionEvent();
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerExit(Collider collider);


}
