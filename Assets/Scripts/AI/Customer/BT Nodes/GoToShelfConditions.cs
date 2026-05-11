using UnityEngine;

public class GoToShelfConditions : BTNode
{
    bool runOnce = false;

    public override NodeState Evaluate(CustomerManager agent)
    {

        if (!runOnce)
        {
            runOnce = true;
            Debug.Log("Entered GoToShelfConditions");
        }


        if (!agent.shelfRouteChosen)
        {
            agent.shelfRouteChosen = true;
            ChooseShelfRoute(agent);

            return NodeState.SUCCESS;

        }
        else if (agent.shelfRouteChosen)
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
            CardboardBoxData chosenGoods = agent.remainingGoodsList[rand];

            agent.chosenShelfPosition = agent.goodsShelfPairs[chosenGoods];
        }
        else
        {
            //Aktivera Kassa FSM
        }
    }
}
