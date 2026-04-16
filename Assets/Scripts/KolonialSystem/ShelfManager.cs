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
        shelfList = new List<Shelf>(FindObjectsByType<Shelf>());
        goodsDataList = pallet.allBoxTypes;
        goodsOnPallet = pallet.boxList;
        SetShelfStatus();
        SetShelfPrefab();
        PlaceAllShelves();
    }

    private void SetShelfPrefab()
    {
        foreach (Shelf shelf in shelfList)
        {
            string shelfGoodsType = shelf.goodsType.ToString();

            foreach (CardboardBoxData boxData in goodsDataList)
            {
                if (shelfGoodsType == boxData.boxID)
                {
                    shelf.stockedPrefab = boxData.stockedPrefab;
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
            if (goodsTypeInPallet)
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
        foreach(Shelf shelf in shelfList)
        {
            shelf.PlaceGoodsInShelves();
        }
    }
}
