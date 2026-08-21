using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour, PlayerInput.IPlayerActions
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI interactText;

    [SerializeField] private LayerMask interactLayerMask;

    public PlayerInput Input { get; private set; }
    public PlayerInventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = GetComponent<PlayerInventory>();
        Input = new PlayerInput();
    }

    private void OnEnable()
    {
        Input.Player.Enable();
        Input.Player.AddCallbacks(this);
    }

    private void OnDisable()
    {
        Input.Player.Disable();
        Input.Player.RemoveCallbacks(this);
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
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
        {
            hit.collider.GetComponentInParent<IInteractable>()?.Interact(this);
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
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