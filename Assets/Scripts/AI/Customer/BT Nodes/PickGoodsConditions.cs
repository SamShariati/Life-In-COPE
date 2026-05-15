using System.Data;
using UnityEngine;

public class PickGoodsConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        if (!agent.currentlyPickingGoods)
        {
            agent.currentlyPickingGoods = true;
            return NodeState.SUCCESS;
        }
        else if (agent.currentlyPickingGoods)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
}
