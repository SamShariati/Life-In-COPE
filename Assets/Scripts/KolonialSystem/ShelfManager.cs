using System.Collections.Generic;
using UnityEngine;


//--------------------------------------------------------
//THIS SCRIPT ONLY WORKS WITH ONE KOLONIALPALLET IN SCENE!
//--------------------------------------------------------
public class ShelfManager : MonoBehaviour
{

    private KolonialPallet pallet;
    private List<CardboardBoxData> goodsDataList;
    [SerializeField] List<CardboardBoxData> goodsOnPallet;
    [SerializeField] List<Shelf> shelfList;



    void Start()
    {
        pallet = FindAnyObjectByType<KolonialPallet>();
        GetShelfList();
        goodsDataList = pallet.allBoxTypes;
        goodsOnPallet = pallet.boxDataList;
        SetShelfStatus();
        SetShelfData();
        PlaceAllShelves();
        DisableShelfArrow();
    }

    private void GetShelfList()
    {
        shelfList = new List<Shelf>(FindObjectsByType<Shelf>());
        foreach (Shelf shelf in shelfList)
        {
            string shelfType = shelf.shelfType.ToString();
            
            if (shelfType == "Decour")
            {
                shelfList.Remove(shelf);
            }
        }
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


    //----------------SHELF ARROWS--------------------------

    public void EnableShelfArrow(CardboardBoxObject box)
    {
        
        foreach (Shelf shelf in shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();
            string shelfType = shelf.shelfType.ToString();
            if (shelfGoodsType == box.data.boxID && shelfType != "decour")
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
