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
            CheckIfGoodChosen(agent);
            return NodeState.SUCCESS;
        }

        else if (agent.playerSpotted && !PlayerState.Instance.currentlyBeingFollowed)
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
        if (agent.customerVision.CanSeePlayer() && !agent.playerSpotted && !PlayerState.Instance.currentlyBeingFollowed
            && agent.C_Functions.CheckSearchForPlayerCooldown() && agent.allowedToChase)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void CheckIfGoodChosen(CustomerManager agent)
    {
        if (agent.currentChosenGood == null)
        {
            int rand = Random.Range(0, agent.remainingGoodsList.Count);
            agent.currentChosenGood = agent.remainingGoodsList[rand];

            agent.chosenShelfPosition = agent.shelfPosPairs[agent.currentChosenGood];
        }
    }
}
