using UnityEngine;

public class NothingState : CustomerFSMBaseState
{
    public override void EnterState(CustomerManager agent)
    {
        agent.animator.SetState(AnimState.Idle);
    }

    public override void UpdateState(CustomerManager agent)
    {

    }
}
