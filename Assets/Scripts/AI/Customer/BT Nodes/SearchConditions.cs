using UnityEngine;

public class SearchConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (agent.customerVision.CanSeePlayer() && !agent.playerSpotted)
        {
            agent.playerSpotted = true;
            agent.C_Functions.ResetFlagVariables();
            return NodeState.SUCCESS;
        }

        else if (agent.playerSpotted)
        {
            return NodeState.SUCCESS;
        }

        else
        {
            return NodeState.FAILURE;
        }
    }
}
