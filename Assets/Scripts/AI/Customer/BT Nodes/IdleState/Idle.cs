using UnityEngine;

public class Idle : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.idle;

        agent.animator.SetState(AnimState.Idle);


        if (agent.C_Functions.TickTimer(Time.deltaTime))
        {
            agent.idleStateAllowed = false;
            agent.idleStateActivated = false;

            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;

        
    }
}
