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
    //private float lastGetHitStateCD;
    private float lastSabotageStateCD;

    private float rotateSpeedDegPerSec = 720f;

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
        lastSearchForPlayerStateCD = -agent.searchForPlayerStateCD;
        //lastGetHitStateCD = -agent.getHitStateCD;
        lastSabotageStateCD = -agent.sabotageStateCD;
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

        //Sabotage State
        agent.BB.Sabotage.sabotageStateActivated = false;
        agent.BB.Sabotage.sabotageStateAllowed = true;
        agent.BB.Sabotage.recentlyHitFood = false;
        agent.BB.Sabotage.isCurrentlyGoingToStall = false;
        agent.BB.Sabotage.isCurrentlyHittingStall = false;
        agent.BB.Sabotage.isCurrentlyTaunting = false;
        agent.BB.Sabotage.seenPlayerDuringSabotage = false;
        agent.AI.BT.hitFood.phase = HitFood.Phase.Initiate;
        agent.AI.BT.tauntPlayer.phase = TauntPlayer.Phase.Idle;



        //SearchForPlayer State
        agent.BB.SFP.spottedPlayer = false;
        agent.BB.SFP.isCurrentlyChasing = false;
        agent.BB.SFP.isCurrentlyFollowing = false;
        agent.BB.SFP.isCurrentlyStaring = false;
        agent.AI.BT.idleStare.phase = IdleStare.Phase.initiate;
        agent.BB.SFP.allowedToChase = true;

        

        SearchForPlayerResetFlags();
    }

    public void SearchForPlayerResetFlags()
    {
        //Shelf State
        agent.BB.GSI.shelfRouteReached = false;
        agent.BB.GSI.currentlyPickingGoods = false;
        agent.AI.BT.pickGoods.phase = PickGoods.Phase.WaitingToPick;

        //Confused State
        agent.BB.Confused.confusedStateActivated = false;
        agent.BB.Confused.isCurrentlyPatrolling = false;
        agent.BB.Confused.patroleRouteChosen = false;
        
        agent.BB.Confused.isCurrCheckingWrongShelf = false;
        agent.BB.Confused.wrongShelfChosen = false;
        agent.AI.BT.patroleAisle.phase = PatroleAisle.Phase.Initiate;
        agent.AI.BT.checkWrongShelf.phase = CheckWrongShelf.Phase.Initiate;



    }

    public void GetCorrectAisle()
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {

            string shelfGoodsType = shelf.goodsType.ToString();

            if (shelfGoodsType == agent.BB.GSI.currentChosenGood.boxID)
            {
                agent.BB.Confused.aisleID = (int)shelf.aisle;
            }
        }

    }

    public void ChooseShelfRoute(CustomerManager agent)
    {
        if (agent.BB.GSI.remainingGoodsList.Count > 0)
        {
            int rand = Random.Range(0, agent.BB.GSI.remainingGoodsList.Count);
            agent.BB.GSI.currentChosenGood = agent.BB.GSI.remainingGoodsList[rand];

            agent.BB.GSI.chosenShelfPosition = agent.BB.GSI.allShelfArrowPositions[agent.BB.GSI.currentChosenGood.boxID];
        }
        else
        {
            agent.SwitchState(agent.AI.FSM.goToLineState);
            agent.AI.BTActivated = false;
        }
    }

    public void CalculateImpactRotation()
    {

        Vector3 dir = agent.BB.GetHit.dotProduct < 0f ? -agent.BB.GetHit.thrownDirection : agent.BB.GetHit.thrownDirection;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            dir = agent.transform.forward; // fallback, e.g. near-vertical throw

        agent.BB.GetHit.impactTargetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }   

    public void RotateOnHitImpact()
    {
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
            agent.BB.GetHit.impactTargetRotation, rotateSpeedDegPerSec * Time.deltaTime);
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
            Time.deltaTime * (agent.navigation.angularSpeed / 120));
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

            return true;
        }
        else
        {
            return false;
        }
    }
 
    public void ResetSearchForPlayerStateCD()
    {
        lastSearchForPlayerStateCD = Time.time;
    }


    //-------------Sabotage State--------------------------

    public bool CheckSabotageCD()
    {
        if (Time.time > lastSabotageStateCD + agent.sabotageStateCD)
        {

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetSabotageStateCD()
    {
        lastSabotageStateCD = Time.time;
    }
}
