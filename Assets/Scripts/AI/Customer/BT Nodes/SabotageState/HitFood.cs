using System.Collections.Generic;
using UnityEngine;

public class HitFood : BTNode
{
    public enum Phase { Initiate, LookAround, HitFood}
    public Phase phase = Phase.Initiate;

    Vector3 foodStallPrefabPos;

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

        switch (phase)
        {
            case Phase.Initiate:

                GameObject key = agent.chosenFoodStall.stallPositionsDictionary[agent.chosenFoodStallPos];
                foodStallPrefabPos = agent.chosenFoodStall.prefabPositionsDictionary[key].position;

                agent.IKRiggingEnabled = false;
                agent.C_Functions.SetTimer(4f);
                phase = Phase.LookAround;
                return NodeState.RUNNING;


            case Phase.LookAround:

                RotateTowardsFoodStallPosition(agent, foodStallPrefabPos);

                if (agent.customerVision.CanSeePlayer())
                {
                    agent.seenPlayerDuringSabotage = true;
                    agent.IKRiggingEnabled = true;
                }


                if (agent.seenPlayerDuringSabotage)
                {
                    agent.animator.SetState(AnimState.Idle);
                }
                else
                {
                    agent.animator.SetState(AnimState.LookAround);
                }


                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {

                    if (agent.seenPlayerDuringSabotage)
                    {
                        if (RollSabotageChance(agent))
                        {
                            phase = Phase.HitFood;
                            return NodeState.RUNNING;
                        }
                        else
                        {
                            phase = Phase.Initiate;
                            agent.isCurrentlyHittingStall = false;
                            agent.isCurrentlyTaunting = true;

                            return NodeState.SUCCESS;
                        }
                    }
                    else
                    {
                        phase = Phase.HitFood;
                        return NodeState.RUNNING;
                    }
                }

                return NodeState.RUNNING;


            case Phase.HitFood:

                agent.animator.SetState(AnimState.Attack);
                agent.recentlyHitFood = true;
                agent.chosenFoodStall.SpawnFood(agent.chosenFoodStallPos, 3);

                if (agent.animator.IsStateFinished("Attack"))
                {
                    agent.isCurrentlyHittingStall = false;
                    agent.isCurrentlyTaunting = true;
                    phase = Phase.Initiate;

                    return NodeState.SUCCESS;
                }

                return NodeState.RUNNING;

        }
        
        return NodeState.SUCCESS;
    }


    private bool RollSabotageChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0f, 100f);
        agent.C_Functions.ResetSabotageStateCD();

        if (randPercent <= agent.sabotageChance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void RotateTowardsFoodStallPosition(CustomerManager agent, Vector3 foodStallPos)
    {

        Vector3 direction = (foodStallPos - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
