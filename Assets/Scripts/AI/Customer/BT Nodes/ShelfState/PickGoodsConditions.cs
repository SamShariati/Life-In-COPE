using System.Data;
using UnityEngine;

public class PickGoodsConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        // Only enter picking phase after the shelf has been reached
        if (!agent.shelfRouteReached)
        {
            return NodeState.FAILURE;
        }
            

        if (!agent.currentlyPickingGoods)
        {
            agent.allowedToChase = false;
            agent.currentlyPickingGoods = true;
            agent.C_Functions.ResetTimer();
        }

        return NodeState.SUCCESS;
    }
}
