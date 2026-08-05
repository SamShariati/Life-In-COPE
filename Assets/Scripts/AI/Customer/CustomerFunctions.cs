using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerFunctions
{
    CustomerManager agent;

    private GameObject shelfObjectParent;
    private GameObject palletObject;
    private GameObject cashRegisterObject;
    private List<CardboardBoxData> palletDataList;
    private GameObject aisles;

    public float idleTimer = 0f;

    private const float destinationBuffer = 0.75f;
    private const float sampleSearchRadius = 3f;

    private float lastChaseCooldown;

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
        lastChaseCooldown = -agent.chaseCooldown;
    }

    public void CalculatePlayerDestination(CustomerManager agent)
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

    public void ResetFlagVariables()
    {
        //Shelf State
        agent.shelfRouteReached = false;
        agent.currentlyPickingGoods = false;

        //ConfusedState
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

    //Generalized Timer---------------------
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

    public bool CheckSearchForPlayerStateCD()
    {
        if (Time.time > lastChaseCooldown + agent.chaseCooldown)
        {
            lastChaseCooldown = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public void StartSearchForPlayerStateCD()
    {
        lastChaseCooldown = Time.time;
    }

    //--------------------------------------
}
