using System.Collections.Generic;
using UnityEngine;

public class CustomerFunctions
{
    CustomerManager agent;

    private GameObject shelfObjectParent;
    private GameObject palletObject;
    private List<CardboardBoxData> palletDataList;

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
    }

    //Searches for specific objects in scene, and picks x random goods that the customer needs.
    public void GenerateSpecificGoods()
    {
        shelfObjectParent = GameObject.Find("Shelfs");
        palletObject = GameObject.Find("KolonialPallet");

        palletDataList = palletObject.GetComponent<KolonialPallet>().boxDataList;

        for (int i = 0; i < agent.nrGoodsNeeded; i++)
        {
            int rand = Random.Range(0, palletDataList.Count);
            agent.remainingGoodsList.Add(palletDataList[rand]);
            palletDataList.RemoveAt(rand);
        }
        GetShelfPositions();
    }

    //Gets the shelf positions of said goods and pairs them into "goodsShelfPairs".
    private void GetShelfPositions()
    {
        Shelf[] shelves = shelfObjectParent.GetComponentsInChildren<Shelf>();

        foreach (CardboardBoxData box in agent.remainingGoodsList)
        {
            foreach (Shelf shelf in shelves)
            {
                string shelfGoodsType = shelf.goodsType.ToString();

                if (shelfGoodsType == box.boxID)
                {
                    Transform shelfArrow = shelf.transform.Find("shelfArrow");
                    agent.shelfPosPairs[box] = shelfArrow.position;
                    agent.shelfIDPairs[box.boxID] = shelf;
                }
            }
        }
    }
}
