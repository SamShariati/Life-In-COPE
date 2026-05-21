using UnityEngine;
using static Shelf;

public class CashRegister : MonoBehaviour, IInteractable
{


    [HideInInspector] public GameObject player;
    private ScanningGoods scanningGoods;
    [HideInInspector] public Vector3 registerPos;

    [HideInInspector] public bool inScanningMode = false;
    [HideInInspector] public CustomerManager customerFirstInLine;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        

        registerPos = transform.Find("cashRegister").position;








        scanningGoods = new ScanningGoods(this);
    }






    public string GetInteractPrompt(PlayerInteract player)
    {
        //float distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(distance);

        return "Test";

    }

    public void Interact(PlayerInteract player)
    {
        if (GetInteractConditions(player))
        {
            scanningGoods.Activate(player);
        }
      
    }

    private bool GetInteractConditions(PlayerInteract player)
    {

        if (!inScanningMode && !player.Inventory.currentlyHoldingBox)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
