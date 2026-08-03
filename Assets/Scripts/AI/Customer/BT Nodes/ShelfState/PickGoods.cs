using UnityEngine;

public class PickGoods : BTNode
{

    private enum Phase { WaitingToPick, Picking, WaitingToExit}
    private Phase phase = Phase.WaitingToPick;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.pickGoods;

        agent.animator.SetState(AnimState.Idle);
        RotateTowardsShelf(agent);

        switch (phase)
        {
            case Phase.WaitingToPick:

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    
                    agent.C_Functions.SetTimer(agent.maxIdleTime);
                    phase = Phase.WaitingToExit;
                }
                return NodeState.RUNNING;


            case Phase.WaitingToExit:
                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    agent.allowedToChase = true;
                    PickGoodsFromShelf(agent);
                    phase = Phase.WaitingToPick;

                    agent.currentlyPickingGoods = false;
                    agent.shelfRouteChosen = false;
                    agent.shelfRouteReached = false;
                    agent.confusedStateAllowed = true;

                    agent.C_Functions.ChooseShelfRoute(agent);

                    return NodeState.SUCCESS;
                }
                return NodeState.RUNNING;

        }

        return NodeState.RUNNING;

    }

    private void RotateTowardsShelf(CustomerManager agent)
    {
        Shelf chosenShelf = agent.shelfIDPairs[agent.currentChosenGood.boxID];
        Vector3 direction = (chosenShelf.transform.position - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed/60));
    }

    private void PickGoodsFromShelf(CustomerManager agent)
    {

        Shelf chosenShelf = agent.shelfIDPairs[agent.currentChosenGood.boxID];


        if (chosenShelf.remainingStockCount <= 0)
        {
            //Plockanimering
            //agent.nrGoodsFound++;
            agent.goodsGathered.Add(agent.currentChosenGood.placedPrefab);
        }
        agent.remainingGoodsList.Remove(agent.currentChosenGood);
    }
}
