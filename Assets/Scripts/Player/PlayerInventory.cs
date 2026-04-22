using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInventory : MonoBehaviour, PlayerInput.IPlayerActions
{

    public bool currentlyHoldingBox;
    public CardboardBoxObject heldBox;
    [SerializeField] Transform holdPoint;
    [SerializeField] GameObject boxPrefab;
    [HideInInspector] public ShelfManager shelfManager;

    private PlayerInput _input;
    private void Awake()
    {   
        _input = new PlayerInput();
        currentlyHoldingBox = false;
        shelfManager = FindAnyObjectByType<ShelfManager>();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.AddCallbacks(this);
    }

    private void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.RemoveCallbacks(this);
    }


    //-----------------------INPUT ACTIONS---------------------
    public void OnInteract(InputAction.CallbackContext ctx) { }
    public void OnMovement(InputAction.CallbackContext ctx) { }
    public void OnLook(InputAction.CallbackContext ctx) { }
    public void OnSprint(InputAction.CallbackContext ctx) { }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) ThrowBox();
    }
    public void OnDrop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) DropBox();
    }

    //---------------------------------------------------------

    public bool IsHoldingBox()
    {
        if (currentlyHoldingBox)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddBoxFromPallet(CardboardBoxData data)
    {
        if (currentlyHoldingBox)
        {
            return;
        }

        GameObject box = Instantiate(boxPrefab);
        heldBox = box.GetComponent<CardboardBoxObject>();
        heldBox.Initiate(data);
        heldBox.GetPickedUp(holdPoint);
        currentlyHoldingBox = true;
        shelfManager.EnableShelfArrow(heldBox);
    }

    //----------------HANDLING BOXES-------------------
    public void PickUpBox(CardboardBoxObject boxObject)
    {
        if (currentlyHoldingBox) return;

        heldBox = boxObject;
        heldBox.GetPickedUp(holdPoint);
        currentlyHoldingBox = true;
        shelfManager.EnableShelfArrow(boxObject);
    }
    public void DropBox()
    {
        if (!currentlyHoldingBox) return;
        heldBox.GetDropped();
        heldBox = null;
        currentlyHoldingBox = false;
        shelfManager.DisableShelfArrow();
    }

    public void ThrowBox()
    {
        if (!currentlyHoldingBox) return;
        heldBox.GetThrown();
        heldBox = null;
        currentlyHoldingBox = false;
        shelfManager.DisableShelfArrow();
    }

    public void DestroyBox()
    {
        GameObject.Destroy(heldBox.gameObject);
        heldBox = null;
        currentlyHoldingBox = false;
        shelfManager.DisableShelfArrow();
    }
    //-------------------------------------------------
}
