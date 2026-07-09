using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SearchConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.playerSpotted = true;
            agent.C_Functions.ResetFlagVariables();
            return NodeState.SUCCESS;
        }

        else if (agent.playerSpotted && !PlayerInventory.Instance.currentlyBeingFollowed) //!PlayerInventory.Instance.currentlyBeingFollowed
        {
            return NodeState.SUCCESS;
        }

        else if (agent.isCurrentlyFollowing || agent.isCurrentlyStaring)
        {
            return NodeState.SUCCESS;
        }

        else
        {
            agent.playerSpotted = false;
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {
        if (agent.customerVision.CanSeePlayer() && !agent.playerSpotted && !PlayerInventory.Instance.currentlyBeingFollowed) //!PlayerInventory.Instance.currentlyBeingFollowed
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
