using System.Threading;
using UnityEngine;

public class PatroleAisle : BTNode
{
    public enum Phase { Initiate, WalkToSpot, Idle}
    public Phase phase = Phase.WalkToSpot;
    private float distanceToTarget;
    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.patroleAisle;
        switch (phase)
        {

            case Phase.Initiate:


                agent.C_Functions.SetTimer(Random.Range(agent.minIdleTime, agent.maxIdleTime));

                phase = Phase.WalkToSpot;

                return NodeState.RUNNING;


            case Phase.WalkToSpot:

                agent.animator.SetState(AnimState.Walk);
                agent.navigation.isStopped = false;
                agent.navigation.speed = agent.walkSpeed;
                agent.navigation.SetDestination(agent.chosenAislePos);

                distanceToTarget = Vector3.Distance(agent.transform.position, agent.chosenAislePos);

                if (distanceToTarget < 0.25f)
                {
                    phase = Phase.Idle;
                }
                return NodeState.RUNNING;

            case Phase.Idle:

                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.Idle);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    agent.isCurrentlyPatrolling = false;
                    agent.patroleRouteChosen = false;
                    agent.confusedStateActivated = false;
                    phase = Phase.Initiate;

                    return NodeState.SUCCESS;
                }


                return NodeState.RUNNING;
        }
        return NodeState.RUNNING;
    }
}
