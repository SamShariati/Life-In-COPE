using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInventory : MonoBehaviour, PlayerInput.IPlayerActions
{

    public bool currentlyHoldingBox;
    CardboardBoxObject heldBox;
    [SerializeField] Transform holdPoint;

    private PlayerInput _input;
    private void Awake()
    {   
        _input = new PlayerInput();
        currentlyHoldingBox = false; 
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
    public void AddBox(CardboardBoxData data)
    {
        if (currentlyHoldingBox)
        {
            return;
        }
        //heldBox = box;
        //Debug.Log(heldBox.boxID);
        //currentlyHoldingBox = true;
        GameObject box = Instantiate(data.prefab);
        heldBox = box.GetComponent<CardboardBoxObject>();
        heldBox.Initiate(data);
        heldBox.GetPickedUp(holdPoint);
        currentlyHoldingBox = true;
    }

    public void PickUpBox(CardboardBoxObject boxObject)
    {
        if (currentlyHoldingBox) return;

        heldBox = boxObject;
        heldBox.GetPickedUp(holdPoint);
        currentlyHoldingBox = true;
    }
    public void DropBox()
    {
        if (!currentlyHoldingBox) return;
        heldBox.GetDropped();
        heldBox = null;
        currentlyHoldingBox = false;
    }

    public void ThrowBox()
    {
        if (!currentlyHoldingBox) return;
        heldBox.GetThrown();
        heldBox = null;
        currentlyHoldingBox = false;
    }
}
