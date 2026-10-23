using UnityEngine;

public class TauntPlayer : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        //  Three different branches:

        //  1. If SPDS = true and RHF = true, ik-rigging is disabled and agent rotates towards player, and swaps to taunting animation.
        //  2. If SPDS = false and RHF = true, ik-rigging is disabled and swaps to pleased animation.
        //  3. If SPDS = true and RHF = false, ik-rigging is enabled and swaps to idle animation.

        //  Total State duration 3-4seconds. SPDS, RHF = false

        return NodeState.SUCCESS;
    }


}
