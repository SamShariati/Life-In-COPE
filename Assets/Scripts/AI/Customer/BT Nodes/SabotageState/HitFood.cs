using UnityEngine;

public class HitFood : BTNode
{


    public override NodeState Evaluate(CustomerManager agent)
    {
        //LOOK AROUND PHASE:
        //  Look around animation is played to its full, with ik-rigging disabled. 2 different paths occur now:

        //  A. Agent sees player during the phase. Agent now immediately swaps to an idle animation and ik-rigging is activated. (SeenPlayerDuringSabotage = true)
        //  Agent now rolls a SabotageChance once again. If it's true it switches to the HitFood phase. 

        //  B. Agent doesn't detect the player. Agent now prepares to switch to the HitFood phase.

        //  4-5 seconds total duration in this phase.

        //HIT-FOOD PHASE:

        //  RecentlyHitFood = true
        //  Immediately hits the food of the chosen position, and SpawnFood() is activated. Phase is active during animation + 0.5-1s. If SPDS = true, enable head ik-rigging.
        //  If SPDS = false;, keep head ik-rigging disabled.


        
        return NodeState.SUCCESS;
    }
}
