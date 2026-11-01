using UnityEngine;

public class ShelfStateConditions : CustomerBTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (agent.BB.GSI.shelfStateAllowed)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
}
