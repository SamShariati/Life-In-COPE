using UnityEngine;

public class CheckWrongShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.checkWrongShelfConditions;

        if (StateConditions(agent))
        {
            agent.wrongShelfChosen = true;

            return NodeState.SUCCESS;
           
        }
        else if (agent.isCurrCheckingWrongShelf)
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
        if (!agent.wrongShelfChosen && agent.isCurrCheckingWrongShelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void ChooseAislePatrolePoint(CustomerManager agent)
    {
        int randomIndex = Random.Range(0, agent.aislePosList[agent.aisleID].Count);
        agent.chosenAislePos = agent.aislePosList[agent.aisleID][randomIndex].position;
    }

    private void ChooseWrongShelf(CustomerManager agent)
    {

    }
}
