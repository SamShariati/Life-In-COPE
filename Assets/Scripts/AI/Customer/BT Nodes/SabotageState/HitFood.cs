using System.Collections.Generic;
using UnityEngine;

public class HitFood : CustomerBTNode
{
    public enum Phase { Initiate, LookAround, Idle, HitFood}
    public Phase phase = Phase.Initiate;
    private bool runOnce = false;

    Vector3 foodStallPrefabPos;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.hitFood;



        switch (phase)
        {
            case Phase.Initiate:

                GameObject key = agent.BB.Sabotage.chosenFoodStall.stallPositionsDictionary[agent.BB.Sabotage.chosenFoodStallPos];
                foodStallPrefabPos = agent.BB.Sabotage.chosenFoodStall.prefabPositionsDictionary[key].position;

                agent.BB.IKRigging.IKRiggingEnabled = false;
                agent.Functions.SetTimer(5f);
                phase = Phase.LookAround;
                runOnce = false;

                return NodeState.RUNNING;



                 
            case Phase.LookAround:

                RotateTowardsFoodStallPosition(agent, foodStallPrefabPos);

                if (agent.BB.SFP.customerVision.CanSeePlayer())
                {
                    agent.BB.Sabotage.seenPlayerDuringSabotage = true;
                    agent.BB.IKRigging.IKRiggingEnabled = true;
                }


                if (agent.BB.Sabotage.seenPlayerDuringSabotage)
                {
                    agent.animator.SetState(AnimState.Idle);
                }
                else
                {
                    agent.animator.SetState(AnimState.LookAround);
                }


                if (agent.Functions.TickTimer(Time.deltaTime))
                {

                    if (agent.BB.Sabotage.seenPlayerDuringSabotage)
                    {
                        if (RollSabotageChance(agent))
                        {
                            phase = Phase.HitFood;
                            agent.Functions.SetTimer(1f);

                            return NodeState.RUNNING;
                        }
                        else
                        {
                            phase = Phase.Initiate;
                            agent.BB.Sabotage.isCurrentlyHittingStall = false;
                            agent.BB.Sabotage.isCurrentlyTaunting = true;

                            return NodeState.SUCCESS;
                        }
                    }
                    else
                    {
                        phase = Phase.HitFood;
                        agent.Functions.SetTimer(1f);
                        return NodeState.RUNNING;
                    }
                }

                return NodeState.RUNNING;



            case Phase.Idle:

                agent.animator.SetState(AnimState.Idle);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.HitFood;
                }
                return NodeState.RUNNING;


            case Phase.HitFood:


                agent.BB.Sabotage.recentlyHitFood = true;

                if (!runOnce)
                {
                    agent.animator.SetState(AnimState.Attack);
                    runOnce = true;
                    agent.BB.Sabotage.chosenFoodStall.SpawnFood(agent.BB.Sabotage.chosenFoodStallPos, agent.sabotageLevel);
                    BossDataTracker.Instance.nrTrashOnGround += agent.sabotageLevel;

                }

                if (agent.animator.IsStateFinished("Attack"))
                {
                    agent.BB.Sabotage.isCurrentlyHittingStall = false;
                    agent.BB.Sabotage.isCurrentlyTaunting = true;
                    runOnce = false;
                    phase = Phase.Initiate;

                    //Prepare an idle time for TauntPlayer
                    agent.Functions.SetTimer(1f);

                    return NodeState.SUCCESS;
                }


                return NodeState.RUNNING;

        }
        
        return NodeState.SUCCESS;
    }


    private bool RollSabotageChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0f, 100f);
        agent.Functions.ResetSabotageStateCD();

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

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 120));
    }
}
