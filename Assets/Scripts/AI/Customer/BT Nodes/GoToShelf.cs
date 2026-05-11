using UnityEngine;

public class GoToShelf : BTNode
{
    bool runOnce = false;
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (!runOnce)
        {
            runOnce = true;
            Debug.Log("Entered GoToShelf");
        }

        return NodeState.RUNNING;
        
    }
}
