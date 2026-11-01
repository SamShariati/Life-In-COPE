using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ChasePlayerConditions : CustomerBTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        //Entering SearchForPlayerState for the first time.
        if (StateConditions(agent))
        {
            agent.BB.SFP.isCurrentlyChasing = true;
            return NodeState.SUCCESS;
        }

        else if (agent.BB.SFP.isCurrentlyChasing)
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
        if (!agent.BB.SFP.isCurrentlyChasing && !agent.BB.SFP.isCurrentlyFollowing && !agent.BB.SFP.isCurrentlyStaring)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
