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

        if (!agent.BB.Sabotage.isCurrentlyGoingToStall && agent.BB.Sabotage.isCurrentlyHittingStall && !agent.BB.Sabotage.isCurrentlyTaunting)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
