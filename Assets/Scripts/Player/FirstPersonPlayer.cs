
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
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
        _input.Player.Movement.performed += OnMovement;
        _input.Player.Movement.canceled += OnMovement;

        _input.Player.Sprint.performed += OnSprint;
        _input.Player.Sprint.canceled += OnSprint;

        _input.Player.Look.performed += OnLook;
        _input.Player.Look.canceled += OnLook;

    }
    private void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Movement.performed -= OnMovement;
        _input.Player.Movement.canceled -= OnMovement;

        _input.Player.Sprint.performed -= OnSprint;
        _input.Player.Sprint.canceled -= OnSprint;

        _input.Player.Look.performed -= OnLook;
        _input.Player.Look.canceled -= OnLook;

    }

    private void OnMovement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>() * mouseSensitivity;
    }
    private void OnSprint(InputAction.CallbackContext ctx)
    {
        currentSpeed = ctx.performed ? sprintSpeed : walkSpeed;
    }

    void Update()
    {
        MovePlayer();
        HandleMouseLook();
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
}