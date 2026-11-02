using System.Collections.Generic;
using UnityEngine;

public class BossFunctions
{

    BossManager agent;

    public float idleTimer = 0f;

    public BossFunctions(BossManager agent)
    {
        this.agent = agent;
    }





    public void WarnResetFlags()
    {
        agent.BB.Inspection.stateActivated = false;
        agent.BB.Inspection.isCurrInspectingShelf = false;

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
