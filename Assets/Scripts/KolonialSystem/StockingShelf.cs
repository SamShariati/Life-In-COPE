using UnityEngine;
using UnityEngine.InputSystem;

public class StockingShelf : MonoBehaviour, PlayerInput.IShelfActions
{
    PlayerInput _input;

    private void Awake()
    {
        _input = new PlayerInput();
      
    }

    private void OnEnable()
    {
        _input.Shelf.Enable();
        _input.Shelf.AddCallbacks(this);
    }

    private void OnDisable()
    {
        _input.Shelf.Disable();
        _input.Shelf.RemoveCallbacks(this);
    }

    public void OnStop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) ; //Metod här
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) ; //Metod här
    }
}
