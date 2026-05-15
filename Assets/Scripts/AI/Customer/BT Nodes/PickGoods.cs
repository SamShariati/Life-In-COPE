using UnityEngine;

public class PickGoods : BTNode
{
    bool runOnce = false;
    float idleTimeLeft;
    float timerUntilExit;
    public override NodeState Evaluate(CustomerManager agent)
    {
        RotateTowardsShelf(agent);

        if (!runOnce)
        {
            idleTimeLeft = agent.maxIdleTime;
            timerUntilExit = 3f; //Måste ändras inga magiska nummer
        }

        idleTimeLeft -= Time.deltaTime;

        if (idleTimeLeft < 0)
        {
            PickGoodsFromShelf(agent);
        }


        return NodeState.RUNNING;

    }

    private void RotateTowardsShelf(CustomerManager agent)
    {
        Shelf chosenShelf = agent.shelfIDPairs[agent.currentChosenGood.boxID];
        Vector3 direction = (chosenShelf.transform.position - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.navigation.angularSpeed);
    }

    private void PickGoodsFromShelf(CustomerManager agent)
    {
        Shelf chosenShelf = agent.shelfIDPairs[agent.currentChosenGood.boxID];

        if (chosenShelf.remainingStockCount <= 0)
        {
            //Plockanimering
            agent.nrGoodsFound++;
        }
    }
}
