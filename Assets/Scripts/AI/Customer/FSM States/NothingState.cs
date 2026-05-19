using UnityEngine;

public class NothingState : FSMBaseState
{
    public override void EnterState(CustomerManager agent)
    {
        agent.animator.SetState(AnimState.Idle);
    }

    public override void UpdateState(CustomerManager agent)
    {

    }
}
