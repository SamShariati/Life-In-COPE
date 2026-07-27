using NUnit.Framework;
using UnityEngine;

public class GoToAisleConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        throw new System.NotImplementedException();
    }


    private void GetCorrectAisle(CustomerManager agent)
    {
        if (agent.currentChosenGood == null)
        {
            int rand = Random.Range(0, agent.remainingGoodsList.Count);
            agent.currentChosenGood = agent.remainingGoodsList[rand];

        }

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
