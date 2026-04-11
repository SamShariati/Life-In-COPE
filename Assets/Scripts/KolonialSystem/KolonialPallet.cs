using UnityEngine;
using UnityEngine.InputSystem;

public class KolonialPallet : MonoBehaviour, IInteractable
{

    [SerializeField] int maxNrCrates;
    [SerializeField] int nrCratesLeft;
    Transform firstLayer;
    Transform secondLayer;
    Transform thirdLayer;
    Transform fourthLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nrCratesLeft = maxNrCrates;
        firstLayer = transform.Find("firstLayer");
        secondLayer = transform.Find("secondLayer");
        thirdLayer = transform.Find("thirdLayer");
        fourthLayer = transform.Find("fourthLayer");
    }

    // Update is called once per frame
    void Update()
    {
        //Keyboard.current.eKey.wasPressedThisFrame  // GetKeyDown
        //Keyboard.current.eKey.isPressed            // GetKey
        //Keyboard.current.eKey.wasReleasedThisFrame // GetKeyUp

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
