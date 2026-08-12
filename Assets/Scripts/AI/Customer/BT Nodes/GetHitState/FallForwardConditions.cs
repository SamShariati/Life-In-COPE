using UnityEngine;

public class FallForwardConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (agent.isCurrFallingForward)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
