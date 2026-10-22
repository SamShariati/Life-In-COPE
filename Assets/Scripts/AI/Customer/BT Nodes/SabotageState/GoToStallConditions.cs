using UnityEngine;
using UnityEngine.AI;

public class GoToStallConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (!agent.isCurrentlyGoingToStall)
        {

            agent.isCurrentlyGoingToStall = true;
            ChooseFoodStallPosition(agent);

            return NodeState.SUCCESS;
        }

        else if (agent.isCurrentlyGoingToStall)
        {
            return NodeState.SUCCESS;
        }

        else
        {
            return NodeState.FAILURE;
        }

    }

    private void ChooseFoodStallPosition(CustomerManager agent)
    {
        int randInt = Random.Range(0, agent.foodStallList.Count);

        FoodStall foodStall = agent.foodStallList[randInt].GetComponent<FoodStall>();

        agent.chosenFoodStallPos = foodStall.stallPosList[Random.Range(0, foodStall.stallPosList.Count)];

    }

}
