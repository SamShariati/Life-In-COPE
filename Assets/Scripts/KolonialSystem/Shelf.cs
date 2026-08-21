using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;

public class Shelf : MonoBehaviour, IInteractable
{

    [HideInInspector] public GameObject stockedPrefab;
    [HideInInspector] public GameObject transparentPrefab;
    [HideInInspector] public GameObject placedPrefab;
    [HideInInspector] public Transform shelfArrow;
    [HideInInspector] public int remainingStockCount; //ATM if variable = 0 --> shelf stocked, else not
    [HideInInspector] private StockingShelf stockingShelf;
    //[HideInInspector] public List<CardboardBoxData> goodsDataList;

    [HideInInspector] public GameObject player;

    Transform shelfLayers;
    [SerializeField] List<Transform> productPosList;
    [HideInInspector] public List<Transform> stockingPosList = new List<Transform>();
    


    public enum GoodsType
    {
        none, alcohol, animalFood, bakingGoods,
        beer, bread, cakes, candy,
        cannedFood, cereal, chips, cleaning,
        coffee, cookies, electronics, energyDrink, 
        hygiene, jam, peanutButter, proteinDrink,
        sauces, shampoo, soda, tacos,
        tea, toiletPaper, toys
    }

    public enum Aisle
    {
        none, one, two, three, four
    }
    
    public enum ShelfStatus
    {
        none,
        empty,
        stocked
    }
    public enum ShelfType
    {
        custom,
        decour
    }

    public GoodsType goodsType;
    public Aisle aisle;
    public ShelfType shelfType;
    public ShelfStatus shelfStatus;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        shelfLayers = transform.Find("layers");
        shelfArrow = transform.Find("shelfArrow");
        remainingStockCount = 20;

        foreach (Transform layer in shelfLayers)
        {
            foreach(Transform posObject in layer)
            {
                productPosList.Add(posObject);
            }
        }
        Transform secondLayer = shelfLayers.GetChild(1);
        Transform thirdLayer = shelfLayers.GetChild(2);
        foreach (Transform pos in secondLayer) stockingPosList.Add(pos);
        foreach (Transform pos in thirdLayer) stockingPosList.Add(pos);



    }

    private void Start()
    {

        stockingShelf = new StockingShelf(this);
    }

    public void PlaceGoodsInShelves()
    {
        switch (shelfStatus)
        {

            case ShelfStatus.empty:

                foreach (Transform layer in shelfLayers)
                {

                    Transform firstLayer = shelfLayers.GetChild(0);
                    Transform fourthLayer = shelfLayers.GetChild(3);

                    List<Transform> positions = new List<Transform>();
                    foreach (Transform pos in layer)
                    {
                        positions.Add(pos);
                    }

                    foreach (Transform pos in positions)
                    {

                        if (layer == firstLayer || layer == fourthLayer)
                        {
                            GameObject product = Instantiate(stockedPrefab);
                            product.transform.SetParent(layer);
                            product.transform.position = pos.position;
                            product.transform.rotation = pos.rotation;
                        }
                        else
                        {
                            GameObject product = Instantiate(transparentPrefab);
                            product.transform.SetParent(layer);
                            product.transform.position = pos.position;
                            product.transform.rotation = pos.rotation;
                        }
                    }
                }

                break;

            case ShelfStatus.stocked:

                remainingStockCount = 0;
                foreach (Transform layer in shelfLayers)
                {

                    List<Transform> positions = new List<Transform>();
                    foreach (Transform pos in layer)
                    {
                        positions.Add(pos);
                    }

                    foreach (Transform pos in positions)
                    {
                        GameObject product = Instantiate(stockedPrefab);
                        product.transform.SetParent(layer);
                        product.transform.position = pos.position;
                        product.transform.rotation = pos.rotation;
                    }

                }

                break;

            case ShelfStatus.none:
                //Debug.Log("shelfStatus is None");
                break;
        }
    }

    public void Interact(PlayerInteract player)
    {
        if (player.Inventory.currentlyHoldingBox && remainingStockCount > 0)
        {
            string shelfGoodsType = goodsType.ToString();
            if (shelfGoodsType == player.Inventory.heldBox.data.boxID && !PlayerState.Instance.currentlyBeingFollowed)
            {
                stockingShelf.Activate(player);
            }
        }

    }
    public string GetInteractPrompt(PlayerInteract player)
    {
        string shelfGoodsType = goodsType.ToString();
        if (GetInteractConditions(player))
        {
            return $"Stock the shelf ({shelfGoodsType})";     
        }
        else { return ""; }
    }        

    private bool GetInteractConditions(PlayerInteract player)
    {

        string shelfGoodsType = goodsType.ToString();
        if (!PlayerState.Instance.inStockingMode && player.Inventory.currentlyHoldingBox && remainingStockCount > 0
            && shelfGoodsType == player.Inventory.heldBox.data.boxID)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
