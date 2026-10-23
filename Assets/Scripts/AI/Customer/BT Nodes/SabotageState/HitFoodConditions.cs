using UnityEngine;

public class HitFoodConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (StateConditions(agent))
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {

        if (!agent.isCurrentlyGoingToStall && agent.isCurrentlyHittingStall && !agent.isCurrentlyTaunting)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
