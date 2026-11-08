using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossFunctions
{

    BossManager agent;

    public float idleTimer = 0f;
    private List<Transform > trashList = new List<Transform>();
    private const float destinationBuffer = 0.75f;
    private const float sampleSearchRadius = 3f;

    public BossFunctions(BossManager agent)
    {
        this.agent = agent;
    }





    public void WarnResetFlags()
    {
        agent.BB.Inspection.stateActivated = false;
        agent.BB.Inspection.isCurrInspectingShelf = false;
        agent.BB.Inspection.shelfToInspectChosen = false;
        agent.AI.BT.inspectShelf.phase = InspectShelf.Phase.Instantiate;
        agent.BB.Inspection.isCurrInspectingTrash = false;
        agent.BB.Inspection.trashToInspectChosen = false;
        agent.AI.BT.inspectTrash.phase = InspectTrash.Phase.Instantiate;
        agent.BB.Inspection.isCurrInspectingRegister = false;
        agent.AI.BT.inspectRegister.phase = InspectRegister.Phase.Instantiate;




    }

    public void UpdateShelvesFullyStockedStatus()
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {

            if (shelf.shelfFullyStocked)
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = true;
            }
        }
    }

    public void SetPlayerDestination()
    {
        agent.navigation.isStopped = false;
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


    public void RotateTowardsTarget(Vector3 target)
    {
        Vector3 toTarget = target - agent.transform.position;
        Vector3 flatDirection = new Vector3(toTarget.x, 0, toTarget.z);

        // Skip rotation if the horizontal distance is (near) zero
        if (flatDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(flatDirection.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation,
        Time.deltaTime * (agent.navigation.angularSpeed / 120));
    }

    public void ChooseTrashToInspect()
    {
        trashList = new List<Transform>();

        foreach (Transform trash in agent.BB.Inspection.trashParent.transform)
        {
            trashList.Add(trash);
        }

        int randNr = Random.Range(0, trashList.Count);

        if (trashList.Count != 0)
        {
            agent.BB.Inspection.chosenTrash = trashList[randNr];
        }
        else
        {
            agent.BB.Inspection.chosenTrash = agent.BB.StoreNav.inspectTrashPos;
        }
    }

    public int GetNrTrashOnGround()
    {
        int count = 0;

        foreach (Transform trash in agent.BB.Inspection.trashParent.transform)
        {
            count++;
        }
        return count;
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
}
