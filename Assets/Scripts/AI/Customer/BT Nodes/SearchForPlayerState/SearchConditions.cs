using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SearchConditions : CustomerBTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.BB.IKRigging.IKRiggingEnabled = false;
            agent.BB.SFP.spottedPlayer = true;
            agent.BB.Sabotage.sabotageStateAllowed = false;
            agent.C_Functions.SearchForPlayerResetFlags();
            CheckIfGoodChosen(agent);

            return NodeState.SUCCESS;
        }

        else if (agent.BB.SFP.spottedPlayer && !PlayerState.Instance.currentlyBeingFollowed)
        {
            return NodeState.SUCCESS;
        }

        else if (agent.BB.SFP.isCurrentlyFollowing || agent.BB.SFP.isCurrentlyStaring)
        {
            return NodeState.SUCCESS;
        }

        else
        {
            agent.BB.SFP.spottedPlayer = false;
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {
        if (agent.BB.SFP.customerVision.CanSeePlayer() && !agent.BB.SFP.spottedPlayer && !PlayerState.Instance.currentlyBeingFollowed
            && agent.C_Functions.CheckSearchForPlayerStateCD() && agent.BB.SFP.allowedToChase && !PlayerState.Instance.inScanningMode)
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

    // In case AI enters BT and instantly detects player. Shouldn't normally happen.
    public void CheckIfGoodChosen(CustomerManager agent)
    {
        if (agent.BB.GSI.currentChosenGood == null)
        {
            int rand = Random.Range(0, agent.BB.GSI.remainingGoodsList.Count);
            agent.BB.GSI.currentChosenGood = agent.BB.GSI.remainingGoodsList[rand];

            agent.BB.GSI.chosenShelfPosition = agent.BB.GSI.allShelfArrowPositions[agent.BB.GSI.currentChosenGood.boxID];
        }
    }

    private bool RollSearchForPlayerChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0f, 100f);
        agent.C_Functions.ResetSearchForPlayerStateCD();

        if (randPercent <= agent.chasePlayerChance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
