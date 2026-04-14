using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour, PlayerInput.IPlayerActions
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI interactText;

    private PlayerInput _input;
    public PlayerInventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = GetComponent<PlayerInventory>();
        _input = new PlayerInput();
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
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TryInteract();
    }
    public void OnMovement(InputAction.CallbackContext ctx) { }
    public void OnLook(InputAction.CallbackContext ctx) { }
    public void OnSprint(InputAction.CallbackContext ctx) { }
    public void OnDrop(InputAction.CallbackContext ctx) { }
    public void OnThrow(InputAction.CallbackContext ctx) { }

    //---------------------------------------------------------


    private void Update()
    {
        CheckForInteractable();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            hit.collider.GetComponent<IInteractable>()?.Interact(this);
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactText.text = interactable.GetInteractPrompt(this);
                interactText.gameObject.SetActive(true);
                return;
            }
        }
        interactText.gameObject.SetActive(false);
    }
}