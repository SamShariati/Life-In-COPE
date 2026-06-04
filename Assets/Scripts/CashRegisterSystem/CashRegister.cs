using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Shelf;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

public class CashRegister : MonoBehaviour, IInteractable
{


    [HideInInspector] public GameObject player;
    public GameObject bagPrefab;
    private ScanningGoods scanningGoods;
    [HideInInspector] public Vector3 registerPos;

    [HideInInspector] public bool inScanningMode = false;
    [HideInInspector] public CustomerManager customerFirstInLine;
    [HideInInspector] public int itemsLeftToScan;
    [HideInInspector] public List<GameObject> itemsToScanList;

    private Transform goodsPositions;
    private Transform objectToStoreGoodsIn;
    [HideInInspector] public Transform bagPosition;
    public List<Transform> goodsPosList = new List<Transform>();

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        

        registerPos = transform.Find("cashRegister").position;

        goodsPositions = transform.Find("goodsPositions");
        objectToStoreGoodsIn = transform.Find("goodsBank");
        bagPosition = transform.Find("bagPosition");

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
            goodsPosList.Add(obj);
        }
    }

    public void PlaceGoodsOnRegister()
    {

        for (int i = 0; i < itemsToScanList.Count; i++)
        {
            GameObject item = Instantiate(itemsToScanList[i]);
            item.transform.SetParent(objectToStoreGoodsIn);
            item.transform.position = goodsPosList[i].position;
            item.transform.rotation = goodsPosList[i].rotation;
 
        }

    }
}
