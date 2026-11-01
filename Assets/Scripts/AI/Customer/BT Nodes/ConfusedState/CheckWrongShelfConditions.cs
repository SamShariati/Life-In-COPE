using UnityEngine;

public class CheckWrongShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.checkWrongShelfConditions;

        if (StateConditions(agent))
        {
            agent.BB.Confused.wrongShelfChosen = true;
            agent.C_Functions.GetCorrectAisle();
            ChooseWrongShelfPosition(agent);

            return NodeState.SUCCESS;
           
        }
        else if (agent.BB.Confused.isCurrCheckingWrongShelf)
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
        if (!agent.BB.Confused.wrongShelfChosen && agent.BB.Confused.isCurrCheckingWrongShelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void ChooseWrongShelfPosition(CustomerManager agent)
    {
        int randomIndex = Random.Range(0, ShelfManager.Instance.shelvesByAisle[agent.BB.Confused.aisleID].Count);
        agent.BB.Confused.currentWrongShelf = ShelfManager.Instance.shelvesByAisle[agent.BB.Confused.aisleID][randomIndex];

        string shelfGoodsType = agent.BB.Confused.currentWrongShelf.goodsType.ToString();


        agent.BB.Confused.wrongShelfArrowPos = agent.BB.GSI.allShelfArrowPositions[shelfGoodsType];
    }
}
