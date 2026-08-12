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

    //----------GET HIT STATE--------------------
    private float minMomentumThreshold = 2f;   // momentum below this = negligible reaction
    private float maxMomentumReference = 15f;  // momentum at/above this = full-force reaction
    [Range(0f, 1f)] public float minForceRatio = 0.2f; // ensures even weak hits produce some visible turn

    private float minRotateDuration = 0.1f;  // fastest possible turn (strong hit)
    private float maxRotateDuration = 0.4f;  // slowest possible turn (weak hit)

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
        lastSearchForPlayerStateCD = -agent.SearchForPlayerStateCD;
        lastGetHitStateCD = -agent.GetHitStateCD;
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

    public void CalculateImpactDotProduct()
    {

        Vector3 boxVelocity = agent.collidingBoxRB.linearVelocity;
        boxVelocity.y = 0f;

        Vector3 agentForward = agent.transform.forward;
        agentForward.y = 0f;

        boxVelocity.Normalize();
        agentForward.Normalize();

        agent.dotProduct = Vector3.Dot(boxVelocity, agentForward);

        if (agent.dotProduct < 0) // - är träffad framifrån, + är träffad bakifrån
        {
            agent.isCurrFallingBackward = true;
        }
        else
        {
            agent.isCurrFallingForward = true;
        }
    }

    //Calculate the correct rotation when getting hit with a box.
    public void CalculateRotationAngle()
    {
        Vector3 flatBoxVelocity = agent.collidingBoxRB.linearVelocity;
        flatBoxVelocity.y = 0f;

        Vector3 flatAgentForward = agent.transform.forward;
        flatAgentForward.y = 0f;

        // Directional component: signed angle between agent facing and box direction
        float signedAngle = Vector3.SignedAngle(flatAgentForward, flatBoxVelocity.normalized, Vector3.up);

        float targetFacingAngle;

        if (agent.dotProduct >= 0f)
        {
            // Hit from behind -> falls forward.
            // Rotate TOWARD the box's direction (no offset).
            targetFacingAngle = signedAngle;
        }
        else
        {
            // Hit from front -> falls backward.
            // Rotate AWAY from the box's direction (180 degree offset),
            // kept within the -180 to 180 range.
            targetFacingAngle = signedAngle > 0 ? signedAngle - 180f : signedAngle + 180f;
        }

        // Force component: momentum-based ratio
        float boxMomentum = agent.collidingBoxRB.mass * flatBoxVelocity.magnitude;
        float forceRatio = Mathf.InverseLerp(minMomentumThreshold, maxMomentumReference, boxMomentum);
        forceRatio = Mathf.Clamp(forceRatio, minForceRatio, 1f);

        agent.forceRatio = forceRatio;
        agent.targetRotationAngle = targetFacingAngle * forceRatio;
    }

    //Rotation based on targetRotationAngle
    public void RotateOnBoxImpact(CustomerManager agent)
    {

        Quaternion startRotation = agent.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, agent.targetRotationAngle, 0f);

        // Stronger hits rotate faster (inverse lerp between max and min duration)
        float duration = Mathf.Lerp(maxRotateDuration, minRotateDuration, agent.forceRatio);

        agent.transform.rotation = targetRotation;
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
        if (Time.time > lastSearchForPlayerStateCD + agent.SearchForPlayerStateCD)
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
        if (Time.time > lastGetHitStateCD + agent.GetHitStateCD)
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
