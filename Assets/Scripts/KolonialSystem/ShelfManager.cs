using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;


//--------------------------------------------------------
//THIS SCRIPT ONLY WORKS WITH ONE KOLONIALPALLET IN SCENE!
//--------------------------------------------------------
public class ShelfManager : MonoBehaviour
{
    public static ShelfManager Instance { get; private set; }

    private void Awake()
    {


        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private KolonialPallet pallet;
    private List<CardboardBoxData> goodsDataList;
    [SerializeField] List<CardboardBoxData> goodsOnPallet;
    public List<Shelf> shelfList;
    [HideInInspector] public Dictionary<int, List<Shelf>> shelvesByAisle = new Dictionary<int, List<Shelf>>();


    void Start()
    {
        pallet = FindAnyObjectByType<KolonialPallet>();
        shelfList = new List<Shelf>(FindObjectsByType<Shelf>());
        goodsDataList = pallet.allBoxTypes;
        goodsOnPallet = pallet.boxDataList;
        SetShelfStatus();
        SetShelfData();
        PlaceAllShelves();
        DisableShelfArrow();
    }


    private void SetShelfData()
    {
        foreach (Shelf shelf in shelfList)
        {
            //shelf.goodsDataList = goodsDataList;
            string shelfGoodsType = shelf.goodsType.ToString();

            foreach (CardboardBoxData boxData in goodsDataList)
            {
                if (shelfGoodsType == boxData.boxID)
                {
                    shelf.stockedPrefab = boxData.stockedPrefab;
                    shelf.placedPrefab = boxData.placedPrefab;
                    shelf.transparentPrefab = boxData.transparentPrefab;

                }
            }

        }
    }
    private void SetShelfStatus()
    {
        foreach (Shelf shelf in shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();
            string shelfType = shelf.shelfType.ToString();
            bool goodsTypeInPallet = false;
            if (shelfGoodsType == "none")
            {
                continue;
            }

            foreach (CardboardBoxData box in goodsOnPallet)
            {
                if (shelfGoodsType == box.boxID)
                {
                    goodsTypeInPallet = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (goodsTypeInPallet && shelfType != "decour")
            {
                shelf.shelfStatus = Shelf.ShelfStatus.empty;
            }
            else
            {
                shelf.shelfStatus = Shelf.ShelfStatus.stocked;
            }

        }

    }
    private void PlaceAllShelves()
    {
        foreach (Shelf shelf in shelfList)
        {
            shelf.PlaceGoodsInShelves();
        }
    }

    private void SortShelvesByAisle()
    {
        for (int i = 1;  i <= 4; i++)
        {
            
            foreach (Shelf shelf in shelfList)
            {
                int shelfAisle = (int)shelf.aisle;

            }
        }
    }

    private void GetCorrectAisle(CustomerManager agent)
    {

        foreach (Shelf shelf in ShelfManager.Instance.shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();

            if (shelfGoodsType == agent.currentChosenGood.boxID)
            {
                agent.aisleID = (int)shelf.aisle;
            }
        }

    }

    //----------------SHELF ARROWS--------------------------

    public Vector3 GetArrowPosition(string id)
    {
        foreach (Shelf shelf in shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();
            string shelfType = shelf.shelfType.ToString();
            if (shelfGoodsType == id && shelfType != "decour")
            {
                return shelf.shelfArrow.position;
            }
        }
        return new Vector3(0, 0, 0);

    }

    public void EnableShelfArrow(string id)
    {
        
        foreach (Shelf shelf in shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();
            string shelfType = shelf.shelfType.ToString();
            if (shelfGoodsType == id && shelfType != "decour")
            {
                shelf.shelfArrow.gameObject.SetActive(true);
            }
        }
    }
    public void DisableShelfArrow()
    {
        foreach(Shelf shelf in shelfList)
        {
            shelf.shelfArrow.gameObject.SetActive(false);
        }
    }
    //-----------------------------------------------------
}
