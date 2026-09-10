using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CashRegister : MonoBehaviour, IInteractable
{


    [HideInInspector] public GameObject player;
    private Transform bagPrefab;
    [HideInInspector] public bool placeBagOnRegister = false;
    
    [HideInInspector] public ScanningGoods scanningGoods;
    [HideInInspector] public Vector3 interactColliderPos;

    [HideInInspector] public CustomerManager customerFirstInLine;
    [HideInInspector] public int itemsLeftToScan;
    [HideInInspector] public List<GameObject> itemsToScanList;

    private Transform goodsPositions;
    private Transform registerInventory;
    [HideInInspector] public Transform pointToLookAt;
    [HideInInspector] public Transform bagPosition;
    public List<Transform> goodsPosList = new List<Transform>();

    public List<GameObject> itemsOnRegisterBand = new List<GameObject>();

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        

        interactColliderPos = transform.Find("interactCollider").position;
        pointToLookAt = transform.Find("pointToLookAt");
        goodsPositions = transform.Find("goodsPositions");
        registerInventory = transform.Find("registerInventory");
        bagPosition = transform.Find("bagPosition");
        bagPrefab = transform.Find("bag");


        GetGoodsPositions();
    }

    private void Start()
    {
        scanningGoods = new ScanningGoods(this);
    }

    private void Update()
    {
        if (itemsOnRegisterBand.Count > 0)
        {
            bagPrefab.gameObject.SetActive(true);         
        }
        else
        {
            bagPrefab.gameObject.SetActive(false);
        }
    }

    public string GetInteractPrompt(PlayerInteract player)
    {
        
        if (PlayerState.Instance.inScanningMode)
        {
            return "";
        }
        else if (PlayerState.Instance.currentlyBeingFollowed)
        {
            return "Help the customer first!";
        }
        else if (player.Inventory.currentlyHoldingBox)
        {
            return "Need to drop the box!";
        }
        else
        {
            return "Start Scanning";
        }

    }

    public void Interact(PlayerInteract player)
    {
        if (GetInteractConditions(player))
        {
            scanningGoods.Activate();
        }
      
    }

    private bool GetInteractConditions(PlayerInteract player)
    {

        if (!PlayerState.Instance.inScanningMode && !PlayerState.Instance.currentlyBeingFollowed && 
            !player.Inventory.currentlyHoldingBox)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void StartCustomerGoodsScan()
    {
        if (scanningGoods.playerInPosition)
        {
            scanningGoods.Activate();
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
            item.transform.SetParent(registerInventory);
            item.transform.position = goodsPosList[i].position;
            item.transform.rotation = goodsPosList[i].rotation;
            
            itemsOnRegisterBand.Add(item);
        }

    }
}
