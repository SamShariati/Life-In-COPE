using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerFunctions
{
    CustomerManager agent;

    public float idleTimer = 0f;

    private const float destinationBuffer = 0.75f;
    private const float sampleSearchRadius = 3f;

    private float lastSearchForPlayerStateCD;
    private float lastGetHitStateCD;

    private float rotateSpeedDegPerSec = 720f;

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
        lastSearchForPlayerStateCD = -agent.searchForPlayerStateCD;
        lastGetHitStateCD = -agent.getHitStateCD;
    }

    public void CalculatePlayerDestination()
    {
        Vector3 rawTarget = agent.player.position;
        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, sampleSearchRadius, NavMesh.AllAreas))
        {
            Vector3 toHit = hit.position - agent.transform.position;
            float distToHit = toHit.magnitude;

            Vector3 destination = hit.position;
            if (distToHit > destinationBuffer)
            {
                destination = hit.position - toHit.normalized * destinationBuffer;

                if (NavMesh.SamplePosition(destination, out NavMeshHit bufferedHit, 1f, NavMesh.AllAreas))
                {
                    destination = bufferedHit.position;
                }
            }

            agent.navigation.SetDestination(destination);
        }
    }

    public void GetHitResetFlags()
    {

        //SearchForPlayer State
        agent.spottedPlayer = false;
        agent.isCurrentlyChasing = false;
        agent.isCurrentlyFollowing = false;
        agent.allowedToChase = true;
        //agent.confusedStateAllowed = true; //osäker på om denna ska finnas

        SearchForPlayerResetFlags();
    }

    public void SearchForPlayerResetFlags()
    {
        //Shelf State
        agent.shelfRouteReached = false;
        agent.currentlyPickingGoods = false;
        agent.pickGoods.phase = PickGoods.Phase.WaitingToPick;

        //Confused State
        agent.confusedStateActivated = false;
        agent.isCurrentlyPatrolling = false;
        agent.patroleRouteChosen = false;
        
        agent.isCurrCheckingWrongShelf = false;
        agent.wrongShelfChosen = false;
        agent.patroleAisle.phase = PatroleAisle.Phase.Initiate;
        agent.checkWrongShelf.phase = CheckWrongShelf.Phase.Initiate;



    }

    public void GetCorrectAisle()
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {

            string shelfGoodsType = shelf.goodsType.ToString();

            if (shelfGoodsType == agent.currentChosenGood.boxID)
            {
                agent.aisleID = (int)shelf.aisle;
            }
        }

    }

    public void ChooseShelfRoute(CustomerManager agent)
    {
        if (agent.remainingGoodsList.Count > 0)
        {
            int rand = Random.Range(0, agent.remainingGoodsList.Count);
            agent.currentChosenGood = agent.remainingGoodsList[rand];

            agent.chosenShelfPosition = agent.allShelfArrowPositions[agent.currentChosenGood.boxID];
        }
        else
        {
            agent.SwitchState(agent.goToLineState);
            agent.BTActivated = false;
        }
    }

    public void CalculateImpactRotation()
    {

        Vector3 dir = agent.dotProduct < 0f ? -agent.thrownDirection : agent.thrownDirection;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            dir = agent.transform.forward; // fallback, e.g. near-vertical throw

        agent.impactTargetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }   

    public void RotateOnHitImpact()
    {
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
            agent.impactTargetRotation, rotateSpeedDegPerSec * Time.deltaTime);
    }


    public void RotateTowardsPlayer()
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

    //--------------GENERAL TIMERS---------------------
    public bool TickTimer(float delta)
    {
        idleTimer -= delta;
        return idleTimer <= 0;
    }
    public void SetTimer(float duration)
    {
        idleTimer = duration;
    }

    public void ResetTimer()
    {
        idleTimer = 0;
    }

    //------------BEHAVIOUR COOLDOWN TIMERS--------------

    //-------------SearchForPlayer State--------------
    public bool CheckSearchForPlayerStateCD()
    {
        if (Time.time > lastSearchForPlayerStateCD + agent.searchForPlayerStateCD)
        {
            lastSearchForPlayerStateCD = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }
 
    public void StartSearchForPlayerStateCD()
    {
        lastSearchForPlayerStateCD = Time.time;
    }

    //-------------GetHit State--------------------------

    public bool CheckGetHitStateCD()
    {
        if (Time.time > lastGetHitStateCD + agent.getHitStateCD)
        {
            lastGetHitStateCD = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartGetHitStateCD()
    {
        lastGetHitStateCD = Time.time;
    }
}
