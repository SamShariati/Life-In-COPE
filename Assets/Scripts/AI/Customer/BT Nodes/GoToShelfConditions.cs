using UnityEngine;

public class GoToShelfConditions : BTNode
{
    bool runOnce = false;

    public override NodeState Evaluate(CustomerManager agent)
    {

        //if (!runOnce)
        //{
        //    runOnce = true;
        //    Debug.Log("Entered GoToShelfConditions");
        //}


        if (!agent.shelfRouteChosen)
        {
            agent.shelfRouteChosen = true;
            ChooseShelfRoute(agent);

            return NodeState.SUCCESS;

        }
        else if (agent.shelfRouteChosen && !agent.shelfRouteReached)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    private void ChooseShelfRoute(CustomerManager agent)
    {
        if (agent.remainingGoodsList.Count > 0)
        {
            int rand = Random.Range(0, agent.remainingGoodsList.Count);
            agent.currentChosenGood = agent.remainingGoodsList[rand];

            agent.chosenShelfPosition = agent.shelfPosPairs[agent.currentChosenGood];
        }
        else
        {
            agent.SwitchState(agent.standInLineState);
            agent.BTActivated = false;
        }
    }
}
