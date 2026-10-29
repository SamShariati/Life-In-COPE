using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class InitiateCustomerComponents
{
    CustomerManager agent;

    private GameObject shelfObjectParent;
    private GameObject palletObject;
    private GameObject cashRegisterObject;
    private List<CardboardBoxData> palletDataList;
    private GameObject aisles;

    public InitiateCustomerComponents(CustomerManager agent)
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
        GetIKRiggingObjects();
        GetAllFoodStalls();

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

            agent.BB.Confused.allAislePositions[i] = targetList;
        }
    }

    private void GetAllFoodStalls()
    {

        GameObject foodStalls = GameObject.Find("FoodStalls");

        foreach (Transform child in foodStalls.transform)
        {
            agent.BB.Sabotage.foodStallList.Add(child);
        }
    }


    private void GetCashRegister()
    {
        cashRegisterObject = GameObject.Find("cashier");
        agent.cashRegister = cashRegisterObject.GetComponent<CashRegister>();

    }

    private void GetHeadObject()
    {
        agent.BB.SFP.headObject = agent.transform.Find("root/pelvis/spine_01/spine_02/spine_03/neck_01/head");
    }

    private void GetIKRiggingObjects()
    {
        agent.BB.IKRigging.IKAimTarget = agent.transform.Find("aimTarget");
        agent.BB.IKRigging.cameraTransform = agent.player.transform.Find("Main Camera");

        Transform chestObj = agent.transform.Find("Rig 1/ChestRig");
        Transform headObj = agent.transform.Find("Rig 1/HeadRig");
        agent.BB.IKRigging.chestRig = chestObj.GetComponent<MultiAimConstraint>();
        agent.BB.IKRigging.headRig = headObj.GetComponent<MultiAimConstraint>();
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
            agent.BB.GSI.remainingGoodsList.Add(palletDataList[rand]);
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
            agent.BB.GSI.allShelfArrowPositions[shelfGoodsType] = shelfPos;
            agent.BB.GSI.allShelfPositions[shelfGoodsType] = shelf;
        }
    }

    private void GenerateNavPositions()
    {

        agent.BB.Store.spawnAgentPos = GameObject.Find("spawnAgentPos").transform.position;
        agent.BB.Store.enterStorePos = GameObject.Find("enterStorePos").transform.position;
        agent.BB.Store.exitStorePos = GameObject.Find("exitStorePos").transform.position;
        agent.BB.Store.walkToRegisterPos = GameObject.Find("walkToRegisterPos").transform.position;

    }
}
