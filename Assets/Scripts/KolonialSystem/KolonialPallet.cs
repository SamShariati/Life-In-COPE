using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class KolonialPallet : MonoBehaviour, IInteractable
{
    [SerializeField] private List<CardboardBoxData> possibleBoxTypes;
    [SerializeField] int maxNrBoxes;
    [SerializeField] int nrBoxesLeft;
    [SerializeField] private List<CardboardBox> crates = new List<CardboardBox>();
    Transform firstLayer;
    Transform secondLayer;
    Transform thirdLayer;
    Transform fourthLayer;
    



    void Start()
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
        if (maxNrBoxes > possibleBoxTypes.Count)
        {
            Debug.LogError("maxNrCrates exceeds the number of available crate types!");
            return;
        }
        List<CardboardBoxData> shuffled = new List<CardboardBoxData>(possibleBoxTypes);

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
            crates.Add(new CardboardBox(shuffled[i]));
            Debug.Log(shuffled[i].boxID);
        }
        
    }

    private void RemoveACrate()
    {
        if (nrBoxesLeft <= 0) return;
        nrBoxesLeft -= 1;
        CheckPalletProgression();
    }

    
    private void CheckPalletProgression()
    {
        if (nrBoxesLeft == 0)
        {
            firstLayer.gameObject.SetActive(false);
        }
        else if (nrBoxesLeft <= maxNrBoxes * 0.25f)
        {
            secondLayer.gameObject.SetActive(false);
        }
        else if (nrBoxesLeft <= maxNrBoxes * 0.50f)
        {
            thirdLayer.gameObject.SetActive(false);
        }
        else if (nrBoxesLeft <= maxNrBoxes * 0.75f)
        {
            fourthLayer.gameObject.SetActive(false);
        }
    }

    //IInteractable methods--------------
    public string GetInteractPrompt()
    {
        return "Grab a crate";
    }

    public void Interact()
    {
        RemoveACrate();
    }

    //-----------------------------------
}
