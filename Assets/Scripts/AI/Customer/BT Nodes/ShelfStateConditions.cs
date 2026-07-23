using UnityEngine;

public class ShelfStateConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (agent.shelfStateAllowed)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
}
