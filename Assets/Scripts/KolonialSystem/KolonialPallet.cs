using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class KolonialPallet : MonoBehaviour, IInteractable
{
    [SerializeField] private List<CardboardBoxData> possibleBoxTypes;
    [SerializeField] int maxNrCrates;
    [SerializeField] int nrCratesLeft;
    Transform firstLayer;
    Transform secondLayer;
    Transform thirdLayer;
    Transform fourthLayer;
    private List<CardboardBox> crates = new List<CardboardBox>();

    void Start()
    {
        nrCratesLeft = maxNrCrates;
        firstLayer = transform.Find("firstLayer");
        secondLayer = transform.Find("secondLayer");
        thirdLayer = transform.Find("thirdLayer");
        fourthLayer = transform.Find("fourthLayer");

        GenerateCrates();
    }

    private void GenerateCrates()
    {
        if (maxNrCrates > possibleBoxTypes.Count)
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

        for (int i = 0; i < maxNrCrates; i++)
        {
            crates.Add(new CardboardBox(shuffled[i]));
        }
    }

    private void RemoveACrate()
    {
        if (nrCratesLeft <= 0) return;
        nrCratesLeft -= 1;
        CheckPalletProgression();
    }

    
    private void CheckPalletProgression()
    {
        if (nrCratesLeft == 0)
        {
            firstLayer.gameObject.SetActive(false);
        }
        else if (nrCratesLeft <= maxNrCrates * 0.25f)
        {
            secondLayer.gameObject.SetActive(false);
        }
        else if (nrCratesLeft <= maxNrCrates * 0.50f)
        {
            thirdLayer.gameObject.SetActive(false);
        }
        else if (nrCratesLeft <= maxNrCrates * 0.75f)
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
