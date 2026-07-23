using UnityEngine;

public class IdleStare : BTNode
{

    private enum Phase { initiate, idle}
    private Phase phase = Phase.initiate;


    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.idleStare;

        switch (phase)
        {
            case Phase.initiate:

                agent.C_Functions.SetTimer(5);
                phase = Phase.idle;

                return NodeState.RUNNING;


            case Phase.idle:

                RotateTowardsPlayer(agent);
                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.Idle);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.initiate;
                    agent.isCurrentlyStaring = false;
                    agent.playerSpotted = false;
                    agent.C_Functions.StartSearchCooldown();
                    return NodeState.SUCCESS;
                }
                else
                {
                    return NodeState.RUNNING;
                }
        }
        return NodeState.RUNNING;

    }

    private void RotateTowardsPlayer(CustomerManager agent)
    {
        Vector3 playerPosition = agent.player.position;
        Vector3 direction = (playerPosition - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
