using UnityEngine;

public class GoToShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        if (!agent.BB.GSI.shelfRouteReached)
        {
            agent.BB.IKRigging.IKRiggingEnabled = true;
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    
}
