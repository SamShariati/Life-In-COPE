using UnityEngine;

public class TauntPlayer : BTNode
{
    public enum Phase { Idle, Initiate, Act}
    public enum Scenario { A, B, C}

    public Phase phase = Phase.Initiate;
    public Scenario scenario = Scenario.A;


    public override NodeState Evaluate(CustomerManager agent)
    {


        agent.currentBehavior = CustomerManager.CurrentBehaviour.tauntPlayer;
        //  Three different branches:

        //  A. If SPDS = true and RHF = true, ik-rigging is disabled and agent rotates towards player, and swaps to taunting animation.
        //  B. If SPDS = false and RHF = true, ik-rigging is disabled and swaps to pleased animation.
        //  C. If SPDS = true and RHF = false, ik-rigging is enabled and swaps to idle animation.

        //  Total State duration 3-4seconds. SPDS, RHF = false

        switch (phase)
        {

            case Phase.Idle:

                agent.animator.SetState(AnimState.Idle);


                //Timer for this statement is set in HitFood.
                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Initiate;
                }
                return NodeState.RUNNING;


            case Phase.Initiate:

                if (agent.BB.Sabotage.seenPlayerDuringSabotage && agent.BB.Sabotage.recentlyHitFood)
                {
                    scenario = Scenario.A;
                    agent.C_Functions.SetTimer(4f);
                }
                else if (!agent.BB.Sabotage.seenPlayerDuringSabotage && agent.BB.Sabotage.recentlyHitFood)
                {
                    scenario = Scenario.B;
                    agent.C_Functions.SetTimer(3f);
                }
                else
                {

                    agent.C_Functions.SetTimer(Random.Range(agent.minIdleTime, agent.maxIdleTime));
                    scenario = Scenario.C;
                }

                phase = Phase.Act;
                

                return NodeState.RUNNING;


            case Phase.Act:


                if (scenario == Scenario.A)
                {
                    agent.C_Functions.RotateTowardsPlayer();
                    agent.animator.SetState(AnimState.Taunt);
                }
                else if (scenario == Scenario.B)
                {
                    agent.animator.SetState(AnimState.Pleased);
                }
                else if (scenario == Scenario.C)
                {
                    agent.animator.SetState(AnimState.Idle);
                }


                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    agent.BB.Sabotage.sabotageStateActivated = false;
                    agent.BB.Sabotage.recentlyHitFood = false;
                    agent.BB.Sabotage.seenPlayerDuringSabotage = false;
                    agent.BB.Sabotage.isCurrentlyTaunting = false;
                    phase = Phase.Initiate;

                    return NodeState.SUCCESS;
                }

                return NodeState.RUNNING;

        }
        return NodeState.RUNNING;

    }


}
