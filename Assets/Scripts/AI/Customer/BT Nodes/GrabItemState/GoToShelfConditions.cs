using UnityEngine;

public class GoToShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToShelfConditions;

        if (!agent.shelfRouteReached)
        {
            agent.IKRiggingEnabled = true;
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    
}
