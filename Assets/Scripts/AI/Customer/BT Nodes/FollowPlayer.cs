using UnityEngine;

public class FollowPlayer : BTNode
{

    float distanceToTarget;
    public override NodeState Evaluate(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.player.position, agent.transform.position);

        agent.navigation.speed = agent.playerMovement.currentSpeed;

        agent.C_Functions.CalculateDestination(agent);

        if (distanceToTarget < 2.5f)
        {
            agent.navigation.isStopped = true;
            agent.animator.SetState(AnimState.Idle);
            RotateTowardsPlayer(agent);


        }
        else
        {
            agent.navigation.isStopped = false;
            if (agent.playerMovement.currentSpeed == agent.playerMovement.walkSpeed)
            {
                agent.animator.SetState(AnimState.Walk);
            }
            else
            {
                {
                    agent.animator.SetState(AnimState.Chase);
                }
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
