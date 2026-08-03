using UnityEngine;

public class GoToShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToShelfConditions;
        if (!agent.shelfRouteChosen)
        {
            agent.shelfRouteChosen = true;

            return NodeState.SUCCESS;

        }
        else if (agent.shelfRouteChosen && !agent.shelfRouteReached)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    
}
