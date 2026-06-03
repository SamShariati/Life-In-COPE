using System.Collections.Generic;
using UnityEngine;
using static Shelf;

public class CashRegister : MonoBehaviour, IInteractable
{


    [HideInInspector] public GameObject player;
    private ScanningGoods scanningGoods;
    [HideInInspector] public Vector3 registerPos;

    [HideInInspector] public bool inScanningMode = false;
    [HideInInspector] public CustomerManager customerFirstInLine;
    [HideInInspector] public int itemsLeftToScan;
    [HideInInspector] public List<GameObject> itemsToScanList;

    private Transform goodsPositions;
    public List<Vector3> goodsPosList = new List<Vector3>();

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        

        registerPos = transform.Find("cashRegister").position;

        goodsPositions = transform.Find("goodsPositions");

        scanningGoods = new ScanningGoods(this);

        GetGoodsPositions();
    }


    public string GetInteractPrompt(PlayerInteract player)
    {
        //float distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(distance);

        return "Test";

    }

    public void Interact(PlayerInteract player)
    {
        if (GetInteractConditions(player))
        {
            scanningGoods.Activate(player);
        }
      
    }

    private bool GetInteractConditions(PlayerInteract player)
    {

        if (!inScanningMode && !player.Inventory.currentlyHoldingBox)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void GetGoodsPositions()
    {
        foreach (Transform obj in goodsPositions)
        {
            goodsPosList.Add(obj.position);
        }
    }
}
