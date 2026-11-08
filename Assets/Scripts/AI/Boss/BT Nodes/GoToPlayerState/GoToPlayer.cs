using JetBrains.Annotations;
using UnityEngine;

public class GoToPlayer : BossBTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(BossManager agent)
    {

        agent.Functions.SetPlayerDestination();
        ChooseAnimation(agent);
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.player.position);
        Debug.Log(distanceToTarget);

        if (distanceToTarget < 1.5f)
        {
            agent.navigation.isStopped = true;
            agent.BB.StoreNav.reachedPlayerDestination = true;
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }

    private void ChooseAnimation(BossManager agent)
    {
        if (agent.navigation.speed == agent.walkSpeed)
        {
            agent.animator.SetState(AnimState.Walk);
        }
        else if (agent.navigation.speed == agent.runSpeed)
        {
            //animation
        }
        else if (agent.navigation.speed == agent.sprintSpeed)
        {
            //animation
        }
    }
}
