using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Shelf : MonoBehaviour
{

    [HideInInspector] public GameObject stockedPrefab;
    [HideInInspector] public GameObject transparentPrefab;

    Transform shelfLayers;
    [SerializeField] List<Transform> productPosList;
    

    public enum GoodsType
    {
        none, animalFood, bakingGoods, alcohol,
        bread, candy, cannedFood, cereal,
        chips, cigarettes, clothes, coffee,
        cookies, diapers, electronics, energyDrink,
        gum, hygiene, jam, juice,
        medicine, party, pasta, soda,
        spices, tacos, tea, toys,
        beer, cakes, peanutButter
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
                            //product.transform.position = new Vector3(pos.position.x, pos.position.y + (pos.localScale.y / 2f), pos.position.z);
                            product.transform.position = pos.position;
                        }
                        else
                        {
                            GameObject product = Instantiate(transparentPrefab);
                            product.transform.SetParent(layer);
                            //product.transform.position = new Vector3(pos.position.x, pos.position.y + (pos.localScale.y / 2f), pos.position.z);
                            product.transform.position = pos.position;
                        }
                    }
                }

                break;

            case ShelfStatus.stocked:

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
                        //product.transform.position = new Vector3(pos.position.x, pos.position.y + (product.transform.localScale.y / 2f), pos.position.z);
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
