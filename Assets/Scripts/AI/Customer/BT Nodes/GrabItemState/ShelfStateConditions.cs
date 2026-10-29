using UnityEngine;

public class ShelfStateConditions : BTNode
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
