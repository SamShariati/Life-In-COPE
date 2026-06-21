using UnityEngine;

public class FollowPlayerConditions : BTNode
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
        if (!agent.isCurrentlyChasing && agent.isCurrentlyFollowing && !agent.isCurrentlyIdle)
        {
            return true;
        }
        else
        {
            return false;
        }

    } 
}
