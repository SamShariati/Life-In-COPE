using UnityEngine;

public class FallBackward : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (agent.isCurrFallingBackward)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
