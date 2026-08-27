using UnityEngine;

public class PickGoods : BTNode
{

    private enum Phase { WaitingToPick, Picking, WaitingToExit}
    private Phase phase = Phase.WaitingToPick;

    private float pickAnimationTime = 1.5f;
    private float currentAnimationTime = 1.5f;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.pickGoods;

        
        RotateTowardsShelf(agent);

        switch (phase)
        {
            case Phase.WaitingToPick:

                agent.animator.SetState(AnimState.Idle);
                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    
                    agent.C_Functions.SetTimer(agent.minIdleTime);
                    phase = Phase.Picking;
                }
                return NodeState.RUNNING;


            case Phase.Picking:

                agent.animator.SetState(AnimState.GrabItem);
                phase = Phase.WaitingToExit;
                return NodeState.RUNNING;


            case Phase.WaitingToExit:

                CheckGrabAnimation(agent);
                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    currentAnimationTime = pickAnimationTime;

                    agent.allowedToChase = true;
                    PickGoodsFromShelf(agent); //Failsafe in case we don't enter ConfusedState right after.
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
    private void CheckGrabAnimation(CustomerManager agent)
    {
        currentAnimationTime -= Time.deltaTime;
        if (currentAnimationTime < 0)
        {
            agent.animator.SetState(AnimState.Idle);
        }
    }

    private void RotateTowardsShelf(CustomerManager agent)
    {
        Shelf chosenShelf = agent.allShelfPositions[agent.currentChosenGood.boxID];
        Vector3 direction = (chosenShelf.transform.position - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed/60));
    }

    private void PickGoodsFromShelf(CustomerManager agent)
    {

        Shelf chosenShelf = agent.allShelfPositions[agent.currentChosenGood.boxID];


        if (chosenShelf.remainingGoodsToStock <= 0)
        {
            
            //agent.nrGoodsFound++;
            agent.goodsGathered.Add(agent.currentChosenGood.placedPrefab);
        }
        agent.remainingGoodsList.Remove(agent.currentChosenGood);
    }
}
