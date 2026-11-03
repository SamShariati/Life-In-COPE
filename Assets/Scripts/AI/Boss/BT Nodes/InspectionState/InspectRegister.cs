using UnityEngine;

public class InspectRegister : BossBTNode
{

    public enum Phase { Instantiate, GoToRegister, Inspect, ResultAnimation, Exit}
    public Phase phase = Phase.Instantiate;
    Vector3 destination;
    float distanceToTarget;


    public override NodeState Evaluate(BossManager agent)
    {
        switch (phase)
        {

            case Phase.Instantiate:


                agent.Functions.SetTimer(4f);
                destination = agent.BB.Store.inspectRegisterPos.position;
                phase = Phase.GoToRegister;
                return NodeState.RUNNING;


            case Phase.GoToRegister:


                agent.navigation.SetDestination(destination);
                agent.navigation.speed = agent.walkSpeed;
                agent.navigation.isStopped = false;
                distanceToTarget = Vector3.Distance(agent.transform.position, destination);
                agent.animator.SetState(AnimState.Walk);

                if (distanceToTarget < 0.2f)
                {                    
                    phase = Phase.Inspect;
                }
                return NodeState.RUNNING;


            case Phase.Inspect:

                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.Inspect);
                agent.Functions.RotateTowardsTarget(agent.BB.Store.inspectRegisterRotPos.position);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.ResultAnimation;

                }
                return NodeState.RUNNING;


            case Phase.ResultAnimation:

                if (BossDataTracker.Instance.nrCustomersInLine != 0)
                {
                    agent.animator.SetState(AnimState.Dissappoint);
                }
                else
                {
                    agent.animator.SetState(AnimState.Pleased);
                }

                if (agent.animator.IsStateFinished("Dissappoint") || agent.animator.IsStateFinished("Pleased"))
                {
                    phase = Phase.Exit;
                }
                return NodeState.RUNNING;


            case Phase.Exit:

                agent.BB.Inspection.stateActivated = false;
                agent.BB.Inspection.isCurrInspectingRegister = false;
                phase = Phase.Instantiate;

                return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;

    }

}
