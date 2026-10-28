using UnityEngine;

public class GrabItemConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {

        if (agent.shelfStateAllowed)
        {
            agent.sabotageStateAllowed = false;
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
}
