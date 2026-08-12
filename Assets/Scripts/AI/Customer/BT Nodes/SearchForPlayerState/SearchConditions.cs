using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SearchConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.spottedPlayer = true;
            agent.C_Functions.SearchForPlayerResetFlags();
            CheckIfGoodChosen(agent);
            return NodeState.SUCCESS;
        }

        else if (agent.spottedPlayer && !PlayerState.Instance.currentlyBeingFollowed)
        {
            return NodeState.SUCCESS;
        }

        else if (agent.isCurrentlyFollowing || agent.isCurrentlyStaring)
        {
            return NodeState.SUCCESS;
        }

        else
        {
            agent.spottedPlayer = false;
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {
        if (agent.customerVision.CanSeePlayer() && !agent.spottedPlayer && !PlayerState.Instance.currentlyBeingFollowed
            && agent.C_Functions.CheckSearchForPlayerStateCD() && agent.allowedToChase)
        {
            if (RollSearchForPlayerChance(agent))
            {
                return true;
            }
            else
            {
                return false;
            }
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

            agent.chosenShelfPosition = agent.allShelfArrowPositions[agent.currentChosenGood.boxID];
        }
    }

    private bool RollSearchForPlayerChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent <= agent.chasePlayerChance)
        {
            return true;
        }
        else
        {
            agent.C_Functions.StartSearchForPlayerStateCD();
            return false;
        }

    }
}
