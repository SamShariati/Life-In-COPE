using System.Collections.Generic;
using UnityEngine;

public class BossFunctions : MonoBehaviour
{

    BossManager agent;

    public BossFunctions(BossManager agent)
    {
        this.agent = agent;
    }


    private void UpdateShelvesFullyStockedStatus()
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {

            if (shelf.remainingGoodsToStock <= 0)
            {
                agent.BB.Inspection.shelvesFullyStockedStatus[shelf] = true;
            }
        }
    }
}
