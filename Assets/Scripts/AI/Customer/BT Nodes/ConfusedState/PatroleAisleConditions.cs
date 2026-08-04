using UnityEngine;
using UnityEngine.AI;

public class PatroleAisleConditions : BTNode
{
    

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.patroleAisleConditions;
        if (StateConditions(agent))
        {
            agent.patroleRouteChosen = true;
            GetCorrectAisle(agent);
            ChooseAislePatrolePoint(agent);

            return NodeState.SUCCESS;
        }
        else if (agent.isCurrentlyPatrolling)
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
        if (!agent.patroleRouteChosen && agent.isCurrentlyPatrolling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetCorrectAisle(CustomerManager agent)
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();

            if (shelfGoodsType == agent.currentChosenGood.boxID)
            {
                agent.aisleID = (int)shelf.aisle;
            }
        }

    }
    private void ChooseAislePatrolePoint(CustomerManager agent)
    {
        int randomIndex = Random.Range(0, agent.aislePosList[agent.aisleID].Count);
        agent.chosenAislePos = agent.aislePosList[agent.aisleID][randomIndex].position;
    }
}
