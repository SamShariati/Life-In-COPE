using System.Collections.Generic;
using UnityEngine;

public class InitiateBossComponents
{
    public Dictionary<Shelf, bool> shelvesFullyStockedStatus = new Dictionary<Shelf, bool>();
    

    BossManager agent;

    public InitiateBossComponents (BossManager agent)
    {
        this.agent = agent;
    }

    public void GenerateAllComponents()
    {
        GenerateAllShelfDictionaries();
        GetTrashParentObject();
        GetNavPositions();
        GetPlayerObject();
        //GetSpeechVariables();
    }

    private void GenerateAllShelfDictionaries()
    {
        List<Shelf> shelfList = ShelfManager.Instance.shelfList;

        foreach (Shelf shelf in shelfList)
        {
            Transform shelfArrow = shelf.transform.Find("shelfArrow");
            Vector3 shelfArrowPos = new Vector3(shelfArrow.position.x, 0, shelfArrow.position.z);

            agent.BB.Inspection.allShelfArrowPositions[shelf] = shelfArrowPos;
            agent.BB.Inspection.allShelfPositions[shelf] = shelf.transform.position;

            if (shelf.shelfFullyStocked)
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = true;
            }
            else
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = false;
                BossDataTracker.Instance.nrEmptyShelvesLeft++;
            }
        }
    }

    private void GetPlayerObject()
    {
        agent.player = GameObject.FindWithTag("Player").transform;
        //agent.playerMovement = agent.player.GetComponent<PlayerMovement>();
    }

    private void GetTrashParentObject()
    {
        agent.BB.Inspection.trashParent = GameObject.Find("CurrentTrash");
    }

    private void GetNavPositions()
    {
        agent.BB.StoreNav.inspectTrashPos = GameObject.Find("inspectTrashPos").transform;
        agent.BB.StoreNav.inspectTrashRotPos = GameObject.Find("inspectTrashRotPos").transform;
        agent.BB.StoreNav.inspectRegisterPos = GameObject.Find("inspectRegisterPos").transform;
        agent.BB.StoreNav.inspectRegisterRotPos = GameObject.Find("inspectRegisterRotPos").transform;
    }

}
