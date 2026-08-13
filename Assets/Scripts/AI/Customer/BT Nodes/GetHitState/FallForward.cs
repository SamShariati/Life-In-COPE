using UnityEngine;

public class FallForward : BTNode
{
    private enum Phase { Initiate, Falling, GettingUp, Idle }
    private Phase phase = Phase.Initiate;
    private float getUpAnimationTime = 1.5f;
    private float idleAnimationTime = 4f;


    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.fallForward;

        switch (phase)
        {
            case Phase.Initiate:

                agent.navigation.isStopped = true;

                agent.C_Functions.SetTimer(agent.gettingStunnedTime);

                phase = Phase.Falling;

                return NodeState.RUNNING;


            case Phase.Falling:

                //agent.C_Functions.RotateOnHitImpact();
                agent.animator.SetState(AnimState.FallForward);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.GettingUp;
                    agent.C_Functions.SetTimer(getUpAnimationTime);

                }

                return NodeState.RUNNING;


            case Phase.GettingUp:

                agent.animator.SetState(AnimState.GetUpBackward);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Idle;
                    agent.C_Functions.SetTimer(idleAnimationTime);
                }
                return NodeState.RUNNING;


            case Phase.Idle:

                agent.animator.SetState(AnimState.Dizzy);
                //agent.C_Functions.RotateTowardsPlayer();

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Initiate;
                    agent.getHitStateActivated = false;
                    agent.isCurrFallingForward = false;

                    return NodeState.SUCCESS;
                }

                return NodeState.RUNNING;

        }
        return NodeState.RUNNING;
    }
}
