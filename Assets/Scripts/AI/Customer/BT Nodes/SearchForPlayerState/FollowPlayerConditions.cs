using UnityEngine;

public class FollowPlayerConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            PlayerState.Instance.currentlyBeingFollowed = true;
            agent.getHitStateAllowed = false;

            return NodeState.SUCCESS;
        }

        else
        {
            return NodeState.FAILURE;
        }
    }


    private bool StateConditions(CustomerManager agent)
    {
        if (!agent.isCurrentlyChasing && agent.isCurrentlyFollowing && !agent.isCurrentlyStaring)
        {
            return true;
        }
        else
        {
            return false;
        }

    } 
}
