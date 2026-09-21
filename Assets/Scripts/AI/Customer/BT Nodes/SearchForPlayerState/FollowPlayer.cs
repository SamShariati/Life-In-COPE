using UnityEngine;

public class FollowPlayer : BTNode
{

    float distanceToPlayer;
    float distanceToChosenArrow;
    Vector3 arrowPosition;
    private enum Phase { Instansiate, RotatePlayer, IdleTime, FollowPlayer}
    private Phase phase = Phase.Instansiate;


    // Push away settings
    private const float minPlayerDistance = 1f; // AI gets pushed back until it is this far from the player
    private const float pushSpeed = 3f;           // Minimum push speed (units/sec); player's speed is used if higher

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.followPlayer;
        
        switch (phase)
        {

            case Phase.Instansiate:

                agent.C_Functions.ChooseShelfRoute(agent); //Kan bli en framtid bug. Möjligt att BTActivated blir false?
                agent.C_Functions.SetTimer(agent.wasteCustomerTime);
                PlayerState.Instance.CaughtPlayer(agent.headObject);

                phase = Phase.RotatePlayer;

                return NodeState.RUNNING;



            case Phase.RotatePlayer:

               
                PushAwayFromPlayer(agent);
                distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);

                if (distanceToPlayer < 1f)
                {
                    agent.navigation.isStopped = true;
                    agent.animator.SetState(AnimState.CaughtPlayer);
                    agent.C_Functions.RotateTowardsPlayer();
                }

                if (PlayerState.Instance._activeCaught.isFacingTarget)
                {
                    phase = Phase.IdleTime;
                    arrowPosition = ShelfManager.Instance.GetArrowPosition(agent.currentChosenGood.boxID);
                    ShelfManager.Instance.DisableShelfArrow();
                }
                return NodeState.RUNNING;


            case Phase.IdleTime:

                PushAwayFromPlayer(agent);
                CustomerDialogue.Instance.ShowBubble();

                distanceToPlayer = Vector3.Distance(agent.player.position, agent.transform.position);

                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.CaughtPlayer);
                agent.C_Functions.RotateTowardsPlayer();

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

                agent.C_Functions.CalculatePlayerDestination();

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
            agent.C_Functions.RotateTowardsPlayer();


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


    /// <summary>
    /// Smoothly moves the agent away from the player if they are overlapping.
    /// Uses NavMeshAgent.Move so the agent stays on the NavMesh and it works even while isStopped = true.
    /// </summary>
    private void PushAwayFromPlayer(CustomerManager agent)
    {
        Vector3 away = agent.transform.position - agent.player.position;
        away.y = 0f;

        float dist = away.magnitude;
        if (dist >= minPlayerDistance) return;

        // If perfectly overlapping, fall back to pushing backwards
        away = dist > 0.001f ? away / dist : -agent.transform.forward;

        // Constant speed, but never further than needed to reach the minimum distance (no overshoot/jitter)
        float speed = Mathf.Max(pushSpeed, agent.playerMovement.currentSpeed);
        float step = Mathf.Min(speed * Time.deltaTime, minPlayerDistance - dist);

        agent.navigation.Move(away * step);
    }


    



}
