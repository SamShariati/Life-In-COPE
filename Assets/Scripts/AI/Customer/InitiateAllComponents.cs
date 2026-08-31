using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InitiateAllComponents 
{

    CustomerManager agent;

    private GameObject shelfObjectParent;
    private GameObject palletObject;
    private GameObject cashRegisterObject;
    private List<CardboardBoxData> palletDataList;
    private GameObject aisles;

    public InitiateAllComponents(CustomerManager agent)
    {
        this.agent = agent;
    }

    public void GenerateAllComponents()
    {
        GenerateNavPositions();
        GenerateSpecificGoods();
        GetCashRegister();
        GetHeadObject();
        GetPlayerObject();
        GetAllAislePositions();
    }

    private void GetAllAislePositions()
    {
        aisles = GameObject.Find("Aisles");

        for (int i = 1; i <= 4; i++)
        {
            Transform aisle = aisles.transform.Find("Aisle" + i);
            List<Transform> targetList = new List<Transform>();

            foreach (Transform pos in aisle)
            {
                targetList.Add(pos);
            }

            agent.allAislePositions[i] = targetList;
        }
    }


    private void GetCashRegister()
    {
        cashRegisterObject = GameObject.Find("cashier");
        agent.cashRegister = cashRegisterObject.GetComponent<CashRegister>();

    }

    private void GetHeadObject()
    {
        agent.headObject = agent.transform.Find("root/pelvis/spine_01/spine_02/spine_03/neck_01/head");
    }

    private void GetPlayerObject()
    {
        agent.player = GameObject.FindWithTag("Player").transform;
        agent.playerMovement = agent.player.GetComponent<PlayerMovement>();
    }

    //Searches for specific objects in scene, and picks x random goods that the customer needs.
    private void GenerateSpecificGoods()
    {
        shelfObjectParent = GameObject.Find("Shelfs");
        palletObject = GameObject.Find("KolonialPallet");


        palletDataList = new List<CardboardBoxData>(palletObject.GetComponent<KolonialPallet>().allBoxTypes);

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

        foreach (Shelf shelf in shelves)
        {
            string shelfGoodsType = shelf.goodsType.ToString();

            Transform shelfArrow = shelf.transform.Find("shelfArrow");

            Vector3 shelfPos = new Vector3(shelfArrow.position.x, 0, shelfArrow.position.z);
            agent.allShelfArrowPositions[shelfGoodsType] = shelfPos;
            agent.allShelfPositions[shelfGoodsType] = shelf;
        }
    }

    private void GenerateNavPositions()
    {

        agent.spawnAgentPos = GameObject.Find("spawnAgentPos").transform.position;
        agent.enterStorePos = GameObject.Find("enterStorePos").transform.position;
        agent.exitStorePos = GameObject.Find("exitStorePos").transform.position;
        agent.walkToRegisterPos = GameObject.Find("walkToRegisterPos").transform.position;

    }
}
