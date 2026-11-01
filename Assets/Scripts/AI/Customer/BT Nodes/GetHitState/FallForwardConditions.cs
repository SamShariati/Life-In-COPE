using UnityEngine;

public class FallForwardConditions : CustomerBTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {


        if (agent.BB.GetHit.isCurrFallingForward)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
