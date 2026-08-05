using UnityEngine;

public class CheckWrongShelf : BTNode
{
    public enum Phase { Initiate, WalkingToShelf, InspectingShelf}
    public Phase phase = Phase.Initiate;
    float distanceTarget;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.checkWrongShelf;

        switch (phase)
        {

            case Phase.Initiate:

                agent.C_Functions.SetTimer(Random.Range(agent.minIdleTime, agent.maxIdleTime));

                phase = Phase.WalkingToShelf;

                return NodeState.RUNNING;


            case Phase.WalkingToShelf:

                agent.animator.SetState(AnimState.Walk);

                agent.navigation.speed = agent.walkSpeed;
                agent.navigation.isStopped = false;
                agent.navigation.SetDestination(agent.wrongShelfArrowPos);

                distanceTarget = Vector3.Distance(agent.transform.position, agent.wrongShelfArrowPos);

                if (distanceTarget < 0.2f)
                {
                    phase = Phase.InspectingShelf;
                    
                }

                return NodeState.RUNNING;

            case Phase.InspectingShelf:

                RotateTowardsWrongShelf(agent);
                agent.animator.SetState(AnimState.Idle);
                agent.navigation.isStopped = true;

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    agent.isCurrCheckingWrongShelf = false;
                    agent.wrongShelfChosen = false;
                    agent.confusedStateActivated = false;
                    phase = Phase.Initiate;

                    return NodeState.SUCCESS;
                }

                return NodeState.RUNNING;
        }

        return NodeState.RUNNING;
    }

    private void RotateTowardsWrongShelf(CustomerManager agent)
    {
        
        Vector3 direction = (agent.currentWrongShelf.transform.position - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
