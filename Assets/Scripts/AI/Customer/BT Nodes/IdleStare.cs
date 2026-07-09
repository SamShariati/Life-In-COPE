using UnityEngine;

public class IdleStare : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        RotateTowardsPlayer(agent);

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
