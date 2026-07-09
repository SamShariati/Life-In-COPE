using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ChasePlayerConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        //Entering SearchForPlayerState for the first time.
        if (StateConditions(agent))
        {
            agent.isCurrentlyChasing = true;
            return NodeState.SUCCESS;
        }

        else if (agent.isCurrentlyChasing)
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
        if (!agent.isCurrentlyChasing && !agent.isCurrentlyFollowing && !agent.isCurrentlyStaring)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
