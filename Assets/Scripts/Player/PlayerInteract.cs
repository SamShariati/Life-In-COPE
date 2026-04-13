using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI interactText;

    private PlayerInput _input;

    private void Awake()
    {
        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= OnInteract;
        _input.Player.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        TryInteract();
    }


    private void Update()
    {
        CheckForInteractable();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            hit.collider.GetComponent<IInteractable>()?.Interact();
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
                interactText.text = interactable.GetInteractPrompt();
                interactText.gameObject.SetActive(true);
                return;
            }
        }
        interactText.gameObject.SetActive(false);
    }
}