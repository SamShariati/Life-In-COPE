using UnityEngine;

public class FallBackward : BTNode
{

    private enum Phase {  Initiate, Falling, GettingUp, Dizzy}
    private Phase phase = Phase.Initiate;
    private float getUpAnimationTime = 1.5f;
    private float idleAnimationTime = 4f;


    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.fallBackward;

        switch (phase)
        {
            case Phase.Initiate:

                agent.navigation.isStopped = true;
                agent.C_Functions.SetTimer(agent.gettingStunnedTime);
                agent.C_Functions.CalculateImpactRotation();

                phase = Phase.Falling;

                return NodeState.RUNNING;

            
            case Phase.Falling:

                agent.C_Functions.RotateOnHitImpact();
                agent.animator.SetState(AnimState.FallBackward);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.GettingUp;
                    agent.C_Functions.SetTimer(getUpAnimationTime);

                }

                return NodeState.RUNNING;


            case Phase.GettingUp:

                agent.animator.SetState(AnimState.GetUpForward);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Dizzy;
                    agent.C_Functions.SetTimer(idleAnimationTime);
                }
                return NodeState.RUNNING;


            case Phase.Dizzy:

                agent.animator.SetState(AnimState.Dizzy);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
          
                    agent.getHitStateActivated = false;
                    agent.isCurrFallingBackward = false;
                    agent.gotHitByBox = false;
                    agent.idleStateAllowed = true;

                    phase = Phase.Initiate;
                    return NodeState.SUCCESS;

                }

                return NodeState.RUNNING;

            


        }
        return NodeState.RUNNING;
    }
}
