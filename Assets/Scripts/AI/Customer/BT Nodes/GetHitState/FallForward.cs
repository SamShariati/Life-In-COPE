using UnityEngine;

public class FallForward : CustomerBTNode
{
    private enum Phase { Initiate, Falling, GettingUp, Dizzy, Idle }
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
                agent.Functions.SetTimer(agent.gettingStunnedTime);
                agent.Functions.CalculateImpactRotation();

                phase = Phase.Falling;

                return NodeState.RUNNING;


            case Phase.Falling:

                agent.Functions.RotateOnHitImpact();
                agent.animator.SetState(AnimState.FallForward);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.GettingUp;
                    agent.Functions.SetTimer(getUpAnimationTime);

                }

                return NodeState.RUNNING;


            case Phase.GettingUp:

                agent.animator.SetState(AnimState.GetUpBackward);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.Dizzy;
                    agent.Functions.SetTimer(idleAnimationTime);
                }
                return NodeState.RUNNING;


            case Phase.Dizzy:

                agent.animator.SetState(AnimState.Dizzy);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {

                    agent.BB.GetHit.getHitStateActivated = false;
                    agent.BB.GetHit.isCurrFallingForward = false;
                    agent.BB.GetHit.gotHitByBox = false;
                    agent.BB.Idle.idleStateAllowed = true;

                    phase = Phase.Initiate;
                    return NodeState.SUCCESS;

                }

                return NodeState.RUNNING;

        }
        return NodeState.RUNNING;
    }
}
