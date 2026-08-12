using UnityEngine;

public class FallBackward : BTNode
{

    private enum Phase {  Initiate, Falling, GettingUp, Idle}
    private Phase phase = Phase.Initiate;
    private float getUpAnimationTime = 1.5f;


    public override NodeState Evaluate(CustomerManager agent)
    {
        
        switch (phase)
        {
            case Phase.Initiate:

                agent.navigation.isStopped = true;

                agent.C_Functions.SetTimer(agent.gettingStunnedTime);

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
                    phase = Phase.Idle;
                    agent.C_Functions.SetTimer(agent.minIdleTime);
                }
                return NodeState.RUNNING;


            case Phase.Idle:

                agent.animator.SetState(AnimState.Idle);
                agent.C_Functions.RotateTowardsPlayer();

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Initiate;
                    agent.getHitStateActivated = false;
                    agent.isCurrFallingBackward = false;

                    return NodeState.SUCCESS;
                }

                return NodeState.RUNNING;

        }
        return NodeState.RUNNING;
    }
}
