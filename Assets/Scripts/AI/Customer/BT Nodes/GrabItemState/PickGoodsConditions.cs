using System.Data;
using UnityEngine;

public class PickGoodsConditions : CustomerBTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        // Only enter picking phase after the shelf has been reached
        if (!agent.BB.GSI.shelfRouteReached)
        {
            return NodeState.FAILURE;
        }
            

        if (!agent.BB.GSI.currentlyPickingGoods)
        {
            agent.BB.IKRigging.IKRiggingEnabled = false;
            agent.BB.SFP.allowedToChase = false;
            agent.BB.GSI.currentlyPickingGoods = true;
            agent.C_Functions.SetTimer(3f);
        }

        return NodeState.SUCCESS;
    }
}
