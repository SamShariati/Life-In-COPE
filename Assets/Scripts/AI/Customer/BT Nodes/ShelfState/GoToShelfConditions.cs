using UnityEngine;

public class GoToShelfConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToShelfConditions;
        if (!agent.shelfRouteChosen)
        {

            agent.shelfRouteChosen = true;

            if (!agent.C_Functions.ChooseShelfRoute(agent))
            {
                agent.SwitchState(agent.goToLineState);
                agent.BTActivated = false;
            }
            //ChooseShelfRoute(agent);

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
            agent.SwitchState(agent.goToLineState);
            agent.BTActivated = false;
        }
    }
}
