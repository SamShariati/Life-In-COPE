using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Shelf : MonoBehaviour
{

    public GameObject stockedPrefab;
    public GameObject transparentPrefab;

    [SerializeField] Transform shelfLayers;
    [SerializeField] List<Transform> productPosList;
    

    public enum GoodsType
    {
        none, animalFood, baking, alcohol,
        bread, candy, cannedFood, cereal,
        chips, cigarettes, clothes, coffee,
        cookies, diapers, electronics, energyDrink,
        gum, hygiene, jam, juice,
        medicine, party, pasta, soda,
        spices, tacos, tea, toys
    }
    
    public enum ShelfStatus
    {
        none,
        empty,
        stocked
    }

    public GoodsType goodsType;
    public ShelfStatus shelfStatus;

    private void Awake()
    {

        shelfLayers = transform.Find("layers");

        foreach (Transform layer in shelfLayers)
        {
            foreach(Transform posObject in layer)
            {
                productPosList.Add(posObject);
            }
        }
    }

    public void PlaceGoodsInShelves()
    {
        switch (shelfStatus)
        {

            case ShelfStatus.empty:

                foreach (Transform layer in shelfLayers)
                {
                    foreach (Transform pos in layer)
                    {
                        GameObject product = Instantiate(transparentPrefab);
                        product.transform.SetParent(layer);
                        product.transform.position = pos.position;
                    }

                }

                break;

            case ShelfStatus.stocked:

                foreach (Transform layer in shelfLayers)
                {
                    foreach (Transform pos in layer)
                    {
                        GameObject product = Instantiate(stockedPrefab);
                        product.transform.SetParent(layer);
                        product.transform.position = pos.position;
                    }

                }

                break;

            case ShelfStatus.none:
                Debug.Log("shelfStatus is None");
                break;
        }
    }

    private void HandleEmpty()
    {

    }
    private void HandleStocked()
    {

    }
}
