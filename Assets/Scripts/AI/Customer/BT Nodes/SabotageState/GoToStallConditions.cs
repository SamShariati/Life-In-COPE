using UnityEngine;
using UnityEngine.AI;

public class GoToStallConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.BB.IKRigging.IKRiggingEnabled = true;
            agent.BB.Sabotage.isCurrentlyGoingToStall = true;
            ChooseFoodStallPosition(agent);

            return NodeState.SUCCESS;
        }

        else if (agent.BB.Sabotage.isCurrentlyGoingToStall)
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
        if (!agent.BB.Sabotage.isCurrentlyGoingToStall && !agent.BB.Sabotage.isCurrentlyHittingStall && !agent.BB.Sabotage.isCurrentlyTaunting)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChooseFoodStallPosition(CustomerManager agent)
    {
        int randInt = Random.Range(0, agent.BB.Sabotage.foodStallList.Count);

        agent.BB.Sabotage.chosenFoodStall = agent.BB.Sabotage.foodStallList[randInt].GetComponent<FoodStall>();

        agent.BB.Sabotage.chosenFoodStallPos = agent.BB.Sabotage.chosenFoodStall.stallPosList[Random.Range(0, agent.BB.Sabotage.chosenFoodStall.stallPosList.Count)];

    }

}
