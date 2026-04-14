
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, PlayerInput.IPlayerActions
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed = 10f;
    private float currentSpeed = 5;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private PlayerInput _input;
    private float xRotation = 0f;
    private Vector2 moveInput;
    private Vector2 mouseDelta;

    private float verticalVelocity;
    private float gravity = -9.81f;


    void Awake()
    {
        currentSpeed = walkSpeed;
        _input = new PlayerInput();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
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
    public void OnMovement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>() * mouseSensitivity;
    }
    public void OnSprint(InputAction.CallbackContext ctx)
    {
        currentSpeed = ctx.performed ? sprintSpeed : walkSpeed;
    }
    public void OnInteract(InputAction.CallbackContext ctx) { }
    public void OnDrop(InputAction.CallbackContext ctx) { }
    public void OnThrow(InputAction.CallbackContext ctx) { }

    //--------------------------------------------------------

    void Update()
    {
        MovePlayer();
        HandleMouseLook();
        CheckGravity();
    }
    private void MovePlayer()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        //currentSpeed = Keyboard.current.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        //Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    private void CheckGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; // small negative to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // accumulate gravity
        }
        Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0);
        controller.Move(verticalMovement * Time.deltaTime);
    }
}
