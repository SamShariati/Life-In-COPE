using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class KolonialPallet : MonoBehaviour, IInteractable
{

    [SerializeField] public List<CardboardBoxData> allBoxTypes;
    [SerializeField] int maxNrBoxes;
    int nrBoxesLeft;
    [SerializeField] public List<CardboardBoxData> boxDataList = new List<CardboardBoxData>();
    Transform firstLayer;
    Transform secondLayer;
    Transform thirdLayer;
    Transform fourthLayer;
    
    void Awake()
    {
        nrBoxesLeft = maxNrBoxes;
        firstLayer = transform.Find("firstLayer");
        secondLayer = transform.Find("secondLayer");
        thirdLayer = transform.Find("thirdLayer");
        fourthLayer = transform.Find("fourthLayer");

        GenerateCrates();
    }

    private void GenerateCrates()
    {
        if (maxNrBoxes > allBoxTypes.Count)
        {
            Debug.LogError("maxNrCrates exceeds the number of available crate types!");
            return;
        }
        List<CardboardBoxData> shuffled = new List<CardboardBoxData>(allBoxTypes);

        // Fisher-Yates shuffle
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            CardboardBoxData temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        for (int i = 0; i < maxNrBoxes; i++)
        {
            boxDataList.Add(shuffled[i]);
            //Debug.Log(shuffled[i].boxID);
        }
        
    }
    private void RemoveACrate()
    {
        nrBoxesLeft -= 1;
        CheckPalletProgression();
    }
    public CardboardBoxData SpawnABox()
    {
        int rand = Random.Range(0, boxDataList.Count);
        CardboardBoxData data = boxDataList[rand];
        boxDataList.RemoveAt(rand);
        return data;
    }
    private void CheckPalletProgression()
    {
        if (nrBoxesLeft == 0)
        {
            firstLayer.gameObject.SetActive(false);
        }
        if (nrBoxesLeft <= maxNrBoxes * 0.25f)
        {
            secondLayer.gameObject.SetActive(false);
        }
        if (nrBoxesLeft <= maxNrBoxes * 0.50f)
        {
            thirdLayer.gameObject.SetActive(false);
        }
        if (nrBoxesLeft <= maxNrBoxes * 0.75f)
        {
            fourthLayer.gameObject.SetActive(false);
        }
    }

    //IInteractable methods--------------
    public string GetInteractPrompt(PlayerInteract player)
    {
        if (player.Inventory.IsHoldingBox() || nrBoxesLeft <= 0)
        {
            return "";
        }
        return "Grab a box";
    }

    public void Interact(PlayerInteract player)
    {
        if (!player.Inventory.IsHoldingBox() && nrBoxesLeft > 0)
        {
            CardboardBoxData box = SpawnABox();
            player.Inventory.AddBoxFromPallet(box);
            RemoveACrate();
        }
        
    }

    //-----------------------------------
}
