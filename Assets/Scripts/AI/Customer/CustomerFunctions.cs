using System.Collections.Generic;
using UnityEngine;

public class CustomerFunctions
{
    CustomerManager agent;

    private GameObject shelfObjectParent;
    private GameObject palletObject;
    private GameObject cashRegisterObject;
    private GameObject customerPositions;
    private List<CardboardBoxData> palletDataList;
    public float idleTimer = 0f;

    public CustomerFunctions (CustomerManager agent)
    {
        this.agent = agent;
    }

    public void GenerateAllComponents()
    {
        GenerateNavPositions();
        GenerateSpecificGoods();
        GetCashRegister();
        GetHead();
        GetPlayerObject();
    }

    public void ResetFlagVariables()
    {
        //Shelf State
        agent.shelfRouteReached = false;
        agent.currentlyPickingGoods = false;

    }

    private void GetCashRegister()
    {
        cashRegisterObject = GameObject.Find("cashier");
        agent.cashRegister = cashRegisterObject.GetComponent<CashRegister>();

    }

    private void GetHead()
    {
        agent.headObject = agent.transform.Find("root/pelvis/spine_01/spine_02/spine_03/neck_01/head");
    }

    private void GetPlayerObject()
    {
        agent.player = GameObject.FindWithTag("Player").transform;
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

    private void GenerateNavPositions()
    {
        
        agent.spawnAgentPos = GameObject.Find("spawnAgentPos").transform.position;
        agent.enterStorePos = GameObject.Find("enterStorePos").transform.position;
        agent.exitStorePos = GameObject.Find("exitStorePos").transform.position;
        agent.walkToRegisterPos = GameObject.Find("walkToRegisterPos").transform.position;

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
    //--------------------------------------
}
