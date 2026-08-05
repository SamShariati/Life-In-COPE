using UnityEngine;

public class FollowPlayer : BTNode
{

    float distanceToPlayer;
    float distanceToChosenArrow;
    Vector3 arrowPosition;
    private enum Phase { Instansiate, RotatePlayer, IdleTime, FollowPlayer}
    private Phase phase = Phase.Instansiate;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.followPlayer;

        switch (phase)
        {

            case Phase.Instansiate:

                agent.C_Functions.ChooseShelfRoute(agent); //Kan bli en framtid bug. Möjligt att BTActivated bli false?
                agent.C_Functions.SetTimer(agent.WasteCustomerTime);
                PlayerState.Instance.CaughtPlayer(agent.headObject);

                phase = Phase.RotatePlayer;

                return NodeState.RUNNING;



            case Phase.RotatePlayer:

               

                distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);

                if (distanceToPlayer < 1f)
                {
                    agent.navigation.isStopped = true;
                    agent.animator.SetState(AnimState.CaughtPlayer);
                    RotateTowardsPlayer(agent);
                }

                if (PlayerState.Instance._activeCaught.isFacingTarget)
                {
                    phase = Phase.IdleTime;
                    arrowPosition = ShelfManager.Instance.GetArrowPosition(agent.currentChosenGood.boxID);
                    ShelfManager.Instance.DisableShelfArrow();
                }
                return NodeState.RUNNING;


            case Phase.IdleTime:

                CustomerDialogue.Instance.ShowBubble();

                distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);

                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.CaughtPlayer);
                RotateTowardsPlayer(agent);

                if (agent.C_Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.FollowPlayer;
                    ShelfManager.Instance.EnableShelfArrow(agent.currentChosenGood.boxID);
                    PlayerState.Instance.ReleasePlayer();
                }
                return NodeState.RUNNING;


            case Phase.FollowPlayer:

                CustomerDialogue.Instance.ShowMessage(agent.currentChosenGood.boxID);

                distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);
                distanceToChosenArrow = Vector3.Distance(agent.player.position, arrowPosition);

                agent.navigation.speed = agent.playerMovement.currentSpeed;

                agent.C_Functions.CalculatePlayerDestination(agent);

                SetAnimation(agent);

                if (distanceToChosenArrow < 1.5f)
                {
                    PlayerState.Instance.currentlyBeingFollowed = false;
                    ShelfManager.Instance.DisableShelfArrow();
                    CustomerDialogue.Instance.HideMessage();
                    agent.isCurrentlyFollowing = false;
                    agent.isCurrentlyStaring = true;
                    phase = Phase.Instansiate;

                    if (PlayerInventory.Instance.IsHoldingBox())
                    {
                        ShelfManager.Instance.EnableShelfArrow(PlayerInventory.Instance.GetHeldBoxID());
                    }
   

                    return NodeState.SUCCESS;
                }
                else
                {
                    return NodeState.RUNNING;
                }

        }
        return NodeState.RUNNING;



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
        Vector3 toPlayer = agent.player.position - agent.transform.position;
        Vector3 flatDirection = new Vector3(toPlayer.x, 0, toPlayer.z);

        // Skip rotation if the horizontal distance is (near) zero
        if (flatDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(flatDirection.normalized);
        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            lookRotation,
            Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
