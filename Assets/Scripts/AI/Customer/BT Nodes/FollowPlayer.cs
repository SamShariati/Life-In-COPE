using UnityEngine;

public class FollowPlayer : BTNode
{

    float distanceToPlayer;
    float distanceToChosenArrow;
    bool runOnce = false;
    Vector3 arrowPosition;

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (!runOnce)
        {
            runOnce = true;
            CustomerDialogue.Instance.ShowMessage(agent.currentChosenGood.boxID);
            arrowPosition = ShelfManager.Instance.GetArrowPosition(agent.currentChosenGood.boxID);
            ShelfManager.Instance.DisableShelfArrow();
            ShelfManager.Instance.EnableShelfArrow(agent.currentChosenGood.boxID);
            Debug.Log(arrowPosition);
        }

        distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);
        distanceToChosenArrow = Vector3.Distance(agent.player.position, arrowPosition);
        Debug.Log(distanceToChosenArrow);

        agent.navigation.speed = agent.playerMovement.currentSpeed;

        agent.C_Functions.CalculateDestination(agent);

        SetAnimation(agent);

        if (distanceToChosenArrow < 1.5f)
        {
            PlayerState.Instance.currentlyBeingFollowed = false;
            ShelfManager.Instance.DisableShelfArrow();
            CustomerDialogue.Instance.HideMessage();
            agent.isCurrentlyFollowing = false;
            agent.isCurrentlyStaring = true;
            runOnce = false;

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.RUNNING;
        }

    }


    private void SetAnimation(CustomerManager agent)
    {
        if (distanceToPlayer < 2.5f)
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
    }


    private void RotateTowardsPlayer(CustomerManager agent)
    {
        Vector3 playerPosition = agent.player.position;
        Vector3 direction = (playerPosition - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
