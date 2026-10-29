using UnityEngine;

public class IdleStareConditions : BTNode
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
        if (!agent.BB.SFP.isCurrentlyChasing && !agent.BB.SFP.isCurrentlyFollowing && agent.BB.SFP.isCurrentlyStaring)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
