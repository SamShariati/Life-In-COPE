using UnityEngine;

public class FallBackwardConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {


        if (agent.BB.GetHit.isCurrFallingBackward)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
