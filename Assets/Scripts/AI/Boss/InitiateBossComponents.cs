using System.Collections.Generic;
using UnityEngine;

public class InitiateBossComponents : MonoBehaviour
{
    public Dictionary<Shelf, bool> shelvesFullyStockedStatus = new Dictionary<Shelf, bool>();
    

    BossManager agent;

    public InitiateBossComponents (BossManager agent)
    {
        this.agent = agent;
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

            if (shelf.remainingGoodsToStock <= 0)
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = true;
            }
            else
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = false;
                agent.dataTracker.nrEmptyShelvesLeft++;
            }
        }
    }
}
